namespace TfsCommunicator.UnitTests
{
    #region

    using System;
    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rhino.Mocks;

    #endregion

    [TestClass]
    public class BuildCommunicatorTest
    {
        private BuildCommunicator communicator;
        private IBuildServer buildServerMock;
        private IBuildDetailSpec buildDetailSpecMock;
        private IBuildQueryResult buildQueryResultMock;


        [TestInitialize]
        public void SetUp()
        {
            this.buildServerMock = MockRepository.GenerateMock<IBuildServer>();
            this.buildDetailSpecMock = MockRepository.GenerateMock<IBuildDetailSpec>();
            this.buildQueryResultMock = MockRepository.GenerateMock<IBuildQueryResult>();

            this.communicator = new BuildCommunicator("http://www.tfs.com");
            this.communicator.buildServer = buildServerMock;
        }

        private IBuildDetail GenerateBuildDetail(string name, DateTime finishTime, string teamProject = "project")
        {
            var build = MockRepository.GenerateStub<IBuildDetail>();
            var definition = MockRepository.GenerateStub<IBuildDefinition>();
            definition.Name = name;
            build.Stub(x => x.BuildDefinition).Return(definition);
            build.Stub(x => x.FinishTime).Return(finishTime);
            build.Stub(x => x.TeamProject).Return(teamProject);

            return build;
        }

        private void SetUpBuildDetails(IBuildDetail[] detials)
        {
            buildQueryResultMock.Expect(x => x.Builds).Return(detials);

            buildServerMock.Expect(x => x.CreateBuildDetailSpec("*")).Return(buildDetailSpecMock);
            buildServerMock.Expect(x => x.QueryBuilds(buildDetailSpecMock)).Return(buildQueryResultMock);
        }

        [TestMethod]
        public void GetBuildInformation_SpecifyBuildDefinitionInQuery()
        {
            SetUpBuildDetails(new[]
                {
                    GenerateBuildDetail("mybuild", DateTime.Now, "some project"),
                });

            buildServerMock.ClearBehavior();
            buildServerMock.Expect(x => x.CreateBuildDetailSpec("*", "mydef")).Repeat.Once().Return(buildDetailSpecMock);
            buildServerMock.Expect(x => x.QueryBuilds(buildDetailSpecMock)).Return(buildQueryResultMock);

            var projects = this.communicator.GetBuildInformation(buildDefinition: "mydef").Projects;

            buildServerMock.VerifyAllExpectations();
            Assert.AreEqual("mybuild", projects[0].DefinitionName);
        }

        [TestMethod]
        public void GetBuildInformatiln_ProjectNameIsTeamProject()
        {
            SetUpBuildDetails(new[]
                {
                    GenerateBuildDetail("mybuild", DateTime.Now, "some project"),
                });

            var projects = this.communicator.GetBuildInformation().Projects;
            Assert.AreEqual("some project", projects[0].Name);
            Assert.AreEqual("mybuild", projects[0].DefinitionName);
        }

        [TestMethod]
        public void GetBuildInformation_SortsByNameThenGroupsAndSortsByFinishTime()
        {
            var fiveDaysAgo = DateTime.Now.AddDays(-5);
            SetUpBuildDetails(new[]
                {
                    GenerateBuildDetail("mybuild", DateTime.Now),
                    GenerateBuildDetail("aCoolBuild", DateTime.Now.AddDays(-10)),
                    GenerateBuildDetail("aCoolBuild", fiveDaysAgo)
                });

            var projects = this.communicator.GetBuildInformation().Projects;
            Assert.AreEqual(2, projects.Count);
            Assert.AreEqual("aCoolBuild", projects[0].DefinitionName);
            Assert.AreEqual(fiveDaysAgo, projects[0].FinishTime);
            Assert.AreEqual("mybuild", projects[1].DefinitionName);
        }
    }
}