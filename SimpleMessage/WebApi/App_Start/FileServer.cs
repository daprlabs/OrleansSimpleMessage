// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Initialization routines.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SimpleMessage.Web
{
    using System.Collections.Generic;

    using Microsoft.Owin;
    using Microsoft.Owin.Extensions;
    using Microsoft.Owin.FileSystems;
    using Microsoft.Owin.StaticFiles;

    using Owin;

    /// <summary>
    /// Initialization routines.
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// Configures the file server.
        /// </summary>
        /// <param name="app">The app being configured.</param>
        public static void ConfigureFileServer(IAppBuilder app)
        {
            // Serve static files only from the "WebSite" subdirectory.
            new List<FileServerOptions>
            {
                new FileServerOptions
                {
                    RequestPath = new PathString(""),
                    FileSystem = new PhysicalFileSystem(@"WebSite"),
                    EnableDefaultFiles = true,
                    EnableDirectoryBrowsing = false,
                },
                new FileServerOptions
                {
                    RequestPath = new PathString("/js"),
                    FileSystem = new PhysicalFileSystem(@"Scripts"),
                    EnableDefaultFiles = true,
                    EnableDirectoryBrowsing = false,
                },
                new FileServerOptions
                {
                    RequestPath = new PathString("/css"),
                    FileSystem = new PhysicalFileSystem(@"Content"),
                    EnableDefaultFiles = true,
                    EnableDirectoryBrowsing = false,
                },
                new FileServerOptions
                {
                    RequestPath = new PathString("/fonts"),
                    FileSystem = new PhysicalFileSystem(@"fonts"),
                    EnableDefaultFiles = true,
                    EnableDirectoryBrowsing = false,
                }
            }.ForEach(fs => app.UseFileServer(fs));
            app.UseStageMarker(PipelineStage.MapHandler);
        }
    }
}