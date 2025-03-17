using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitComitter.Root.Config
{
    public class cfg_GitCredentials
    {
        public string Username { get; set; }
        public string Github_token { get; set; }
        public string Owner { get; set; }
        public string Repo { get; set; }
        public string Branch { get; set; }
        public string File_path { get; set; }
    }
}
