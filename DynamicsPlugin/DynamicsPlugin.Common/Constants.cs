namespace DynamicsPlugin.Common.Constants
{
    public static class ResponseMessages
    {
        public const string NoServiceProvider = "There was no Service Provider.  {0} is unable to execute.";

        public const string InvalidEntity = "The entity {0} is not support by {1}.";

        public const string InvalidMessageName = "The message name is not support by {0}.";

        public const string OrganizationServiceFault =
            "There was an organization service fault.  {0} is unable to execute successfully.";

        public const string PluginAborted = "{0} Aborted.  Forced error at the end of execution.";
    }

    public static class TraceMessages
    {
        public const string OrganizationServiceFault = "There was an organization service fault.  {0} is unable to execute successfully.";

        public const string EnteringPlugin = "Entered {0}.Execute()";

        public const string ExitingPlugin = "Exiting {0}.Execute()";
    }
}