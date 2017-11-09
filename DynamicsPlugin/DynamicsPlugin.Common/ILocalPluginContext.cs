using System;
using System.Globalization;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;

namespace DynamicsPlugin.Common
{
    public interface ILocalPluginContext
    {
       
        IServiceEndpointNotificationService NotificationService { get; }
        IOrganizationService OrganizationService { get; }
        IOrganizationServiceFactory OrganizationServiceFactory { get; }
        IPluginExecutionContext PluginExecutionContext { get; }

        EntityImageCollection PostEntityImages { get; }
        EntityImageCollection PreEntityImages { get; }
        
        IServiceProvider ServiceProvider { get; }
        ITracingService TracingService { get; }
        

        T GetAttributeValue<T>(string fieldname);
        Entity GetEntityImage(string name = null);
        T GetInputParameter<T>(string field);

        void Trace(CultureInfo cultureInfo, string format, params object[] args);
        void Trace(Exception exception);
        void Trace(string format, params object[] args);
    }
}