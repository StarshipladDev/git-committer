using GitComitter.Root.Config;
using System;
using System.Diagnostics;

namespace GitComitter.Root.Git
{
    public class GitHandler : IGitHandler
    {
        public void CommitAndPushChanges(cfg_GitCredentials credentials)
        {
            RunGitCommand("add .");
            RunGitCommand("commit -m \"Updated markdown file\"");

            string remoteUrl = GetRemoteUrl();
            if (string.IsNullOrEmpty(remoteUrl))
            {
                Console.WriteLine("Failed to get remote repository URL.");
                return;
            }

            string authUrl = remoteUrl.Replace("https://", $"https://{credentials.Username}:{credentials.Token}@");
            RunGitCommand($"push {authUrl} main");
        }

        public string GetRemoteUrl()
        {
            return RunGitCommand("config --get remote.origin.url");
        }

        public string RunGitCommand(string command)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = command,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrWhiteSpace(result))
            {
                Console.WriteLine(result);
            }

            return result.Trim();
        }
    }
}
