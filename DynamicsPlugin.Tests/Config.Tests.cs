using System;
using DynamicsPlugin.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace DynamicsPlugin.Tests
{
    [TestClass]
    public class ConfigTests
    {
        #region Base Configuration Tests

        #region Xml Test
        [TestMethod]
        public void DeserializeXml_CreatesValidConfigObject()
        {
            #region arrange - given

            var xml = "<config>" +
                "<setting name=\"Item1\">Value1</setting>" +
                "<setting name=\"Item2\">Value2</setting>" +
                "</config>";

            var settingsShouldMatchThis = new[] { new[] { "Item1", "Value1" }, new[] { "Item2", "Value2" } };

            #endregion

            #region act - when

            var config = XmlConfig.Deserialize<XmlConfig>(xml);

            #endregion

            #region assert - then

            AssertConfigurationIsValid(config, settingsShouldMatchThis,
                "Xml configuration did not parse settings correctly.");

            #endregion
        }

        [TestMethod]
        public void DeserializeSerializedXmlConfig_CreatesValidConfigObject()
        {
            #region arrange - given

            var xml = SerializeXmlConfig();
            var settingsShouldMatchThis = new[] { new[] { "Item1", "Value1" }, new[] { "Item2", "Value2" } };

            #endregion

            #region act - when

            var config = XmlConfig.Deserialize<XmlConfig>(xml);

            #endregion

            #region assert - then

            AssertConfigurationIsValid(config, settingsShouldMatchThis,
                "Xml configuration did not parse settings correctly.");

            #endregion
        }

        #endregion

        #region Json Test
        [TestMethod]
        public void DeserializeDirtyJson_CleansAndCreatesValidConfigObject()
        {
            #region arrange - given

            var json = @"{settings  : [
                            {name:'Item1',value:'Value1'},
                            {name:'Item2',value:'Value2'}
                         ]}";
            var settingsShouldMatchThis = new[] {new[] {"Item1", "Value1"}, new[] {"Item2", "Value2"}};

            #endregion

            #region act - when

            var config = JsonConfig.Deserialize<JsonConfig>(json);

            #endregion

            #region assert - then

            AssertConfigurationIsValid(config, settingsShouldMatchThis,
                "Json configuration did not parse settings correctly.");

            #endregion
        }

        [TestMethod]
        public void DeserializeSerializedJsonConfig_CreatesValidConfigObject()
        {
            #region arrange - given

            var json = SerializeJsonConfig();
            var settingsShouldMatchThis = new[] {new[] {"Item1", "Value1"}, new[] {"Item2", "Value2"}};

            #endregion

            #region act - when

            var config = JsonConfig.Deserialize<JsonConfig>(json);

            #endregion

            #region assert - then

            AssertConfigurationIsValid(config, settingsShouldMatchThis,
                "Json configuration did not parse settings correctly.");

            #endregion
        }
        
        #endregion

        #endregion

        #region Test Setup/Configuration Methods

        public TestContext TestContext { get; set; }

        #region Helper Methods

        private void Trace(string message)
        {
            TestContext.WriteLine(message);
            Console.WriteLine(message);
            System.Diagnostics.Trace.WriteLine(message);
        }

        private static string SerializeJsonConfig()
        {
            var jsonConfig = new JsonConfig();
            jsonConfig.Settings.Add(new ConfigSetting { Name = "Item1", Value = "Value1" });
            jsonConfig.Settings.Add(new ConfigSetting { Name = "Item2", Value = "Value2" });
            return JsonConfig.Serialize(jsonConfig);
        }

        private static string SerializeXmlConfig()
        {
            var xmlConfig = new XmlConfig();
            xmlConfig.Settings.Add(new ConfigSetting { Name = "Item1", Value = "Value1" });
            xmlConfig.Settings.Add(new ConfigSetting { Name = "Item2", Value = "Value2" });
            return XmlConfig.Serialize(xmlConfig);
        }

        private static void AssertConfigurationIsValid(IPluginConfig config, string[][] configSettings,
            string errorMessage)
        {
            Assert.AreEqual(configSettings.Length, config.Settings.Count,
                $"{errorMessage} Incorrect number of settings returned.");
            Assert.AreEqual(configSettings[0][0], config.Settings[0].Name, errorMessage);
            Assert.AreEqual(configSettings[0][1], config.Settings[0].Value, errorMessage);
            Assert.AreEqual(configSettings[1][0], config.Settings[1].Name, errorMessage);
            Assert.AreEqual(configSettings[1][1], config.Settings[1].Value, errorMessage);
        }

        #endregion

        #endregion
    }
}