namespace TfsCommunicator
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.Client;

    #endregion

    public class BuildCommunicator : IBuildCommunicator
    {
        private readonly string tfsServerAddress;
        internal IBuildServer buildServer;

        private IBuildServer BuildServer
        {
            get
            {
                if (buildServer == null)
                {
                    TfsTeamProjectCollection tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(tfsServerAddress));
                    this.buildServer = tfs.GetService<IBuildServer>();
                }
                return this.buildServer;
            }
        }

        public BuildCommunicator(string tfsServerAddress)
        {
            this.tfsServerAddress = tfsServerAddress;
        }

        public BuildStatus GetBuildInformation(int maxDays = 5, int maxRuns = 10, string teamProject = "*", string buildDefinition = "")
        {
            var buildStatus = new BuildStatus();

            var builds = GetBuildsFromTfs(maxDays, teamProject, buildDefinition);

            var currentDefinition = string.Empty;

            foreach (var build in builds)
            {
                string definitionName = build.BuildDefinition.Name;
                var project = MapBuildToProject(build, definitionName);

                if (definitionName == currentDefinition)
                {
                    AddBuildToParentProject(buildStatus, definitionName, project, maxRuns);
                }
                else
                {
                    currentDefinition = definitionName;
                    buildStatus.Projects.Add(project);
                }
            }
            return buildStatus;
        }

        private IEnumerable<IBuildDetail> GetBuildsFromTfs(int maxDays, string teamProject, string buildDefinition)
        {
            IBuildDetailSpec spec = string.IsNullOrEmpty(buildDefinition)
                                        ? BuildServer.CreateBuildDetailSpec(teamProject)
                                        : BuildServer.CreateBuildDetailSpec(teamProject, buildDefinition);

            spec.MinFinishTime = DateTime.Now.Subtract(TimeSpan.FromDays(maxDays));
            spec.MaxFinishTime = DateTime.Now;
            spec.QueryDeletedOption = QueryDeletedOption.IncludeDeleted;

            var builds = BuildServer.QueryBuilds(spec).Builds.OrderBy(b => b.BuildDefinition.Name).ThenByDescending(b => b.FinishTime);
            return builds;
        }


        private Project MapBuildToProject(IBuildDetail build, string definitionName)
        {
            var project = new Project
                {
                    DefinitionName = definitionName,
                    Name = build.TeamProject,
                    Status = build.Status.ToString(),
                    StartTime = build.StartTime,
                    FinishTime = build.FinishTime
                };
            return project;
        }

        private void AddBuildToParentProject(BuildStatus buildStatus, string definitionName, Project project, int maxRuns)
        {
            var parent = buildStatus.Projects.First(p => p.DefinitionName == definitionName);
            if (parent.Runs.Count < maxRuns)
            {
                parent.Runs.Add(project);
            }
        }
    }
}