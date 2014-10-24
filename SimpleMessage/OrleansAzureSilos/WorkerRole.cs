// ********************************************************* //
//    Copyright (c) Microsoft. All rights reserved.
//    
//    Apache 2.0 License
//    
//    You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//    
//    Unless required by applicable law or agreed to in writing, software 
//    distributed under the License is distributed on an "AS IS" BASIS, 
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or 
//    implied. See the License for the specific language governing 
//    permissions and limitations under the License.
//
// *********************************************************

namespace Orleans.Azure.Silos
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Reflection;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Diagnostics;
    using Microsoft.WindowsAzure.ServiceRuntime;

    using Orleans.Host;
    using Orleans.Runtime.Configuration;
    
    /// <summary>
    /// The worker role.
    /// </summary>
    public class WorkerRole : RoleEntryPoint
    {
        /// <summary>
        /// The data connection string key.
        /// </summary>
        private const string DataConnectionStringKey = "DataConnectionString";

        /// <summary>
        /// The storage key.
        /// </summary>
        private const string StorageKey = "Storage";

        /// <summary>
        /// The orleans azure silo.
        /// </summary>
        private OrleansAzureSilo orleansAzureSilo;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkerRole"/> class.
        /// </summary>
        public WorkerRole()
        {
            Console.WriteLine("OrleansAzureSilos-Constructor called");
        }

        /// <summary>
        /// Gets or sets a value indicating whether to collect performance counters.
        /// </summary>
        public static bool CollectPerfCounters { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to collect windows event logs.
        /// </summary>
        public static bool CollectWindowsEventLogs { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to collect full crash dumps.
        /// </summary>
        public static bool FullCrashDumps { get; set; }

        /// <summary>
        /// Configure diagnostics.
        /// </summary>
        /// <returns>
        /// The <see cref="DiagnosticMonitorConfiguration"/>.
        /// </returns>
        public static DiagnosticMonitorConfiguration ConfigureDiagnostics()
        {
            // Get default initial configuration.
            var diagConfig = DiagnosticMonitor.GetDefaultInitialConfiguration();

            // Add performance counters to the diagnostic configuration
            if (CollectPerfCounters)
            {
                diagConfig.PerformanceCounters.DataSources.Add(
                    new PerformanceCounterConfiguration
                    {
                        CounterSpecifier = @"\Processor(_Total)\% Processor Time",
                        SampleRate = TimeSpan.FromSeconds(5)
                    });
                diagConfig.PerformanceCounters.DataSources.Add(
                    new PerformanceCounterConfiguration
                    {
                        CounterSpecifier = @"\Memory\Available Mbytes",
                        SampleRate = TimeSpan.FromSeconds(5)
                    });
            }

            // Add event collection from the Windows Event Log
            if (CollectWindowsEventLogs)
            {
                diagConfig.WindowsEventLog.DataSources.Add("System!*");
                diagConfig.WindowsEventLog.DataSources.Add("Application!*");
            }

            // Schedule log transfers into storage
            diagConfig.DiagnosticInfrastructureLogs.ScheduledTransferLogLevelFilter = LogLevel.Information;
            diagConfig.DiagnosticInfrastructureLogs.ScheduledTransferPeriod = TimeSpan.FromMinutes(5);

            // Specify whether full crash dumps should be captured 
            CrashDumps.EnableCollection(FullCrashDumps);

            return diagConfig;
        }

        /// <summary>
        /// Handle the start event.
        /// </summary>
        /// <returns>A value indicating whether or not this role started successfully.</returns>
        public override bool OnStart()
        {
            Trace.WriteLine("OrleansAzureSilos-OnStart called", "Information");

            Trace.WriteLine("OrleansAzureSilos-OnStart Initializing config", "Information");

            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            RoleEnvironment.Changing += RoleEnvironmentChanging;

            Trace.WriteLine("OrleansAzureSilos-OnStart Initializing diagnostics", "Information");

            var diagConfig = ConfigureDiagnostics();

            // Start the diagnostic monitor. 
            // The parameter references a connection string specified in the service configuration file 
            // that indicates the storage account where diagnostic information will be transferred. 
            // If the value of this setting is "UseDevelopmentStorage=true" then logs are written to development storage.
            DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", diagConfig);

            Trace.WriteLine("OrleansAzureSilos-OnStart Starting Orleans silo", "Information");

            this.orleansAzureSilo = new OrleansAzureSilo();

            var ok = base.OnStart();
            if (ok)
            {
                var cfg = new OrleansConfiguration();
                cfg.LoadFromFile("OrleansConfiguration.xml");
                ProviderCategoryConfiguration storageConfiguration;
                if (cfg.Globals.ProviderConfigurations.TryGetValue(StorageKey, out storageConfiguration))
                {
                    // Find all storage providers with service configuration key names and modify them as necessary.
                    foreach (var provider in storageConfiguration.Providers.Where(provider => provider.Value is ProviderConfiguration))
                    {
                        string connectionString;
                        var properties = provider.Value.Properties;
                        if (properties.TryGetValue(DataConnectionStringKey, out connectionString))
                        {
                            ModifyProviderProperty(
                                provider.Value as ProviderConfiguration,
                                DataConnectionStringKey,
                                GetConnectionStringFromServiceConfiguration(connectionString));
                        }
                    }
                }

                // Modify the Azure Liveness ConnectionString as necessary.
                cfg.Globals.DataConnectionString = GetConnectionStringFromServiceConfiguration(cfg.Globals.DataConnectionString);

                ok = this.orleansAzureSilo.Start(RoleEnvironment.DeploymentId, RoleEnvironment.CurrentRoleInstance, cfg);
            }

            Trace.WriteLine("OrleansAzureSilos-OnStart Orleans silo started ok=" + ok, "Information");
            return ok;
        }

        /// <summary>
        /// Run the silo.
        /// </summary>
        public override void Run()
        {
            Trace.WriteLine("OrleansAzureSilos-Run entry point called", "Information");
            this.orleansAzureSilo.Run(); // Call will block until silo is shutdown
        }

        /// <summary>
        /// Handle the stop event.
        /// </summary>
        public override void OnStop()
        {
            Trace.WriteLine("OrleansAzureSilos-OnStop called", "Information");
            this.orleansAzureSilo.Stop();
            RoleEnvironment.Changing -= RoleEnvironmentChanging;
            base.OnStop();
            Trace.WriteLine("OrleansAzureSilos-OnStop finished", "Information");
        }

        /// <summary>
        /// Sets the <paramref name="property"/> property of <paramref name="provider"/> to <paramref name="newValue"/>.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="property">The property.</param>
        /// <param name="newValue">The new value.</param>
        private static void ModifyProviderProperty(ProviderConfiguration provider, string property, string newValue)
        {
            // Note: This is obviously not a stable solution
            var privateProperties = typeof(ProviderConfiguration).GetField("_properties", BindingFlags.NonPublic | BindingFlags.Instance);
            if (privateProperties != null)
            {
                var properties = (Dictionary<string, string>)privateProperties.GetValue(provider);
                properties[property] = newValue;
            }
        }

        /// <summary>
        /// Returns the connection string stored in service configuration given a connection string.
        /// </summary>
        /// <param name="originalConnectionString">
        /// The original connection string.
        /// </param>
        /// <returns>
        /// The connection string from the service configuration setting indicated by the "ServiceConfigurationSetting"
        /// parameter of <paramref name="originalConnectionString"/>, or <paramref name="originalConnectionString"/> if
        /// the parameter is unspecified.
        /// </returns>
        /// <remarks>
        /// Returns <see langword="null"/> if the specified parameter does not indicate a valid setting.
        /// </remarks>
        private static string GetConnectionStringFromServiceConfiguration(string originalConnectionString)
        {
            // Default string is returned if setting is invalid.
            var result = default(string);
            var settings = originalConnectionString.Split(';')
                .Select(kvp => kvp.Split('='))
                .Where(kvp => kvp.Length == 2)
                .ToDictionary(kvp => kvp[0], kvp => kvp[1]);
            string settingName;
            if (settings.TryGetValue("ServiceConfigurationSetting", out settingName))
            {
                var connectionString = CloudConfigurationManager.GetSetting(settingName);
                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    result = connectionString;
                }
            }
            else
            {
                // Setting was not provided.
                result = originalConnectionString;
            }

            return result;
        }

        /// <summary>
        /// Handle the RoleEnvironmentChanging event.
        /// </summary>
        /// <param name="sender">
        /// The event sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private static void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            var i = 1;
            foreach (var c in e.Changes)
            {
                Trace.WriteLine(string.Format("RoleEnvironmentChanging: #{0} Type={1} Change={2}", i++, c.GetType().FullName, c));
            }

            // If a configuration setting is changing);
            if (e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange))
            {
                // Set e.Cancel to true to restart this role instance
                e.Cancel = true;
            }
        }
    }
}
