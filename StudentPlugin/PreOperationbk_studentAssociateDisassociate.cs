// <copyright file="PreOperationbk_studentAssociateDisassociate.cs" company="">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>1/30/2022 11:46:58 AM</date>
// <summary>Implements the PreOperationbk_studentAssociateDisassociate Plugin.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>

using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CrmPackage.StudentPlugin
{

    /// <summary>
    /// PreOperationbk_studentAssociateDisassociate Plugin.
    /// </summary>    
    public class PreOperationbk_studentAssociateDisassociate: PluginBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreOperationbk_studentAssociateDisassociate"/> class.
        /// </summary>
        /// <param name="unsecure">Contains public (unsecured) configuration information.</param>
        /// <param name="secure">Contains non-public (secured) configuration information. 
        /// When using Microsoft Dynamics 365 for Outlook with Offline Access, 
        /// the secure string is not passed to a plug-in that executes while the client is offline.</param>
        public PreOperationbk_studentAssociateDisassociate(string unsecure, string secure)
            : base(typeof(PreOperationbk_studentAssociateDisassociate))
        {
            
           // TODO: Implement your custom configuration handling.
        }


        /// <summary>
        /// Main entry point for he business logic that the plug-in is to execute.
        /// </summary>
        /// <param name="localContext">The <see cref="LocalPluginContext"/> which contains the
        /// <see cref="IPluginExecutionContext"/>,
        /// <see cref="IOrganizationService"/>
        /// and <see cref="ITracingService"/>
        /// </param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics 365 caches plug-in instances.
        /// The plug-in's Execute method should be written to be stateless as the constructor
        /// is not called for every invocation of the plug-in. Also, multiple system threads
        /// could execute the plug-in at the same time. All per invocation state information
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new InvalidPluginExecutionException("localContext");
            }

            EntityReference targetEntity = null;
            EntityReference relatedEntity = null;
            string relationshipName = string.Empty;
            Entity studentEntity = null;
            Entity classEntity = null;
            Entity courseEntity = null;

            if (localContext.PluginExecutionContext.InputParameters.Contains("Target") && localContext.PluginExecutionContext.InputParameters["Target"] is EntityReference)
            {
                targetEntity = localContext.PluginExecutionContext.InputParameters["Target"] as EntityReference;

            }

            if (localContext.PluginExecutionContext.InputParameters.Contains("Relationship"))
            {
                relationshipName = ((Relationship)localContext.PluginExecutionContext.InputParameters["Relationship"]).SchemaName;
            }

            if (relationshipName != "bk_bk_student_bk_class")
            {
                return;
            }

            if (localContext.PluginExecutionContext.InputParameters.Contains("RelatedEntities") && localContext.PluginExecutionContext.InputParameters["RelatedEntities"] is EntityReferenceCollection)
            {
                EntityReferenceCollection relatedEntityCollection =
                    localContext.PluginExecutionContext.InputParameters["RelatedEntities"] as EntityReferenceCollection;
                relatedEntity = relatedEntityCollection[0];
            }

            studentEntity = localContext.OrganizationService.Retrieve(targetEntity.LogicalName, targetEntity.Id,
                new ColumnSet("bk_courseappliedfor", "bk_subsidy", "bk_tax", "bk_totalfee", "bk_universityfee"));

            classEntity =
                localContext.OrganizationService.Retrieve(relatedEntity.LogicalName, relatedEntity.Id,
                    new ColumnSet("bk_courseid"));

            courseEntity = localContext.OrganizationService.Retrieve("bk_course",
                classEntity.GetAttributeValue<EntityReference>("bk_courseid").Id, new ColumnSet("bk_basicfee"));


            if (localContext.PluginExecutionContext.MessageName.ToLower() == "associate")
            {
                if (studentEntity.GetAttributeValue<EntityReference>("bk_courseappliedfor").Id !=
                    classEntity.GetAttributeValue<EntityReference>("bk_courseid").Id)
                {
                    throw new InvalidPluginExecutionException("The student is not applied for this course or class");
                }

                decimal subsity = studentEntity.GetAttributeValue<Money>("bk_subsidy") == null
                    ? new Money(0).Value
                    : studentEntity.GetAttributeValue<Money>("bk_subsidy").Value;

                decimal tax = studentEntity.GetAttributeValue<Money>("bk_tax") == null
                    ? new Money(0).Value
                    : studentEntity.GetAttributeValue<Money>("bk_tax").Value;

                decimal university = studentEntity.GetAttributeValue<Money>("bk_universityfee") == null
                    ? new Money(0).Value
                    : studentEntity.GetAttributeValue<Money>("bk_tax").Value;

                decimal baseFee = courseEntity.GetAttributeValue<Money>("bk_basicfee") == null
                    ? new Money(0).Value
                    : courseEntity.GetAttributeValue<Money>("bk_basicfee").Value; 

                Entity updateStudent = new Entity(targetEntity.LogicalName);
                updateStudent.Id = targetEntity.Id;

                updateStudent["bk_basicfee"] = new Money(baseFee);
                updateStudent["bk_subsidy"] = new Money(subsity);
                updateStudent["bk_tax"] = new Money(tax);
                updateStudent["bk_universityfee"] = new Money(university);
                updateStudent["bk_totalfee"] = new Money(baseFee + university + tax - subsity);
                localContext.OrganizationService.Update(updateStudent);

            }
            else if (localContext.PluginExecutionContext.MessageName.ToLower() == "disassociate")
            {
                decimal baseFee = courseEntity.GetAttributeValue<Money>("bk_basicfee") == null
                    ? new Money(0).Value
                    : courseEntity.GetAttributeValue<Money>("bk_basicfee").Value;

                decimal totalFee = studentEntity.GetAttributeValue<Money>("bk_totalfee") == null
                    ? new Money(0).Value
                    : studentEntity.GetAttributeValue<Money>("bk_totalfee").Value;

                Entity updateStudent = new Entity(targetEntity.LogicalName);
                updateStudent.Id = targetEntity.Id;

                updateStudent["bk_basicfee"] = new Money(baseFee);
                updateStudent["bk_subsidy"] = new Money(0);
                updateStudent["bk_tax"] = new Money(0);
                updateStudent["bk_universityfee"] = new Money(0);
                updateStudent["bk_totalfee"] = new Money(totalFee - baseFee);
                localContext.OrganizationService.Update(updateStudent);

                localContext.OrganizationService.Update(updateStudent);
            }

        }
    }
}