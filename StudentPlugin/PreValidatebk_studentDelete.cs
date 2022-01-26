// <copyright file="PreValidatebk_studentDelete.cs" company="">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>1/26/2022 11:21:41 AM</date>
// <summary>Implements the PreValidatebk_studentDelete Plugin.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>

namespace CrmPackage.StudentPlugin
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    /// <summary>
    /// PreValidatebk_studentDelete Plugin.
    /// </summary>
    public class PreValidatebk_studentDelete : PluginBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreValidatebk_studentDelete"/> class.
        /// </summary>
        /// <param name="unsecure">Contains public (unsecured) configuration information.</param>
        /// <param name="secure">The secure<see cref="string"/>.</param>
        public PreValidatebk_studentDelete(string unsecure, string secure)
            : base(typeof(PreValidatebk_studentDelete))
        {
        }

        /// <summary>
        /// Main entry point for he business logic that the plug-in is to execute.
        /// </summary>
        /// <param name="localContext">The localContext<see cref="LocalPluginContext"/>.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new InvalidPluginExecutionException("localContext");
            }

            var context = localContext.PluginExecutionContext;
            var service = localContext.OrganizationService;

            if (context.MessageName.ToLower() != "delete" && context.Stage != 10)
            {
                return;
            }


            EntityReference targetEntity = context.InputParameters["Target"] as EntityReference;

            QueryExpression qeStudentCourseEntity = new QueryExpression()
            {
                EntityName = "bk_studentcourse",
                ColumnSet = new ColumnSet("bk_name"),
                Criteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression("bk_student", ConditionOperator.Equal, targetEntity.Id)
                        }
                    }
            };

            EntityCollection ec = service.RetrieveMultiple(qeStudentCourseEntity);

            if (ec.Entities.Count != 0)
            {
                throw new Exception("Child record of Student found. Cannot delete this record");
            }
        }
    }
}
