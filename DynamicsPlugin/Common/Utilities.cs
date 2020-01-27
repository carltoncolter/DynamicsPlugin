using Microsoft.Xrm.Sdk.Query;

namespace Plugins.Common
{
    using System;
    using System.Globalization;
    using System.Text;

    using Microsoft.Xrm.Sdk;

    /// <summary>
    ///     Common Plugin Utilities.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        ///     Convert EntityReference to an Entity
        ///     If a service is specified, then it will retrieve the record.
        /// </summary>
        /// <param name="target">
        ///     The target EntityReference.
        /// </param>
        /// <param name="service">
        ///     The Organization Service.
        /// </param>
        /// <param name="columns">
        ///     The columns to retrieve, if null, all columns are retrieved.
        /// </param>
        /// <returns>
        ///     The <see cref="Entity" />.
        /// </returns>
        public static Entity ToEntity(this EntityReference target,IOrganizationService service=null, ColumnSet columns=null) =>
            service==null ? 
                new Entity(target.LogicalName)
                {
                    Id = target.Id
                }
        : service.Retrieve(target.LogicalName,target.Id, columns??new ColumnSet(true));
    }
}