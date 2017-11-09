using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicsPlugin.Tests
{
    [TestClass]
    public class Assembly
    {
        [TestMethod]
        public void Assembly_IsSigned()
        {
            #region arrange - given
            var asm = typeof(DynamicsPlugin.Plugin).Assembly;
            var asmName = asm.GetName();
            #endregion

            #region act - when
            //always
            #endregion

            #region assert - then
            byte[] key = asmName.GetPublicKey();
            Assert.IsTrue(key.Length > 0, "Dynamics Plugins must be signed.");
            #endregion
        }
    }
}
