namespace DynamicsPlugin.Common.Constants
{
    public static class ResponseMessages
    {
        public const string NoServiceProvider = "There was no Service Provider.  {0} is unable to execute.";

        public const string InvalidEntity = "The entity {0} is not support by {1}.";

        public const string InvalidMessageName = "The message \"{0}\" is not support by {1}.";

        public const string InvalidMessageEntityCombination =
            "The message \"{0}\" is not supported for entity \"{1}\" by {2}.";

        public const string OrganizationServiceFault =
            "There was an organization service fault.  {0} is unable to execute successfully.";

        public const string PluginAborted = "{0} Aborted.  Forced error at the end of execution.";

        public const string DuplicatePluginStepNames = "Found types with duplicate attributes of the same name(s) {0}";
    }

    public static class TraceMessages
    {
        public const string OrganizationServiceFault = "There was an organization service fault.  {0} is unable to execute successfully.";

        public const string EnteringPlugin = "Entered {0}.Execute()";

        public const string ExitingPlugin = "Exiting {0}.Execute()";

        public const string ErrorLoadingConfig = "Error Loading Configuration: {0}";
    }
}