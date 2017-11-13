using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using DynamicsPlugin.Common.Constants;

namespace DynamicsPlugin.Common.Attributes
{
    public static class AttributeExtensions
    {
        #region CrmPluginConfigurationAttribute: Get and Create
        public static CrmPluginConfigurationAttribute GetCrmPluginConfigurationAttribute(Type type)
        {
            var attribute =
                type.GetCustomAttributesData()
                    .FirstOrDefault(a => a.AttributeType == typeof(CrmPluginConfigurationAttribute));

            return attribute != null ? attribute.CreateConfigurationAttributeFromData() : new CrmPluginConfigurationAttribute();
        }

        /// <summary>
        /// Reads the CustomAttributeData to create a CrmPluginConfigurationAttribute object to use as a 
        /// data object for settings on how a plugin is registered.  The default attribute data is returned
        /// if the attribute is not found.
        /// </summary>
        /// <param name="data">The CustomAttributeData</param>
        /// <returns>CrmPluginConfigurationAttribute</returns>
        public static CrmPluginConfigurationAttribute CreateConfigurationAttributeFromData(
            this CustomAttributeData data)
        {
            var attribute = new CrmPluginConfigurationAttribute();
            if (data.NamedArguments == null) return attribute;
            foreach (var namedArgument in data.NamedArguments)
                switch (namedArgument.MemberName)
                {
                    case "ConfigType":
                        attribute.ConfigType = (ConfigType)namedArgument.TypedValue.Value;
                        break;
                    case "AutoLoad":
                        attribute.AutoLoad = (bool)namedArgument.TypedValue.Value;
                        break;
                    case "ForceErrorWhenComplete":
                        attribute.ForceErrorWhenComplete = (bool)namedArgument.TypedValue.Value;
                        break;
                }

            return attribute;
        }
        #endregion

        #region CrmPluginRegistrationAttribute: Get and Create
        public static IEnumerable<CrmPluginRegistrationAttribute> GetCrmPluginRegistrationAttributes(Type type)
        {
            var data = type.GetCustomAttributesData()
                .Where(a => a.AttributeType.Name == "CrmPluginRegistrationAttribute");

            // Don't allow multiple steps with the same name per type
            var results = data.Select(a => a.CreateRegistrationAttributeFromData()).ToList();

            var duplicateNames = results.GroupBy(s => s.Name).SelectMany(grp => grp.Skip(1)).ToArray();
            if (duplicateNames.Any())
            {
                var names = string.Join(", ", duplicateNames.Select(a => a.Name).ToArray());
                throw new DuplicateNameException(string.Format(ResponseMessages.DuplicatePluginStepNames, names));
            }

            return results;
        }

