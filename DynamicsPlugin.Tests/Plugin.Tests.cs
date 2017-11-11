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

        private readonly string _unsecureConfig = "";
        private readonly string _secureConfig = "";

        #endregion

        #region Success Tests

        [TestMethod]
        public void CreateAccount_ModifiesTheCreatedEntity()
        {
            #region arrange - given

            var entityName = "account";
            var target = new Entity(entityName) { Id = Guid.NewGuid() };

            #endregion

            using (var pipeline = new PluginPipeline(FakeMessageNames.Create, FakeStages.PreOperation, target))
            using (var plugin = new PluginContainer<Plugin>(true, _unsecureConfig, _secureConfig))
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
                SetPipelineDefaults(pipeline);
                #endregion

                #region act - when
                pipeline.Execute(plugin);
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
            using (var plugin = new PluginContainer<Plugin>(_unsecureConfig, _secureConfig))
            {
                string exceptionOccurred = null;

                //Wrapped in try catch because a failure is expected.
                try
                {
                    #region arrange - given with pipeline

                    SetPipelineDefaults(pipeline);

                    #endregion

                    #region act - when

                    pipeline.Execute(plugin);

                    #endregion
                }
                catch (Exception ex)
                {
                    exceptionOccurred = ex.Message;
                    Trace(ex.Message);
                }

                #region assert - then

                Assert.AreEqual(string.Format(ResponseMessages.InvalidEntity, entityName, plugin.Instance.PluginName),
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
            using (var plugin = new PluginContainer<Plugin>(_unsecureConfig, _secureConfig))
            {
                string exceptionOccurred = null;
                //Wrapped in try catch because a failure is expected.
                try
                {
                    #region arrange - given with pipeline

                    SetPipelineDefaults(pipeline);

                    #endregion

                    #region act - when

                    pipeline.Execute(plugin);

                    #endregion
                }
                catch (Exception ex)
                {
                    exceptionOccurred = ex.Message;
                    Trace(ex.Message);
                }

                #region assert - then

                Assert.AreEqual(string.Format(ResponseMessages.InvalidEntity, entityName, plugin.Instance.PluginName),
                    exceptionOccurred);

                #endregion
            }


        }

        [TestMethod]
        public void DeleteAccount_ThrowsAnError()
        {
            #region arrange - given

            var entityName = "account";
            var target = new Entity(entityName) { Id = Guid.NewGuid() };

            #endregion

            using (var pipeline = new PluginPipeline(FakeMessageNames.Delete, FakeStages.PreOperation, target))
            using (var plugin = new PluginContainer<Plugin>(_unsecureConfig, _secureConfig))
            {
                string exceptionOccurred = null;
                //Wrapped in try catch because a failure is expected.
                try
                {
                    #region arrange - given with pipeline

                    SetPipelineDefaults(pipeline);

                    #endregion

                    #region act - when

                    pipeline.Execute(plugin);

                    #endregion
                }
                catch (Exception ex)
                {
                    exceptionOccurred = ex.Message;
                    Trace(ex.Message);
                }

                #region assert - then

                Assert.AreEqual(string.Format(ResponseMessages.InvalidMessageName, FakeMessageNames.Delete, plugin.Instance.PluginName),
                    exceptionOccurred);

                #endregion
            }
        }

        [TestMethod]
        public void CatchInvalidSandboxWebAccess()
        {
            #region arrange - given

            var entityName = "account";
            var target = new Entity(entityName) { Id = Guid.NewGuid() };

            #endregion

            using (var pipeline = new PluginPipeline(FakeMessageNames.Create, FakeStages.PreOperation, target))
            using (var plugin = new PluginContainer<Plugin>(true, _unsecureConfig, _secureConfig))
            {

                #region arrange - given with pipeline

                SetPipelineDefaults(pipeline);

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

                #region act - when

                pipeline.Execute(plugin);

                #endregion

                #region assert - then


                #endregion
            }
        }
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

        private void SetPipelineDefaults(PluginPipeline pipeline)
        {
            // Set the default values
            //pipeline.PluginExecutionContext.Depth = 1;
            pipeline.UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            pipeline.InitiatingUserId = pipeline.UserId;
            pipeline.OrganizationId = Guid.Parse("c0000000-c000-c000-c000-c00000000000");
            pipeline.OrganizationName = "TestOrganization";
            pipeline.CorrelationId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
            pipeline.BusinessUnitId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
            pipeline.RequestId = Guid.NewGuid();
            pipeline.OperationId = Guid.NewGuid();
            pipeline.OperationCreatedOn = DateTime.Now;
            pipeline.IsolationMode = PluginAssemblyIsolationMode.Sandbox;
            pipeline.IsExecutingOffline = false;
            pipeline.IsInTransaction = false;
            pipeline.Mode = SdkMessageProcessingStepMode.Synchronous;
        }

        #endregion

        #endregion
    }
}