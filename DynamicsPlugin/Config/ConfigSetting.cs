using System.Xml.Serialization;
using Newtonsoft.Json;

namespace DynamicsPlugin.Common
{
    [XmlType(TypeName = "setting")]
    [XmlRoot(Namespace = "", IsNullable = false)]
    [JsonObject(MemberSerialization.OptIn)]
    public class ConfigSetting
    {
        [XmlAttribute("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [XmlText]
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}