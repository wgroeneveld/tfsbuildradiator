using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;

namespace TfsCommunicator
{
    [ExcludeFromCodeCoverage]
    public class BuildCommunicator : IBuildCommunicator
    {
        private string tfsServerAddress;

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
                    AddBuiltToParentProject(buildStatus, definitionName, project, maxRuns);
                }
                else
                {
                    currentDefinition = definitionName;
                    buildStatus.Projects.Add(project);
                }
            }
            return buildStatus;
        }

        private IOrderedEnumerable<IBuildDetail> GetBuildsFromTfs(int maxDays, string teamProject, string buildDefinition)
        {
            var tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(tfsServerAddress));
            IBuildServer buildServer = tfs.GetService<IBuildServer>();

            IBuildDetailSpec spec = string.IsNullOrEmpty(buildDefinition) ? 
                buildServer.CreateBuildDetailSpec(teamProject) : 
                buildServer.CreateBuildDetailSpec(teamProject, buildDefinition);

            spec.MinFinishTime = DateTime.Now.Subtract(TimeSpan.FromDays(maxDays));
            spec.MaxFinishTime = DateTime.Now;
            spec.QueryDeletedOption = QueryDeletedOption.IncludeDeleted;

            var builds = buildServer.QueryBuilds(spec).Builds.OrderBy(b => b.BuildDefinition.Name).ThenByDescending(b => b.FinishTime);
            return builds;
        }


        private static Project MapBuildToProject(IBuildDetail build, string definitionName)
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

        private void AddBuiltToParentProject(BuildStatus buildStatus, string definitionName, Project project, int maxRuns)
        {
            var parent = buildStatus.Projects.First(p => p.DefinitionName == definitionName);
            if (parent.Runs.Count < maxRuns)
            {
                parent.Runs.Add(project);
            }
        }
    }
}
