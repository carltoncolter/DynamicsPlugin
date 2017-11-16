using System;
using System.Diagnostics.CodeAnalysis;
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
         * 
         * Constructors should be left empty. Any initialization code should be put into InitializePlugin
         */
        
        #region constructors

        [ExcludeFromCodeCoverage]
        public Plugin() { }

        [ExcludeFromCodeCoverage]
        public Plugin(string unsecureString, string secureString) : base(unsecureString, secureString) { }
        
        #endregion

        /// <inheritdoc />
        /// <remarks>
        /// The <c>InitializePlugin</c> method is executed whenever a new plugin object is created.
        /// </remarks>
        public override void InitializePlugin()
        {
            //TODO: Add initialization work here or remove method
        }

        public override void Execute(ILocalPluginContext context)
        {
            //TODO: Write plug-in code here.

            //TODO: Remove sample code.
            #region This is sample code that should be removed.
            // Try and access the file system - this isn't allowed in sandboxed plugin
            //var sw = new StreamWriter("C:\\test.txt");
            //sw.WriteLine("ouch");

            context.OrganizationService.Retrieve("account", Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new ColumnSet(true));

            var note = new Entity("annotation") {["description"] = "test"};
            context.OrganizationService.Create(note);
            #endregion
        }
    }
}
