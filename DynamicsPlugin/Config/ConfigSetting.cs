using System.Xml.Serialization;
using Newtonsoft.Json;

namespace DynamicsPlugin.Common
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    [JsonObject(MemberSerialization.OptIn)]
    public class ConfigSetting
    {
        [XmlElement("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [XmlElement("value")]
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}