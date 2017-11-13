using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicsPlugin.Common;

namespace DynamicsPlugin.Common.Attributes
{
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
