using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using DynamicsPlugin.Common.Constants;
using Newtonsoft.Json;

namespace DynamicsPlugin.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonConfig : IEnumerable<ConfigSetting>, IPluginConfig
    {
        /// <summary>
        /// Default JsonConfig Constructor
        /// </summary>
        public JsonConfig()
        {
            Settings = new List<ConfigSetting>();
        }

        /// <inheritdoc cref="IPluginConfig.GetEnumerator" />
        public IEnumerator<ConfigSetting> GetEnumerator() => Settings.GetEnumerator();

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        [JsonProperty("settings")]
        public List<ConfigSetting> Settings { get; set; }

        /// <inheritdoc />
        public string this[string name]
        {
            get
            {
                return Settings.FirstOrDefault(s => s.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    ?.Value;
            }
            set
            {
                var current =
                    Settings.FirstOrDefault(s => s.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                if (current != null) Settings.Remove(current);

                Settings.Add(new ConfigSetting {Name = name, Value = value});
            }
        }

        /// <summary>
        ///     Deserialize a json string into a T:JsonConfig object.
        /// </summary>
        /// <typeparam name="T">JsonConfig</typeparam>
        /// <param name="json">The json config string to be converted into a T:JsonConfig object.</param>
        /// <returns>The T:JsonConfig object.</returns>
        /// <remarks>The json can be loosely formatted, meaning property names do not need to be in quotes.</remarks>
        public static T Deserialize<T>(string json) where T : JsonConfig
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(CleanJson(json), new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });
            }
            catch (Exception ex)
            {
                throw new SerializationException(ConfigMessages.DeserializationError, ex);
            }
        }

        /// <summary>
        ///     Serializes the specified T:JsonConfig object and writes the json a string.
        /// </summary>
        /// <typeparam name="T">JsonConfig</typeparam>
        /// <param name="obj">The configuration to serialize.</param>
        /// <returns>A json string containing the serialized configuration.</returns>
        public static string Serialize<T>(T obj) => JsonConvert.SerializeObject(obj);

        /// <summary>
        ///     Cleans a loosely formatted json string by adding quotes around property names and removing white space that could
        ///     cause the deserialization of the json string not to work properly.
        /// </summary>
        /// <param name="json">The dirty json string to clean.</param>
        /// <returns>A clean json string.</returns>
        private static string CleanJson(string json)
        {
            var pattern = new Regex(@"({|,)(?:\s*)(?:')?([A-Za-z_$\.][A-Za-z0-9_\-\.$]*)(?:')?(?:\s*)(:\s*)",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

            json = pattern.Replace(json, "$1\"$2\":");
            return json;
        }
    }
}