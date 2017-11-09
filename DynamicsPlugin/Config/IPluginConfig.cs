using System.Collections.Generic;

namespace DynamicsPlugin.Common
{
    public interface IPluginConfig
    {
        ConfigSetting this[string name] { get; set; }

        List<ConfigSetting> Settings { get; set; }

        IEnumerator<ConfigSetting> GetEnumerator();
    }
}