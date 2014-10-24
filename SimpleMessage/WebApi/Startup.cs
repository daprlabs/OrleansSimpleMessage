// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Defines the Startup type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using SimpleMessage.Web;

[assembly: Microsoft.Owin.OwinStartup(typeof(Startup))]

namespace SimpleMessage.Web
{
    using System;
    using System.IO;
    using System.Reflection;

    using Orleans.Host;

    using Owin;

    /// <summary>
    /// Initialization routines.
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// The name of the client configuration file.
        /// </summary>
        private const string ClientConfigurationFileName = "ClientConfiguration.xml";

        /// <summary>
        /// Configure the application.
        /// </summary>
        /// <param name="app">The app being configured.</param>
        public void Configuration(IAppBuilder app)
        {
            // Get the orleans config file.
            var clientConfiguration = Path.Combine(GetAssemblyPath(), ClientConfigurationFileName);
            
            // Initialize Orleans.
            OrleansAzureClient.Initialize(clientConfiguration);

            // Initialize Web API and static file server.
            ConfigureWebApi(app);
            ConfigureFileServer(app);
        }
        
        /// <summary>
        /// Returns the path of the executing assembly.
        /// </summary>
        /// <returns>The path of the executing assembly.</returns>
        private static string GetAssemblyPath()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var localPath = new Uri(codeBase).LocalPath;
            return Path.GetDirectoryName(localPath);
        }
    }
}