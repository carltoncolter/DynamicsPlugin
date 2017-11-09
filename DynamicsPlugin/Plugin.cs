using System;
using System.Diagnostics;
using System.IO;
using DynamicsPlugin.Common;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace DynamicsPlugin
{
    [Serializable]
    public class Plugin : PluginBase
    {
        #region constructors
        public Plugin() : base()
        { }

        public Plugin(string unsecureString, string secureString) : base(unsecureString, secureString)
        { }
        #endregion

        public override string[] ValidMessageNames => new[] {"Create", "Update"};

        public override string RequiredPrimaryEntityLogicalName => "account";

        public override void Execute(ILocalPluginContext context)
        {
            //var sw = new StreamWriter("C:\\test.txt");
            //sw.WriteLine("ouch");
            var account = context.OrganizationService.Retrieve("account", Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new ColumnSet(true));


            var note = new Entity("annotation");
            note["description"] = "test";
            context.OrganizationService.Create(note);

        }
    }
}
