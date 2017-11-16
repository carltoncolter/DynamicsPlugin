using System.Collections.Generic;

namespace DynamicsPlugin.Common
{
    public interface IPluginConfig
    {
        /// <summary>
        /// Gets or sets the value at the specified index name.
        /// </summary>
        /// <param name="name">The config setting name.</param>
        /// <returns>A string containing the config setting value.</returns>
        string this[string name] { get; set; }

        /// <summary>
        /// The collection of configuration settings, List{ConfigSetting}
        /// </summary>
        List<ConfigSetting> Settings { get; set; }

        /// <summary>
        /// Returns an enumerator that iterates through the collection of ConfigSetting(s).
        /// </summary>
        /// <returns>
        /// A ConfigSetting collection enumerator.
        /// </returns>
        IEnumerator<ConfigSetting> GetEnumerator();

        
    }
}