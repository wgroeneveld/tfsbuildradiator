namespace TfsCommunicator
{
    #region

    using System.Collections.Generic;

    #endregion

    public class BuildStatus
    {
        public BuildStatus()
        {
            Projects = new List<Project>();
        }

        public List<Project> Projects { get; private set; }
    }
}