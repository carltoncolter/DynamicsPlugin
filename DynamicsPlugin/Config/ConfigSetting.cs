using System.Xml.Serialization;
using Newtonsoft.Json;

namespace DynamicsPlugin.Common
{
    /// <summary>
    /// A configuration setting.
    /// </summary>
    [XmlType(TypeName = "setting")]
    [XmlRoot(Namespace = "", IsNullable = false)]
    [JsonObject(MemberSerialization.OptIn)]
    public class ConfigSetting
    {
        /// <summary>
        /// The name of the configuration setting.
        /// </summary>
        [XmlAttribute("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The value of the configuration setting.
        /// </summary>
        [XmlText]
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}