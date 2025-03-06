using GitComitter.Root.Config;
using GitComitter.Root.Git;
using Newtonsoft.Json;
using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GitComitter
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string jsonConfigPath = "Assets/Config_JSON/gitconfig.json";
            string markdownFilePath = "README.md";

            if (!File.Exists(jsonConfigPath))
            {
                Console.WriteLine("Git credentials file not found.");
                return;
            }

            string jsonContent = File.ReadAllText(jsonConfigPath);
            var credentials = JsonConvert.DeserializeObject<cfg_GitCredentials>(jsonContent);

            if (credentials == null || string.IsNullOrEmpty(credentials.Username) || string.IsNullOrEmpty(credentials.Token))
            {
                Console.WriteLine("Invalid credentials.");
                return;
            }

            Console.WriteLine("Press enter to commit a new Readme to the repo");

            var kernel = new StandardKernel();
            kernel.Bind<IGitHandler>().To<GitHandler>().WithConstructorArgument("credentials", credentials);

            // Resolve the dependency
            var gitManager = kernel.Get<IGitHandler>();

            gitManager.CommitAndPushChanges(credentials);

        }
    }
}
