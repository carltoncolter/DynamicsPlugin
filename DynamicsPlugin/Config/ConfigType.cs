namespace DynamicsPlugin.Common
{
    /// <summary>
    /// The type of configuration.  This is utilized when loading a configuration string into an object.
    /// </summary>
    public enum ConfigType
    {
        /// <summary>
        /// None: There is no configuration (nothing is done to load the configuration)
        /// </summary>
        None,
        /// <summary>
        /// String: The configuration is a string, not an object, so nothing is done to load the configuration.
        /// </summary>
        String,
        /// <summary>
        /// Json: The configuration is json, and when loaded, by default, it will load as a JsonConfig object.
        /// </summary>
        Json,
        /// <summary>
        /// Xml: The configuration is xml, and when loaded, by default, it will load as an XmlConfig object.
        /// </summary>
        Xml
    }
}