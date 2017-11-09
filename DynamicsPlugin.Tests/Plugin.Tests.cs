using System;
using DynamicsPlugin.Common.Constants;
using Microsoft.Crm.Sdk.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace DynamicsPlugin.Tests
{
    [TestClass]
    public class PluginTests
    {
        #region Test Settings

        private static readonly bool _testUsingSandbox = true;
        private readonly string _unsecureConfig = "";
        private readonly string _secureConfig = "";

        #endregion

        #region Success Tests

        [TestMethod]
        public void CreateAccount_ModifiesTheCreatedEntity()
        {
            #region arrange - given

            var entityName = "account";
            var target = new Entity(entityName) {Id = Guid.NewGuid()};

            #endregion

            using (var pipeline = new PluginPipeline(FakeMessageNames.Create, FakeStages.PreOperation, target))
            {
                //pipelines have to have the arrange and any inner asserts as part of them if you are attempting to
                //check a child entity

                #region pipeline responses and tests

                pipeline.FakeService.ExpectRetrieve((retrieveEntityName, retrieveEntityId, retrieveColumnSet) =>
                {
                    return new Entity("account");
                }).ExpectCreate(createEntity =>
                {
                    // test in create call
                    Assert.IsTrue(createEntity.LogicalName.Equals("annotation",
                        StringComparison.InvariantCultureIgnoreCase));
                    return Guid.NewGuid();
                });

                #endregion

                string exceptionOccurred = null;

                #region arrange - given with pipeline
                // ReSharper disable once UnusedVariable - It is bound to pipeline - it is used
                var contextConfig = new ContextConfigurator(pipeline);
                #endregion

                #region act - when
                pipeline.Execute(_plugin);
                #endregion

                #region assert - then

                Assert.IsNull(exceptionOccurred, exceptionOccurred);
                pipeline.FakeService.AssertExpectedCalls();

                #endregion
            }
        }

        #endregion

        #region Failure Tests

        [TestMethod]
        public void CreateUknownEntity_ThrowsAnError()
        {
            #region arrange - given

            var entityName = "==UnkownEntity==";
            var target = new Entity(entityName) { Id = Guid.NewGuid() };

            #endregion

            using (var pipeline = new PluginPipeline(FakeMessageNames.Create, FakeStages.PreOperation, target))
            {
                string exceptionOccurred = null;

                //Wrapped in try catch because a failure is expected.
                try
                {
                    #region arrange - given with pipeline

                    // ReSharper disable once UnusedVariable - It is bound to pipeline - it is used
                    var contextConfig = new ContextConfigurator(pipeline);

                    #endregion

                    #region act - when

                    pipeline.Execute(_plugin);

                    #endregion
                }
                catch (Exception ex)
                {
                    exceptionOccurred = ex.Message;
                    Trace(ex.Message);
                }

                #region assert - then

                Assert.AreEqual(string.Format(ResponseMessages.InvalidEntity, entityName, _plugin.PluginName),
                    exceptionOccurred);

                #endregion
            }
        }


        [TestMethod]
        public void UpdateUknownEntity_ThrowsAnError()
        {
            #region arrange - given

            var entityName = "==UnkownEntity==";
            var target = new Entity(entityName) { Id = Guid.NewGuid() };

            #endregion

            using (var pipeline = new PluginPipeline(FakeMessageNames.Update, FakeStages.PreOperation, target))
            {
                string exceptionOccurred = null;
                //Wrapped in try catch because a failure is expected.
                try
                {
                    #region arrange - given with pipeline

                    // ReSharper disable once UnusedVariable - It is bound to pipeline - it is used
                    var contextConfig = new ContextConfigurator(pipeline);

                    #endregion

                    #region act - when

                    pipeline.Execute(_plugin);

                    #endregion
                }
                catch (Exception ex)
                {
                    exceptionOccurred = ex.Message;
                    Trace(ex.Message);
                }

                #region assert - then

                Assert.AreEqual(string.Format(ResponseMessages.InvalidEntity, entityName, _plugin.PluginName),
                    exceptionOccurred);

                #endregion
            }
        }

        [TestMethod]
        public void DeleteAccount_ThrowsAnError()
        {
            #region arrange - given

            var entityName = "==UnkownEntity==";
            var target = new Entity(entityName) { Id = Guid.NewGuid() };

            #endregion

            using (var pipeline = new PluginPipeline(FakeMessageNames.Delete, FakeStages.PreOperation, target))
            {
                string exceptionOccurred = null;
                //Wrapped in try catch because a failure is expected.
                try
                {
                    #region arrange - given with pipeline

                    // ReSharper disable once UnusedVariable - It is bound to pipeline - it is used
                    var contextConfig = new ContextConfigurator(pipeline);

                    #endregion

                    #region act - when

                    pipeline.Execute(_plugin);

                    #endregion
                }
                catch (Exception ex)
                {
                    exceptionOccurred = ex.Message;
                    Trace(ex.Message);
                }

                #region assert - then

                Assert.AreEqual(string.Format(ResponseMessages.InvalidMessageName, _plugin.PluginName),
                    exceptionOccurred);

                #endregion
            }
        }

        #endregion

        #region Private Test Variables

        private IOrganizationService _fakeService;
        private static PluginSandbox<Plugin> _sandbox;
        private Plugin _plugin;

        #endregion

        #region Test Setup/Configuration Methods

        public TestContext TestContext { get; set; }

        /// <summary>
        ///     Called once prior to executing any test in the class
        /// </summary>
        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            if (_testUsingSandbox) _sandbox = new PluginSandbox<Plugin>();
        }

        /// <summary>
        ///     Called once to cleanup the class
        /// </summary>
        [ClassCleanup]
        public static void CleanupClass()
        {
        }

        /// <summary>
        ///     Called prior to executing each test method
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            _fakeService = new FakeOrganzationService();
            _plugin = _testUsingSandbox
                ? _sandbox.Create(_unsecureConfig, _secureConfig)
                : new Plugin(_unsecureConfig, _secureConfig);

            //_plugin = new Plugin(_unsecureConfig, _secureConfig);
        }

        /// <summary>
        ///     Called after executing each test method
        /// </summary>
        [TestCleanup]
        public void CleanupTest()
        {
            //fakeService cleaned by Garbage Collector
            _plugin = null;
            if (_sandbox != null)
            {
                _sandbox.Dispose();
                _sandbox = null;
            }
        }

        #region Helper Methods

        private void Trace(string message)
        {
            TestContext.WriteLine(message);
            Console.WriteLine(message);
            System.Diagnostics.Trace.WriteLine(message);
        }

        #endregion

        #endregion

        
    }
}