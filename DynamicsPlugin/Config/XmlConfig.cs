using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace DynamicsPlugin.Common
{
    [XmlRoot("config", Namespace = "", IsNullable = false)]
    public partial class XmlConfig : IEnumerable<ConfigSetting>, IPluginConfig
    {
        public XmlConfig()
        {
            Settings = new List<ConfigSetting>();
        }

        [XmlElement("setting", typeof(ConfigSetting))]
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

        /// <summary>
        /// ConvertStringToBytes: Convert a string to a byte array
        /// </summary>
        /// <param name="text">The text to be converted</param>
        /// <returns></returns>
        private static byte[] ConvertStringToBytes(string text)
        {
            var ms = new MemoryStream();
            using (var writer = new StreamWriter(ms) { AutoFlush = true })
            {
                writer.Write(text);
            }
            return ms.ToArray();
        }

        public static T Deserialize<T>(string xml)
        {
            var byteArray = ConvertStringToBytes(xml);
            var xs = new XmlSerializer(typeof(T));
            var ms = new MemoryStream(byteArray);
            return (T)xs.Deserialize(ms);
        }
    }
}