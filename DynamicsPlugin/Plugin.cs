using System;
using System.IO;
using DynamicsPlugin.Common;
using DynamicsPlugin.Common.Attributes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace DynamicsPlugin
{
    [CrmPluginConfiguration(ConfigType = ConfigType.Xml)]
    [CrmPluginRegistration(MessageNameEnum.Create,"account", StageEnum.PreOperation, ExecutionModeEnum.Synchronous,
        "", "PreCreate Account", 1, IsolationModeEnum.Sandbox)]
    [CrmPluginRegistration(MessageNameEnum.Update, "account", StageEnum.PreOperation, ExecutionModeEnum.Synchronous,
        "", "PreUpdate Account", 1, IsolationModeEnum.Sandbox, Image1Type = ImageTypeEnum.PreImage, Image1Name = "pre", Image1Attributes = "")]
    [Serializable]
    public class Plugin : PluginBase
    {
        /*
         * Override AutoLoadConfig to false if you do not want to autoload the config 
         * Config Files are built in to the base class to handle both json and xml config files.
         */
        #region constructors
        public Plugin()
        { }

        public Plugin(string unsecureString, string secureString) : base(unsecureString, secureString)
        { }
        #endregion

        public override void Execute(ILocalPluginContext context)
        {
            // Try and access the file system - this isn't allowed in sandboxed plugin
            //var sw = new StreamWriter("C:\\test.txt");
            //sw.WriteLine("ouch");

            var account = context.OrganizationService.Retrieve("account", Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new ColumnSet(true));

            var note = new Entity("annotation");
            note["description"] = "test";
            context.OrganizationService.Create(note);

        }
    }
}
