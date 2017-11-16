using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using DynamicsPlugin.Common.Constants;

namespace DynamicsPlugin.Common
{
    [XmlRoot("config", Namespace = "", IsNullable = false)]
    public class XmlConfig : IEnumerable<ConfigSetting>, IPluginConfig
    {
        /// <summary>
        /// Default XmlConfig Constructor
        /// </summary>
        public XmlConfig()
        {
            Settings = new List<ConfigSetting>();
        }

        /// <inheritdoc cref="IPluginConfig.GetEnumerator" />
        public IEnumerator<ConfigSetting> GetEnumerator() => Settings.GetEnumerator();

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        [XmlElement("settings")]
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
        /// Adds a ConfigSetting to the end of the Settings collection.
        /// </summary>
        /// <param name="item">The ConfigSetting to add to the collection of settings.</param>
        /// <remarks>Null items are ignored and will not be added.</remarks>
        public void Add(ConfigSetting item)
        {
            if (item!=null) Settings.Add(item);
        }

        /// <summary>
        ///     Convert a string to a byte array
        /// </summary>
        /// <param name="text">The text to be converted</param>
        /// <returns></returns>
        private static byte[] ConvertStringToBytes(string text)
        {
            var ms = new MemoryStream();
            using (var writer = new StreamWriter(ms) {AutoFlush = true})
            {
                writer.Write(text);
            }
            return ms.ToArray();
        }

        /// <summary>
        /// Deserialize an xml string into a T:XmlConfig object.
        /// </summary>
        /// <typeparam name="T">XmlConfig</typeparam>
        /// <param name="xml">The xml config string to be converted into a T:XmlConfig object.</param>
        /// <returns>The T:XmlConfig object.</returns>
        public static T Deserialize<T>(string xml) where T : XmlConfig
        {
            try
            {
                var byteArray = ConvertStringToBytes(xml);
                var xs = new XmlSerializer(typeof(T));
                var ms = new MemoryStream(byteArray);
                return (T) xs.Deserialize(ms);
            }
            catch (Exception ex)
            {
                throw new SerializationException(ConfigMessages.DeserializationError, ex);
            }
        }

        /// <summary>
        /// Serializes the specified T:XmlConfig object and writes the XML a string.
        /// </summary>
        /// <typeparam name="T">XmlConfig</typeparam>
        /// <param name="obj">The configuration to serialize.</param>
        /// <returns>An xml string containing the serialized configuration.</returns>
        public static string Serialize<T>(T obj) where T : XmlConfig
        {
            string response;
            var xs = new XmlSerializer(typeof(T));
            using (var ms = new MemoryStream())
            {
                xs.Serialize(ms, obj);
                ms.Position = 0;
                var sr = new StreamReader(ms);
                response = sr.ReadToEnd();
            }
            return response;
        }
    }
}