using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GitComitter.Root.Config;
using GitComitter.Root.Git;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public async Task<int> UpdateAndCommitReadmeFile(string filePath)
        {

                
                if (_gitCredentials == null || string.IsNullOrEmpty(_gitCredentials.Username) || string.IsNullOrEmpty(_gitCredentials.Github_token))
                {
                    Console.WriteLine("Invalid _gitCredentials.");
                    return 0;
                }

                await UpdateMarkdownFile(filePath);
                await _gitHandler.CommitAndPushChanges(_gitCredentials);
                return 1;
            }

        async Task<int> UpdateMarkdownFile(string filePath)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"token {this._gitCredentials.Github_token}");
            client.DefaultRequestHeaders.Add("User-Agent", "C# GitHub API");

            try
            {
                // Step 1: Fetch the current README.md content and SHA
                string fileUrl = $"https://api.github.com/repos/{_gitCredentials.Owner}/{_gitCredentials.Repo}/contents/{_gitCredentials.File_path}";
                var fileResponse = await client.GetAsync(fileUrl);
                if (!fileResponse.IsSuccessStatusCode)
                {
                    string errorContent = await fileResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ GitHub API error: {fileResponse.StatusCode} - {errorContent}");
                    return 0;
                }
                // Deserialize the JSON response
                string fileJson = await fileResponse.Content.ReadAsStringAsync();
                JObject fileData = JObject.Parse(fileJson);
                string oldContentBase64 = fileData["content"]?.ToString();
                string fileSha = fileData["sha"]?.ToString();

                // Decode the Base64 file content
                string oldContent = Encoding.UTF8.GetString(Convert.FromBase64String(oldContentBase64.Replace("\n", "")));

                // Step 2: Update "Last updated at" timestamp
                string newTimestamp = $"Last updated at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
                string updatedContent = Regex.Replace(oldContent, @"Last updated at: .* UTC", newTimestamp);

                // If "Last updated at" doesn't exist, append it at the end
                if (!Regex.IsMatch(oldContent, @"Last updated at: .* UTC"))
                {
                    updatedContent += $"\n\n{newTimestamp}";
                }

                // Step 3: Commit the updated README.md
                var updateData = new JObject
            {
                { "message", "Updated README.md with latest timestamp" },
                { "content", Convert.ToBase64String(Encoding.UTF8.GetBytes(updatedContent)) },
                { "sha", fileSha },
                { "branch", this._gitCredentials.Branch }
            };

                var updateResponse = await client.PutAsync(
                    fileUrl,
                    new StringContent(updateData.ToString(), Encoding.UTF8, "application/json")
                );

                if (updateResponse.IsSuccessStatusCode)
                    Console.WriteLine("✅ README.md updated successfully!");
                else
                    Console.WriteLine($"❌ Failed to update README.md: {await updateResponse.Content.ReadAsStringAsync()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Exception occurred: {ex.Message}");
                client.Dispose();
                return 0;
            }
            client.Dispose();
            return 1;
        }

    }
}
