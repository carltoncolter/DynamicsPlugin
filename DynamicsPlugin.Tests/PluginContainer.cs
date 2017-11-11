using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using DynamicsPlugin.Common.Attributes;
using Microsoft.Xrm.Sdk;

namespace DynamicsPlugin.Tests
{
    /// <summary>
    /// A Plugin Container for <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">IPlugin: The plugin to run inside the container</typeparam>
    internal class PluginContainer<T> : MarshalByRefObject, IDisposable, IPlugin where T : IPlugin
    {
        private const string DomainSuffix = "Sandbox";

        /// <inheritdoc />
        public PluginContainer() : this(null, null)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Create a container to run a plugin.
        /// Auto-detects sandbox mode plugins and tests them using Sandbox mode if the IsolationMode is Sandbox
        /// </summary>
        /// <param name="unsecureConfig">Unsecure configuration string</param>
        /// <param name="secureConfig">Secure configuration string</param>
        public PluginContainer(string unsecureConfig, string secureConfig)
        {
            var sandbox = AttributeExtensions.GetCrmPluginRegistrationAttributes(typeof(T))
                .Any(a => a.IsolationMode == IsolationModeEnum.Sandbox);
            SetupContainer(sandbox, unsecureConfig, secureConfig);
        }

        /// <inheritdoc />
        public PluginContainer(bool isSandboxed): this(isSandboxed, null, null)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Create a container to run a plugin
        /// </summary>
        /// <param name="isSandboxed">True for the container to run in sandbox mode, false to run fully trusted</param>
        /// <param name="unsecureConfig">Unsecure configuration string</param>
        /// <param name="secureConfig">Secure configuration string</param>
        public PluginContainer(bool isSandboxed, string unsecureConfig, string secureConfig)
        {
            SetupContainer(isSandboxed, unsecureConfig, secureConfig);
        }

        private void SetupContainer(bool isSandboxed, string unsecureConfig, string secureConfig)
        {
            if (isSandboxed)
            {
                /*
                 * Sandboxed plug-ins and custom workflow activities can access the network through the HTTP and HTTPS protocols. This capability provides 
                   support for accessing popular web resources like social sites, news feeds, web services, and more. The following web access restrictions
                   apply to this sandbox capability.

                    * Only the HTTP and HTTPS protocols are allowed.
                    * Access to localhost (loopback) is not permitted.
                    * IP addresses cannot be used. You must use a named web address that requires DNS name resolution.
                    * Anonymous authentication is supported and recommended. There is no provision for prompting the 
                      on user for credentials or saving those credentials.
                 */

                var type = typeof(T);
                var source = type.Assembly.Location;
                var sourceAssembly = System.Reflection.Assembly.UnsafeLoadFrom(source);

                var setup = new AppDomainSetup
                {
                    ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                    ApplicationName = $"{sourceAssembly.GetName().Name}{DomainSuffix}",
                    DisallowBindingRedirects = true,
                    DisallowCodeDownload = true,
                    DisallowPublisherPolicy = true
                };

                var ps = new PermissionSet(PermissionState.None);
                ps.AddPermission(new SecurityPermission(SecurityPermissionFlag.SerializationFormatter));
                ps.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
                ps.AddPermission(new FileIOPermission(PermissionState.None));
                ps.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess));

                //RegEx pattern taken from: https://msdn.microsoft.com/en-us/library/gg334752.aspx
                ps.AddPermission(new WebPermission(NetworkAccess.Connect,
                    new Regex(
                        @"^http[s]?://(?!((localhost[:/])|(\[.*\])|([0-9]+[:/])|(0x[0-9a-f]+[:/])|(((([0-9]+)|(0x[0-9A-F]+))\.){3}(([0-9]+)|(0x[0-9A-F]+))[:/]))).+")));

                // We don't need to add these, but it is important to note that there is no access to the following
                ps.AddPermission(new NetworkInformationPermission(NetworkInformationAccess.None));
                ps.AddPermission(new EnvironmentPermission(PermissionState.None));
                ps.AddPermission(new RegistryPermission(PermissionState.None));
                ps.AddPermission(new EventLogPermission(PermissionState.None));


                PluginAppDomain = AppDomain.CreateDomain(DomainSuffix, null, setup, ps, null);

                Instance = Create(unsecureConfig, secureConfig);
            }
            else
            {
                // Just create a non-sandboxed plugin
                Instance = (T) Activator.CreateInstance(typeof(T), unsecureConfig, secureConfig);
            }
        }

        /// <summary>
        /// The Sandbox AppDomain to execute the plugin
        /// </summary>
        public AppDomain PluginAppDomain { get; private set; }

        /// <summary>
        /// The instance of the plugin
        /// </summary>
        public T Instance { get; private set; }

        private T Create(string unsecureConfig, string secureConfig)
        {
            var type = typeof(T);
            if (typeof(T) != type) throw new ArgumentException("Generic T does not match initialized type.");

            return (T) Activator.CreateInstanceFrom(
                    PluginAppDomain, 
                    type.Assembly.ManifestModule.FullyQualifiedName,
                    // ReSharper disable once AssignNullToNotNullAttribute
                    type.FullName, false, BindingFlags.CreateInstance, 
                    null, new object[] {unsecureConfig, secureConfig},
                    CultureInfo.CurrentCulture, null
                ).Unwrap();
        }

        #region IDisposable Support
        //Implementing IDisposable Pattern: https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/dispose-pattern
        private bool _disposed; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (PluginAppDomain != null)
                {
                    AppDomain.Unload(PluginAppDomain);
                    PluginAppDomain = null;
                }
            }

            _disposed = true;
        }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

        #region IPlugin Supporrt
        void IPlugin.Execute(IServiceProvider serviceProvider)
        {
            Instance.Execute(serviceProvider);
        }
        #endregion
    }
}