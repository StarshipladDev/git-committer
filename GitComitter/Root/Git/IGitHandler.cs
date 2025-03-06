using GitComitter.Root.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GitComitter.Root.Git
{
    public interface IGitHandler
    {
        void CommitAndPushChanges(cfg_GitCredentials credentials);
        string GetRemoteUrl();
        string RunGitCommand(string command);
    }
}
