using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using DynamicsPlugin.Common.Attributes;
using DynamicsPlugin.Common.Constants;
using Microsoft.Xrm.Sdk;

namespace DynamicsPlugin.Common
{
    /// <summary>
    ///     Base class for all plug-in classes.
    /// </summary>
    [Serializable]
    public abstract class PluginBase : IPlugin
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PluginBase" /> class.
        /// </summary>
        public PluginBase()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PluginBase" /> class.
        /// </summary>
        public PluginBase(string unsecureString, string secureString)
        {
            UnsecureConfigString = unsecureString;
            SecureConfigString = secureString;
        }

        /// <summary>
        ///     Main entry point for he business logic that the plug-in is to execute.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <remarks>
        ///     For improved performance, Microsoft Dynamics 365 caches plug-in instances.
        ///     The plug-in's Execute method should be written to be stateless as the constructor
        ///     is not called for every invocation of the plug-in. Also, multiple system threads
        ///     could execute the plug-in at the same time. All per invocation state information
        ///     is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new InvalidPluginExecutionException(string.Format(ResponseMessages.NoServiceProvider,
                    PluginName));

            var localContext = new LocalPluginContext(serviceProvider);
            localContext.Trace(CultureInfo.InvariantCulture, TraceMessages.EnteringPlugin, PluginName);

            #region Load Config

            var configAttributeSettings = AttributeExtensions.GetCrmPluginConfigurationAttribute(GetType());
            ConfigType = configAttributeSettings.ConfigType;
            if (configAttributeSettings.AutoLoad)
                try
                {
                    LoadConfig();
                }
                catch (Exception ex)
                {
                    localContext.Trace(CultureInfo.InvariantCulture, TraceMessages.ErrorLoadingConfig, ex.Message);
                }

            #endregion

            var registrationAttributes = AttributeExtensions.GetCrmPluginRegistrationAttributes(GetType()).ToList();
            var execContext = localContext.PluginExecutionContext;

            #region Validate Primary EntityName (if specified)

            if (!registrationAttributes.IsValidEntity(execContext.PrimaryEntityName))
                throw new InvalidPluginExecutionException(
                    string.Format(ResponseMessages.InvalidEntity, execContext.PrimaryEntityName,
                        PluginName));

            #endregion

            #region Validate Message Names

            if (!registrationAttributes.IsValidMessageName(execContext.MessageName))
                throw new InvalidPluginExecutionException(
                    string.Format(ResponseMessages.InvalidMessageName, execContext.MessageName, PluginName));

            #endregion

            #region Validate Message Name and Entity Name combination

            if (!registrationAttributes.IsValidMessageAndEntityName(execContext.MessageName, execContext.PrimaryEntityName))
                throw new InvalidPluginExecutionException(
                    string.Format(ResponseMessages.InvalidMessageEntityCombination, execContext.MessageName, execContext.PrimaryEntityName, PluginName));

            #endregion

            try
            {
                // Invoke the custom implementation 
                Execute(localContext);
                // now exit

                if (configAttributeSettings.ForceErrorWhenComplete)
                    throw new InvalidPluginExecutionException(string.Format(ResponseMessages.PluginAborted,
                        PluginName));
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                localContext.Trace(TraceMessages.OrganizationServiceFault, PluginName);
                localContext.Trace(e);
                throw;
            }
            catch (InvalidPluginExecutionException pex)
            {
                localContext.Trace(TraceMessages.OrganizationServiceFault, PluginName);
                localContext.Trace(pex);
                throw;
            }
            catch (Exception ex)
            {
                localContext.Trace(TraceMessages.OrganizationServiceFault, PluginName);
                localContext.Trace(ex);
                // Handle the exception.
                throw new InvalidPluginExecutionException(
                    string.Format(ResponseMessages.OrganizationServiceFault, PluginName), ex);
            }
            finally
            {
                localContext.Trace(CultureInfo.InvariantCulture, TraceMessages.ExitingPlugin, PluginName);
            }
        }


        /// <summary>
        ///     Placeholder for a custom plug-in implementation.
        /// </summary>
        /// <param name="context">Context for the current plug-in.</param>
        public abstract void Execute(ILocalPluginContext context);

        #region Config Handling

        /// <summary>
        ///     Gets the configuration type for deserializing the <c>ConfigString</c> into the <c>Config</c> object.
        /// </summary>
        /// <value>The configuration type.</value>
        /// <example>ConfigType.Json</example>
        private ConfigType ConfigType { get; set; }

        /// <summary>
        ///     Gets the plugin configuration.
        /// </summary>
        /// <value>The plugin configuration.</value>
        public IPluginConfig Config { get; private set; }

        /// <summary ref="LoadConfig">
        ///     Load the configuration from the configuration string.
        /// </summary>
        /// <param name="configType">The type of configuration.</param>
        public void LoadConfig(ConfigType? configType = null)
        {
            var config = ConfigString;
            if (string.IsNullOrEmpty(config)) return;

            switch (configType ?? ConfigType)
            {
                case ConfigType.Json:
                    Config = JsonConfig.Deserialize<JsonConfig>(config);
                    break;
                case ConfigType.Xml:
                    Config = XmlConfig.Deserialize<XmlConfig>(config);
                    break;
                case ConfigType.None:
                    break;
                case ConfigType.String:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(configType), configType, null);
            }
        }

        #endregion

        #region Virtual and Abstract Properties (Settings)

        /// <summary>
        ///     Gets or sets the name of the child class.
        /// </summary>
        /// <value>The name of the child class.</value>
        public virtual string PluginName => GetType().Name;

        #endregion

        #region ConfigString Properties

        /// <summary>
        ///     Gets the unsecure config string registered to the plugin.
        /// </summary>
        /// <value>The unsecure config string registered to the plugin.</value>
        public string UnsecureConfigString { get; }

        /// <summary>
        ///     Gets the secure config string registered to the plugin.
        /// </summary>
        /// <value>The secure config string registered to the plugin.</value>
        public string SecureConfigString { get; }

        /// <summary>
        ///     Gets the plugin config string.  If the secure config string is empty, it will get the unsecure config string.
        /// </summary>
        /// <value>The config string registered to the plugin.</value>
        public string ConfigString =>
            string.IsNullOrEmpty(SecureConfigString) ? SecureConfigString : UnsecureConfigString;

        #endregion
    }
}