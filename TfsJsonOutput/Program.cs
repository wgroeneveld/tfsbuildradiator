namespace TfsJsonOutput
{
    #region

    using System;
    using System.Configuration;
    using System.IO;
    using System.Threading;
    using Newtonsoft.Json;
    using TfsCommunicator;

    #endregion

    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("TFS Build status; connecting...");
            BuildCommunicator communicator = new BuildCommunicator(ConfigurationManager.AppSettings["tfsServer"]);

            while (true)
            {
                try
                {
                    GetBuildInfo(communicator);
                }
                catch(Exception e)
                {
                    Console.WriteLine("Whoops, something went wrong: "  + e.Message);
                    Console.WriteLine("Don't worry, we'll retry...");
                }
            }
        }

        private static void GetBuildInfo(BuildCommunicator communicator)
        {
            Console.WriteLine("Getting TFS Build info for all projects...");
            BuildStatus info = communicator.GetBuildInformation();

            CleanBuildStatusFile();
            WriteBuildStatusToFile(info);

            Console.WriteLine("Written build info to buildstatus.json. [waiting...] ");
            Thread.Sleep(20000);
        }

        private static void WriteBuildStatusToFile(BuildStatus info)
        {
            StreamWriter streamWriter = File.CreateText("buildstatus.json");
            new JsonSerializer().Serialize(streamWriter, info);
            streamWriter.Close();
        }

        private static void CleanBuildStatusFile()
        {
            try
            {
                File.Delete("buildstatus.json");
            }
            catch
            {
            }
        }
    }
}