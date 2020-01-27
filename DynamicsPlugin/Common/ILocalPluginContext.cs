using System;
using System.Globalization;
using Microsoft.Xrm.Sdk;

namespace DynamicsPlugin.Common
{
    public interface ILocalPluginContext
    {
        /// <summary>
        ///     Synchronous registered plug-ins can post the execution context to the Microsoft Azure Service Bus. <br />
        ///     It is through this notification service that synchronous plug-ins can send brokered messages to the Microsoft Azure
        ///     Service Bus.  The <c>NotificationService</c> posts the plug-in execution context to the Microsoft Azure Service
        ///     Bus.
        /// </summary>
        /// <remarks>
        ///     Both the Microsoft Azure Service Bus and Microsoft Dynamics 365 must be properly configured before using this
        ///     service.
        /// </remarks>
        IServiceEndpointNotificationService NotificationService { get; }

        /// <summary>
        ///     Provides programmatic access to the metadata and data for an organization.
        /// </summary>
        IOrganizationService OrganizationService { get; }

        /// <summary>
        ///     The factory for creating IOrganizationService instances.
        /// </summary>
        IOrganizationServiceFactory OrganizationServiceFactory { get; }

        /// <summary>
        ///     Defines the contextual information passed to a plug-in at run-time. Contains information that describes the
        ///     run-time environment that the plug-in is executing in, information related to the execution pipeline, and entity
        ///     business information.
        /// </summary>
        IPluginExecutionContext PluginExecutionContext { get; }

        /// <summary>
        ///     Gets the collection of registered post-operation images for the plugin.  Entity imnages are
        ///     <c>Microsoft.Xrm.Sdk.Entity</c> objects whose properties contain the values after the core platform operation has
        ///     completed.
        /// </summary>
        EntityImageCollection PostEntityImages { get; }

        /// <summary>
        ///     Gets the collection of registered pre-operation images for the plugin.  Entity imnages are
        ///     <c>Microsoft.Xrm.Sdk.Entity</c> objects whose properties contain the values before the core platform operation has
        ///     begins.
        /// </summary>
        EntityImageCollection PreEntityImages { get; }

        /// <summary>
        ///     The mechanism for retrieving a service object, such as:<br />
        ///     the plug-in execution context (<c>Microsoft.Xrm.Sdk.IPluginExecutionContext)</c>), <br />
        ///     tracing service (<c>Microsoft.Xrm.Sdk.ITracingService</c>), <br />
        ///     organization service (<c>Microsoft.Xrm.Sdk.IOrganizationServiceFactory</c>),
        ///     and the notification service (<c>Microsoft.Xrm.Sdk.IServiceEndpointNotificationService</c>).
        /// </summary>
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        ///     A service that provides a method of logging run-time trace information for plug-ins.
        /// </summary>
        /// <remarks>
        ///     A reference to a tracing service implementation that is obtained from the <c>ServiceProvider</c>.
        /// </remarks>
        ITracingService TracingService { get; }

        /// <summary>
        ///     Gets the value of the attribute.
        /// </summary>
        /// <typeparam name="T">The type of the attribute.</typeparam>
        /// <param name="fieldname">The logical name of the attribute.</param>
        /// <returns>The value of the attribute.</returns>
        T GetAttributeValue<T>(string fieldname);

        /// <summary>
        ///     Gets an entity image (pre or post) by name.
        /// </summary>
        /// <param name="name">The name of the entity image to find.</param>
        /// <returns>The entity image.</returns>
        Entity GetEntityImage(string name = null);

        /// <summary>
        ///     Gets the value of the input parameter.
        /// </summary>
        /// <typeparam name="T">The type of the value of the input parameter.</typeparam>
        /// <param name="field">The name of the input parameter.</param>
        /// <returns>The value of the input parameter.</returns>
        T GetInputParameter<T>(string field);

        /// <summary>
        ///     Logs trace information.
        /// </summary>
        /// <param name="cultureInfo">An object that supplies culture-specific formatting information.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        void Trace(CultureInfo cultureInfo, string format, params object[] args);

        /// <summary>
        ///     Logs trace information.
        /// </summary>
        /// <param name="exception">The exception that is to be logged to the trace log.</param>
        void Trace(Exception exception);

        /// <summary>
        ///     Logs trace information.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        void Trace(string format, params object[] args);
    }
}