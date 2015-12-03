using NetBpm.Util.Xml;
using NetBpm.Workflow.Definition.Impl;
using NetBpm.Workflow.Delegation;
using NetBpm.Workflow.Delegation.Impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Definition
{
    public class ProcessDefinitionBuildService
    {
        private XmlElement xmlElement = null;

        string id;
        string name;
        StartStateImpl startState = null;

        public ProcessDefinitionBuildService(XmlElement xmlElement) 
        {
            this.xmlElement = xmlElement;
        }

        public void FetchProcessDefinition() 
        {
            this.start();
        }

        private void definition() 
        {
            
        }

        private void start() 
        {
            XmlElement startElement = xmlElement.GetChildElement("start-state");
            startState = new StartStateImpl();

           
        }

        private void end() 
        {
        
        }

        

        private void activityState(XmlElement nodeElement, ActivityStateImpl activityState)
        {
            //this.assign(startElement, startState);
            this.state(nodeElement,activityState);
        }

        private void state(XmlElement nodeElement, StateImpl state)
        {
            //this.field();
            this.node(nodeElement,state);
        }

        private void node(XmlElement nodeElement, NodeImpl node) 
        {
            node.ArrivingTransitions = new ListSet();
            node.LeavingTransitions = new ListSet();

            IEnumerator iter = xmlElement.GetChildElements("transition").GetEnumerator();
            while (iter.MoveNext())
            {
                XmlElement transitionElement = (XmlElement)iter.Current;
                TransitionImpl transition = new TransitionImpl();
                transition.From = node;
                this.transition(transitionElement,transition);
                node.LeavingTransitions.Add(transition);
            }

            this.definitionObject(nodeElement, node);
        }

        private void definitionObject(XmlElement nodeElement,DefinitionObjectImpl definitionObject) 
        {
            definitionObject.Name = nodeElement.GetProperty("name");
            definitionObject.Description = nodeElement.GetProperty("description");
        }

        private void assign(XmlElement nodeElement, ActivityStateImpl activityState)
        {
            this.state(nodeElement, activityState);
            XmlElement assignmentElement = xmlElement.GetChildElement("assignment");
            if (assignmentElement != null)
            {
                activityState.AssignmentDelegation = new DelegationImpl();
                this.delegation(assignmentElement, activityState.AssignmentDelegation);

            }
            activityState.ActorRoleName = xmlElement.GetProperty("role");
        }

        private void delegation(XmlElement nodeElement,DelegationImpl delegation) 
        {
            //Type delegatingObjectClass = delegation.GetDelegate().GetType();
            //if (delegatingObjectClass == typeof(AttributeImpl))
            //{
            //    String type = xmlElement.GetProperty("type");
            //    if ((Object)type != null)
            //    {
            //        delegation.ClassName = ((String)attributeTypes[type]);
            //        string suportedTypes = "supported types: ";
            //        foreach (Object o in attributeTypes.Keys)
            //        {
            //            suportedTypes += o.ToString() + " ,";
            //        }
            //    }
            //    else
            //    {
            //        delegation.ClassName = xmlElement.GetProperty("serializer");
            //    }
            //}
            //else if (delegatingObjectClass == typeof(FieldImpl))
            //{
            //    delegation.ClassName = xmlElement.GetProperty("class");
            //}
            //else
            //{
            //    delegation.ClassName = xmlElement.GetProperty("handler");
            //}

            //// parse the exception handler    
            //String exceptionHandlerText = xmlElement.GetAttribute("on-exception");
            //if ((Object)exceptionHandlerText != null)
            //{
            //    delegation.ExceptionHandlingType = ExceptionHandlingTypeHelper.FromText(exceptionHandlerText);
            //}

            //// create the configuration string
            //XmlElement configurationXml = new XmlElement("cfg");
            //IEnumerator iter = xmlElement.GetChildElements("parameter").GetEnumerator();
            //while (iter.MoveNext())
            //{
            //    configurationXml.AddChild((XmlElement)iter.Current);
            //}
            //delegation.Configuration = configurationXml.ToString();
        }

        private void transition(XmlElement nodeElement,TransitionImpl transition) 
        {
            this.definitionObject(nodeElement,transition);
        }
    }
}
