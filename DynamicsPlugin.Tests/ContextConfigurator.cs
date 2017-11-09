using System;
using Microsoft.Crm.Sdk.Fakes;

namespace DynamicsPlugin.Tests
{
    public class ContextConfigurator
    {
        public ContextConfigurator(PluginPipeline pipeline)
        {
            SetDefaultValues();
            BindContextToConfigurator(pipeline);
        }

        public SdkMessageProcessingStepMode Mode { get; set; }
        public PluginAssemblyIsolationMode IsolationMode { get; set; }
        public int Depth { get; set; }
        public string MessageName { get; set; }
        public Guid? RequestId { get; set; }
        public Guid UserId { get; set; }
        public Guid InitiatingUserId { get; set; }
        public Guid BusinessUnitId { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public Guid CorrelationId { get; set; }
        public bool IsExecutingOffline { get; set; }
        public bool IsOfflinePlayback { get; set; }
        public bool IsInTransaction { get; set; }
        public Guid OperationId { get; set; }
        public DateTime OperationCreatedOn { get; set; }

        private void SetDefaultValues()
        {
            // Set the default values
            Depth = 1;
            UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            InitiatingUserId = UserId;
            OrganizationId = Guid.Parse("c0000000-c000-c000-c000-c00000000000");
            OrganizationName = "TestOrganization";
            CorrelationId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
            BusinessUnitId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
            RequestId = Guid.NewGuid();
            OperationId = Guid.NewGuid();
            OperationCreatedOn = DateTime.Now;
            IsolationMode = PluginAssemblyIsolationMode.Sandbox;
            IsExecutingOffline = false;
            IsInTransaction = false;
            Mode = SdkMessageProcessingStepMode.Synchronous;
        }

        private void BindContextToConfigurator(PluginPipeline pipeline)
        {
            var context = pipeline.PluginExecutionContext;

            context.InitiatingUserIdGet = () => InitiatingUserId;
            context.UserIdGet = () => UserId;
            context.DepthGet = () => Depth;
            context.OrganizationIdGet = () => OrganizationId;
            context.OrganizationNameGet = () => OrganizationName;
            context.CorrelationIdGet = () => CorrelationId;
            context.BusinessUnitIdGet = () => BusinessUnitId;

            context.IsInTransactionGet = () => IsInTransaction;
            context.IsExecutingOfflineGet = () => IsExecutingOffline;

            context.IsolationModeGet = () => (int)IsolationMode;
            context.ModeGet = () => (int)Mode;
            context.RequestIdGet = () => RequestId;
            context.OperationCreatedOnGet = () => OperationCreatedOn;
            context.OperationIdGet = () => OperationId;
        }

        #region enums from SampleCode\CS\HelperCode\OptionSets.cs

        public enum PluginAssemblyIsolationMode
        {
            None = 1,
            Sandbox = 2
        }

        public enum SdkMessageProcessingStepMode
        {
            Synchronous = 0,
            Asynchronous = 1
        }
        #endregion
    }
}