# DynamicsPlugin
The DynamicsPlugin solution is a starter solution for developing Dynamics CRM 365 plugins for CRM Online.

It simplifies developing plugins by:

1. Providing a base class that includes a LocalPluginContext that:
   * Helps get attribute values from the entity images as well as the target
   * Provides quick and easy access to simplify tracing
   * Provides additional checks for valid entity types and message types
   * Handles the access to the OrganizationService
1. Provides a starter Test Solution that uses spkl.fakes, which is part of Scott Durrow's SparkleXrm library and spkl.fakes is available as a nuget package.
1. Provides a restricted sandbox for testing to help ensure you are writing code that is usable in crm online.

All of this is to help simplify test driven development for plugins.  There are a few tests that test the base plugin by testing for failures handling invalid entity names, etc.

:warning: The plugin is not signed, and it must be.  There is an Assembly test to make sure the plugin is signed, so right out the gate there is a failed test that needs to resolved!

:question: If you have any problems, please post it in the issues.

:information_source: If you would like more information on spkl.fakes, check out: https://github.com/scottdurow/SparkleXrm/wiki/spkl.fakes

:information_source: If you have [pluralsight](http://pluralsight.com), Justin Pihony has a great session on Patterns for Pragmaytic Unit Testing.  He also references a number of other lessons, sessions, and videos that would help anyone get started with unit testing.  He does use NUnit in his video, but that is pretty easy to map over to MSTest using [this simple article on msdn](https://blogs.msdn.microsoft.com/nnaderi/2007/02/01/comparing-the-mstest-and-nunit-frameworks/) by Naysawn Naderi.
