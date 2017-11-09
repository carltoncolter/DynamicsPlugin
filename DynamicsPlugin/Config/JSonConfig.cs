using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;

namespace DynamicsPlugin.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public partial class JsonConfig : IEnumerable<ConfigSetting>, IPluginConfig
    {
        public JsonConfig()
        {
            Settings = new List<ConfigSetting>();
        }

        [JsonProperty("settings")]
        public List<ConfigSetting> Settings { get; set; }

        public ConfigSetting this[string name]
        {
            get
            {
                return Settings.FirstOrDefault(s => s.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            }
            set
            {
                var current =
                    Settings.FirstOrDefault(s => s.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                if (!value.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) value.Name = name;
                if (current == null)
                {
                    Settings.Add(value);
                    return;
                }
                Settings.Remove(current);
                Settings.Add(value);
            }
        }

        public IEnumerator<ConfigSetting> GetEnumerator()
        {
            return Settings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            });
        }
    }
}