using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;

namespace DynamicsPlugin.Common
{
    public enum EntityImageTypes
    {
        Pre,
        Post
    }

    /// <summary>
    ///     Defines the contextual information passed to a plug-in at run-time and the common services and methods used by
    ///     plug-ins.
    /// </summary>
    [ExcludeFromCodeCoverage] // This code has been tested heavily and is not likely to change much over time
    public class LocalPluginContext : ILocalPluginContext
    {
        private IServiceEndpointNotificationService _notificationService;
        private IOrganizationService _organizationService;
        private IOrganizationServiceFactory _organizationServiceFactory;
        private IPluginExecutionContext _pluginExecutionContext;
        private ITracingService _tracingService;


        /// <summary>
        ///     Helper object that stores the services available in this plug-in.
        /// </summary>
        /// <param name="serviceProvider">
        ///     A container for service objects. Contains references to:
        ///     the plug-in execution context (<c>Microsoft.Xrm.Sdk.IPluginExecutionContext)</c>), <br />
        ///     tracing service (<c>Microsoft.Xrm.Sdk.ITracingService</c>), <br />
        ///     organization service (<c>Microsoft.Xrm.Sdk.IOrganizationServiceFactory</c>),
        ///     and the notification service (<c>Microsoft.Xrm.Sdk.IServiceEndpointNotificationService</c>).
        /// </param>
        public LocalPluginContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            if (serviceProvider == null)
                throw new InvalidPluginExecutionException("serviceProvider");
        }

        /// <inheritdoc />
        public IServiceProvider ServiceProvider { get; }

        /// <inheritdoc />
        public IOrganizationServiceFactory OrganizationServiceFactory =>
            _organizationServiceFactory ?? (_organizationServiceFactory =
                (IOrganizationServiceFactory) ServiceProvider.GetService(typeof(IOrganizationServiceFactory))
            );

        /// <inheritdoc />
        public IOrganizationService OrganizationService => _organizationService ?? (_organizationService =
                                                               OrganizationServiceFactory.CreateOrganizationService(
                                                                   PluginExecutionContext.UserId));

        /// <inheritdoc />
        public IPluginExecutionContext PluginExecutionContext => _pluginExecutionContext ?? (_pluginExecutionContext =
                                                                     (IPluginExecutionContext) ServiceProvider
                                                                         .GetService(typeof(IPluginExecutionContext)));

        /// <inheritdoc />
        public IServiceEndpointNotificationService NotificationService =>
            _notificationService ?? (_notificationService =
                (IServiceEndpointNotificationService) ServiceProvider.GetService(
                    typeof(IServiceEndpointNotificationService)));

        /// <inheritdoc />
        public ITracingService TracingService => _tracingService ??
                                                 (_tracingService =
                                                     (ITracingService) ServiceProvider.GetService(
                                                         typeof(ITracingService)));

        /// <inheritdoc />
        public EntityImageCollection PreEntityImages => PluginExecutionContext.PreEntityImages;

        /// <inheritdoc />
        public EntityImageCollection PostEntityImages => PluginExecutionContext.PostEntityImages;

        /// <inheritdoc />
        /// <remarks>
        ///     GetAttributeValue gets the attribute from the target, then the post entity image, then the pre entity image
        /// </remarks>
        public T GetAttributeValue<T>(string fieldname)
        {
            if (PluginExecutionContext.InputParameters.Contains("Target"))
                if (PluginExecutionContext.InputParameters["Target"] is Entity target && target.Contains(fieldname))
                    return target.GetAttributeValue<T>(fieldname);

            Entity pre = null, post = null;
            if (PluginExecutionContext.PostEntityImages.Count > 0)
                post = PluginExecutionContext.PostEntityImages.First().Value;
            if (PluginExecutionContext.PreEntityImages.Count > 0)
                pre = PluginExecutionContext.PreEntityImages.First().Value;

            if (post != null && (pre == null || post.Contains(fieldname))) return post.GetAttributeValue<T>(fieldname);
            if (pre != null) return pre.GetAttributeValue<T>(fieldname);

            return default(T);
        }

        /// <inheritdoc />
        public T GetInputParameter<T>(string field)
        {
            if (PluginExecutionContext?.InputParameters == null ||
                !PluginExecutionContext.InputParameters.ContainsKey(field)) return default(T);

            return (T) PluginExecutionContext.InputParameters[field];
        }

        /// <inheritdoc />
        /// <remarks>Post entity images are checked for an image with the <c>name</c>, then pre entity images.</remarks>
        public Entity GetEntityImage(string name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                if (PluginExecutionContext.PostEntityImages.Count > 0)
                    return PluginExecutionContext.PostEntityImages.First().Value;
                if (PluginExecutionContext.PreEntityImages.Count > 0)
                    return PluginExecutionContext.PreEntityImages.First().Value;
            }
            if (PostEntityImages.ContainsKey(name)) return PostEntityImages[name];
            if (PreEntityImages.ContainsKey(name)) return PreEntityImages[name];

            return null;
        }

        #region Trace

        /// <inheritdoc />
        public void Trace(CultureInfo cultureInfo, string format, params object[] args)
        {
            if (cultureInfo == null) cultureInfo = CultureInfo.InvariantCulture;

            var message = format;
            if (args != null) message = string.Format(cultureInfo, format, args);

            if (string.IsNullOrWhiteSpace(message) || TracingService == null)
                return;

            if (PluginExecutionContext == null)
                TracingService.Trace(message);
            else
                // This should never happen
                TracingService.Trace(
                    "{0}, Correlation Id: {1}, Initiating User: {2}",
                    message,
                    PluginExecutionContext.CorrelationId,
                    PluginExecutionContext.InitiatingUserId);
        }

        /// <inheritdoc />
        public void Trace(Exception exception)
        {
            // Trace the first message using the embedded Trace to get the Correlation Id and User Id out.
            Trace($"Exception: {exception.Message}");

            // From here on use the tracing service trace
            Trace(exception.StackTrace);

            var faultException = exception as FaultException<OrganizationServiceFault>;
            if (faultException?.Detail == null) return;

            Trace($"Error Code: {faultException.Detail.ErrorCode}");
            Trace($"Detail Message: {faultException.Detail.Message}");
            if (!string.IsNullOrEmpty(faultException.Detail.TraceText))
            {
                Trace("Trace: ");
                Trace(faultException.Detail.TraceText);
            }

            if (faultException.Detail.ErrorDetails.Count > 0)
                Trace("Error Details: ");

            foreach (var item in faultException.Detail.ErrorDetails)
                Trace($"{item.Key,20} = {item.Value}");

            if (faultException.Detail.InnerFault == null) return;

            Trace(new FaultException<OrganizationServiceFault>(faultException.Detail.InnerFault));
        }

        /// <inheritdoc />
        public void Trace(string format, params object[] args)
        {
            if (args != null)
                Trace(null, format, args);
        }

        #endregion
    }
}