        /// <summary>
        /// Reads the CustomAttributeData to create a CrmPluginRegistrationAttribute object to use as a 
        /// data object for settings on how a plugin is registered.
        /// </summary>
        /// <param name="data">The CustomAttributeData</param>
        /// <returns>CrmPluginRegistrationAttribute</returns>
        /// <remarks>Taken from Scott Durrow's spkl sourcecode: spkl/SparkleXrm.Tasks/CustomAttributeDataEx.cs - CreateFromData</remarks>
        public static CrmPluginRegistrationAttribute CreateRegistrationAttributeFromData(this CustomAttributeData data)
        {
            CrmPluginRegistrationAttribute attribute;
            var arguments = data.ConstructorArguments.ToArray();
            // determine which constructor is being used by the first type
            if (data.ConstructorArguments.Count == 8 && data.ConstructorArguments[0].ArgumentType.Name == "String")
                attribute = new CrmPluginRegistrationAttribute(
                    (string)arguments[0].Value,
                    (string)arguments[1].Value,
                    (StageEnum)Enum.ToObject(typeof(StageEnum), (int)arguments[2].Value),
                    (ExecutionModeEnum)Enum.ToObject(typeof(ExecutionModeEnum), (int)arguments[3].Value),
                    (string)arguments[4].Value,
                    (string)arguments[5].Value,
                    (int)arguments[6].Value,
                    (IsolationModeEnum)Enum.ToObject(typeof(IsolationModeEnum), (int)arguments[7].Value)
                );
            else if (data.ConstructorArguments.Count == 8 &&
                     data.ConstructorArguments[0].ArgumentType.Name == "MessageNameEnum")
                attribute = new CrmPluginRegistrationAttribute(
                    (MessageNameEnum)Enum.ToObject(typeof(MessageNameEnum), (int)arguments[0].Value),
                    (string)arguments[1].Value,
                    (StageEnum)Enum.ToObject(typeof(StageEnum), (int)arguments[2].Value),
                    (ExecutionModeEnum)Enum.ToObject(typeof(ExecutionModeEnum), (int)arguments[3].Value),
                    (string)arguments[4].Value,
                    (string)arguments[5].Value,
                    (int)arguments[6].Value,
                    (IsolationModeEnum)Enum.ToObject(typeof(IsolationModeEnum), (int)arguments[7].Value)
                );
            else if (data.ConstructorArguments.Count == 5 && data.ConstructorArguments[0].ArgumentType.Name == "String")
                attribute = new CrmPluginRegistrationAttribute(
                    (string)arguments[0].Value,
                    (string)arguments[1].Value,
                    (string)arguments[2].Value,
                    (string)arguments[3].Value,
                    (IsolationModeEnum)Enum.ToObject(typeof(IsolationModeEnum), (int)arguments[4].Value)
                );
            else
                return null;

            if (data.NamedArguments == null) return attribute;
            foreach (var namedArgument in data.NamedArguments)
                switch (namedArgument.MemberName)
                {
                    case "Id":
                        attribute.Id = (string)namedArgument.TypedValue.Value;
                        break;
                    case "FriendlyName":
                        attribute.FriendlyName = (string)namedArgument.TypedValue.Value;
                        break;
                    case "GroupName":
                        attribute.FriendlyName = (string)namedArgument.TypedValue.Value;
                        break;
                    case "Image1Name":
                        attribute.Image1Name = (string)namedArgument.TypedValue.Value;
                        break;
                    case "Image1Attributes":
                        attribute.Image1Attributes = (string)namedArgument.TypedValue.Value;
                        break;
                    case "Image2Name":
                        attribute.Image2Name = (string)namedArgument.TypedValue.Value;
                        break;
                    case "Image2Attributes":
                        attribute.Image2Attributes = (string)namedArgument.TypedValue.Value;
                        break;
                    case "Image1Type":
                        attribute.Image1Type = (ImageTypeEnum)namedArgument.TypedValue.Value;
                        break;
                    case "Image2Type":
                        attribute.Image2Type = (ImageTypeEnum)namedArgument.TypedValue.Value;
                        break;
                    case "Description":
                        attribute.Description = (string)namedArgument.TypedValue.Value;
                        break;
                    case "DeleteAsyncOperaton":
                        attribute.DeleteAsyncOperaton = (bool)namedArgument.TypedValue.Value;
                        break;
                    case "UnSecureConfiguration":
                        attribute.UnSecureConfiguration = (string)namedArgument.TypedValue.Value;
                        break;
                    case "SecureConfiguration":
                        attribute.SecureConfiguration = (string)namedArgument.TypedValue.Value;
                        break;
                    case "Offline":
                        attribute.Offline = (bool)namedArgument.TypedValue.Value;
                        break;
                    case "Server":
                        attribute.Server = (bool)namedArgument.TypedValue.Value;
                        break;
                    case "Action":
                        attribute.Action = (PluginStepOperationEnum)namedArgument.TypedValue.Value;
                        break;
                }
            return attribute;
        }

        #endregion

        #region CrmPluginRegistrationAttribute: Conditional Checks
        public static bool IsValidEntity(this IEnumerable<CrmPluginRegistrationAttribute> attributes, string entityName)
        {
            return attributes.Any(a =>
                a.EntityLogicalName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool IsValidMessageName(this IEnumerable<CrmPluginRegistrationAttribute> attributes, string message)
        {
            return attributes.Any(a => a.Message.Equals(message, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool IsValidMessageAndEntityName(this IEnumerable<CrmPluginRegistrationAttribute> attributes,
            string message, string entityName)
        {
            return attributes.Any(a =>
                a.EntityLogicalName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase) &&
                a.Message.Equals(message, StringComparison.InvariantCultureIgnoreCase));
        }
        #endregion

    }
}