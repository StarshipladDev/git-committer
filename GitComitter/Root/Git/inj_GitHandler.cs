using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GitComitter.Root.Config;
using Ninject;

namespace GitComitter.Root.Git
{
    internal class inj_GitHandler : NinjectModule
    {
        private readonly cfg_GitCredentials _credentials;

        public inj_GitHandler(cfg_GitCredentials credentials)
        {
            _credentials = credentials;
        }

        public override void Load()
        {
            Bind<IGitHandler>().To<GitHandler>().WithConstructorArgument("credentials", _credentials);
        }
    }
}
