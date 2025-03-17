using GitComitter.Root.Application;
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
    public class Program
    {
        public static void Main()
        {
            string jsonConfigPath = "Assets/Config_JSON/gitconfig.json";
            string markdownFilePath = @"..\..\..\README.md";

            if (!File.Exists(jsonConfigPath))
            {
                Console.WriteLine("Git credentials file not found.");
                return;
            }
            if (!File.Exists(markdownFilePath))
            {
                Console.WriteLine("Git credentials file not found.");
                return;
            }

            string jsonContent = File.ReadAllText(jsonConfigPath);
            string markdownContent = File.ReadAllText(markdownFilePath);

            var credentials = JsonConvert.DeserializeObject<cfg_GitCredentials>(jsonContent);

            if (credentials == null || string.IsNullOrEmpty(credentials.Username) || string.IsNullOrEmpty(credentials.Github_token))
            {
                Console.WriteLine("Invalid credentials.");
                return;
            }

            Console.WriteLine("Press enter to commit a new Readme to the repo");

            var kernel = new StandardKernel();
            kernel.Bind<IGitHandler>().To<GitHandler>().WithConstructorArgument("credentials", credentials);

            // Resolve the dependency
            var gitManager = kernel.Get<IGitHandler>();
            Commiter comit_machine = new Commiter(credentials, gitManager);
            comit_machine.UpdateAndCommitReadmeFile(markdownFilePath).Wait();

        }
    }
}
