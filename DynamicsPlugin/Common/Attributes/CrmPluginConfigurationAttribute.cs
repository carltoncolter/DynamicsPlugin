using System;
using System.Diagnostics.CodeAnalysis;

namespace DynamicsPlugin.Common.Attributes
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CrmPluginConfigurationAttribute : Attribute
    {
        public CrmPluginConfigurationAttribute()
        {
            ConfigType = ConfigType.Json;
            AutoLoad = true;
        }

        /// <summary>
        ///     The type of configuration: none, string, json, xml
        /// </summary>
        public ConfigType ConfigType { get; set; }

        /// <summary>
        ///     Automatically load the configuration. Default is <c>true</c>.
        /// </summary>
        /// <value>
        ///     The <c>AutoLoad</c> property defaults to <c>true</c>, and it controls whether or not an <c>XmlConfig</c> or
        ///     <c>JsonConfig</c>c> is automatically parsed at load time.
        /// </value>
        public bool AutoLoad { get; set; }

        /// <summary>
        ///     Force an error when the plugin has completed. Default is <c>false</c>.
        /// </summary>
        /// <remarks>
        ///     <c>ForceErrorWhenComplete</c> is useful when testing a plugin in a live Dynamics environment.  Doing so will force
        ///     the tracelog to be written and allow you to see debugging information easily.  It is a quick and easy way to test
        ///     something or to see why something is working, but not the way that was intended.
        /// </remarks>
        public bool ForceErrorWhenComplete { get; set; }
    }
}