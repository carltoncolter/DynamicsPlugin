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

        public ConfigType ConfigType { get; set; }
        public bool AutoLoad { get; set; }
        public bool ForceErrorWhenComplete { get; set; }
    }
}