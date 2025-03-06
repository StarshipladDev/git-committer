using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using GitComitter.Root.Config;
using GitComitter.Root.Git;
using Newtonsoft.Json;

namespace GitComitter.Root.Application
{
    public class Commiter
    {

        IGitHandler _gitHandler;
        cfg_GitCredentials _gitCredentials;
        public Commiter(cfg_GitCredentials _gitCredentials, IGitHandler _gitHandler)
        {
            this._gitCredentials = _gitCredentials;
            this._gitHandler = _gitHandler;
        }

        public void UpdateAndCommitReadmeFile(string filePath)
        {

                
                if (_gitCredentials == null || string.IsNullOrEmpty(_gitCredentials.Username) || string.IsNullOrEmpty(_gitCredentials.Token))
                {
                    Console.WriteLine("Invalid _gitCredentials.");
                    return;
                }

                UpdateMarkdownFile(filePath);
                _gitHandler.CommitAndPushChanges(_gitCredentials);
            }

        static void UpdateMarkdownFile(string filePath)
        {
            string content = File.Exists(filePath) ? File.ReadAllText(filePath) : "";
            string newTimestamp = $"_Last updated on: **{DateTime.UtcNow} UTC**_";

            if (Regex.IsMatch(content, @"_Last updated on: \*\*.*?\*\*_"))
            {
                content = Regex.Replace(content, @"_Last updated on: \*\*.*?\*\*_", newTimestamp);
            }
            else
            {
                content += $"\n{newTimestamp}\n";
            }

            File.WriteAllText(filePath, content);
            Console.WriteLine("README updated.");
        }

    }
}
