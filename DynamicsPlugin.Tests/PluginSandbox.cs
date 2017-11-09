using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using DynamicsPlugin.Common;

namespace DynamicsPlugin.Tests
{
    internal class PluginSandbox<T> : MarshalByRefObject, IDisposable where T : PluginBase
    {
        private const string DomainSuffix = "Sandbox";

        public PluginSandbox()
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
            //https://msdn.microsoft.com/en-us/library/gg334752.aspx
            ps.AddPermission(new WebPermission(NetworkAccess.Connect,
                new Regex(
                    @"^http[s]?://(?!((localhost[:/])|(\[.*\])|([0-9]+[:/])|(0x[0-9a-f]+[:/])|(((([0-9]+)|(0x[0-9A-F]+))\.){3}(([0-9]+)|(0x[0-9A-F]+))[:/]))).+")));
            // We don't need to add these, but it is important to note that there is no access to the following
            ps.AddPermission(new NetworkInformationPermission(NetworkInformationAccess.None));
            ps.AddPermission(new EnvironmentPermission(PermissionState.None));
            ps.AddPermission(new RegistryPermission(PermissionState.None));
            ps.AddPermission(new EventLogPermission(PermissionState.None));


            PluginAppDomain = AppDomain.CreateDomain(DomainSuffix, null, setup, ps, null);
        }

        public AppDomain PluginAppDomain { get; private set; }


        public void Dispose()
        {
            AppDomain.Unload(PluginAppDomain);
            PluginAppDomain = null;
        }

        public T Create(string unsecureConfig, string secureConfig)
        {
            var type = typeof(T);
            if (typeof(T) != type) throw new ArgumentException("Generic T does not match initialized type.");
            return (T) Activator.CreateInstanceFrom(PluginAppDomain, type.Assembly.ManifestModule.FullyQualifiedName,
                type.FullName, false, BindingFlags.CreateInstance, null, new[] {unsecureConfig, secureConfig},
                CultureInfo.CurrentCulture, null).Unwrap();
        }
    }
}