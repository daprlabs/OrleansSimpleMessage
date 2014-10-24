// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Initialzation routines.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SimpleMessage.Web
{
    using System.Linq;
    using System.Web.Http;

    using Newtonsoft.Json.Serialization;

    using Owin;

    /// <summary>
    /// Initialization routines.
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// Configures Web APIs.
        /// </summary>
        /// <param name="app">
        /// The app being configured.
        /// </param>
        /// <param name="config">
        /// The configuration, or <see langword="null"/> to use the default.
        /// </param>
        /// <returns>
        /// The <see cref="HttpConfiguration"/>.
        /// </returns>
        public static HttpConfiguration ConfigureWebApi(IAppBuilder app, HttpConfiguration config = null)
        {
            config = config ?? new HttpConfiguration();
            
            // Web API configuration and services
            // Use camel case for JSON data.
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Use JSON instead of XML.
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            app.UseWebApi(config);
            config.EnsureInitialized();
            return config;
        }
    }
}
