﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
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

        public static T Deserialize<T>(string json) where T:JsonConfig
        {
            return JsonConvert.DeserializeObject<T>(CleanJson(json), new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore, 
            });
        }

        public static string Serialize<T>(T obj) => JsonConvert.SerializeObject(obj);

        private static string CleanJson(string json)
        {
            var pattern = new Regex(@"({|,)(?:\s*)(?:')?([A-Za-z_$\.][A-Za-z0-9_\-\.$]*)(?:')?(?:\s*)(:\s*)",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

            json = pattern.Replace(json, "$1\"$2\":");
            return json;
        }
    }
}