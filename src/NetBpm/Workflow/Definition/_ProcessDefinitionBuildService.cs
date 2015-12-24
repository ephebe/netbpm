using Iesi.Collections;
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

        public ProcessDefinitionBuildService(XmlElement xmlElement) 
        {
            this.xmlElement = xmlElement;
        }

        public ProcessDefinitionImpl BuildProcessDefinition() 
        {
            ProcessDefinitionImpl processDefinition = definition();
            processDefinition.StartState = start();
            processDefinition.EndState = end();
            processDefinition.Nodes.Add(processDefinition.StartState);
            processDefinition.Nodes.Add(processDefinition.EndState);
            return processDefinition;
        }

        private ProcessDefinitionImpl definition() 
        {
            ProcessDefinitionImpl processDefinition = new ProcessDefinitionImpl();
            this.processBlock(xmlElement, processDefinition);
            return processDefinition;
        }

        private StartStateImpl start() 
        {
            XmlElement startElement = xmlElement.GetChildElement("start-state");
            StartStateImpl startState = new StartStateImpl();

            this.activityState(startElement, startState);

            return startState;
        }

        private EndStateImpl end() 
        {
            XmlElement endElement = xmlElement.GetChildElement("end-state");
            EndStateImpl endState = new EndStateImpl();

            this.state(endElement, endState);

            return endState;
        }

        private void processBlock(XmlElement nodeElement, ProcessBlockImpl processBlock) 
        {
            processBlock.Nodes = new ListSet();
            processBlock.Attributes = new ListSet();
            processBlock.ChildBlocks = new ListSet();

            IEnumerator iter = nodeElement.GetChildElements("activity-state").GetEnumerator();
            while (iter.MoveNext())
            {
                ActivityStateImpl activityState = new ActivityStateImpl();
                this.activityState((XmlElement)iter.Current, activityState);
                processBlock.Nodes.Add(activityState);
            }

            this.definitionObject(nodeElement, processBlock);
        }

        private void activityState(XmlElement nodeElement, ActivityStateImpl activityState)
        {
            XmlElement assignmentElement = nodeElement.GetChildElement("assignment");
            if (assignmentElement != null)
            {
                DelegationImpl delegation = new DelegationImpl();
                activityState.AssignmentDelegation = delegation;
                this.delegation<ActivityStateImpl>(assignmentElement, delegation);
            }

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

            IEnumerator iter = nodeElement.GetChildElements("transition").GetEnumerator();
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

        private void delegation<T>(XmlElement nodeElement,DelegationImpl delegation) 
        {
            Type delegatingObjectClass = typeof(T);
            if (delegatingObjectClass == typeof(AttributeImpl))
            {
                String type = nodeElement.GetProperty("type");
                if ((Object)type != null)
                {
                    delegation.ClassName = ((String)DelegationImpl.attributeTypes[type]);
                    string suportedTypes = "supported types: ";
                    foreach (Object o in DelegationImpl.attributeTypes.Keys)
                    {
                        suportedTypes += o.ToString() + " ,";
                    }
                }
                else
                {
                    delegation.ClassName = xmlElement.GetProperty("serializer");
                }
            }
            else if (delegatingObjectClass == typeof(FieldImpl))
            {
                delegation.ClassName = nodeElement.GetProperty("class");
            }
            else
            {
                delegation.ClassName = nodeElement.GetProperty("handler");
            }

            // parse the exception handler    
            String exceptionHandlerText = nodeElement.GetAttribute("on-exception");
            if ((Object)exceptionHandlerText != null)
            {
                delegation.ExceptionHandlingType = ExceptionHandlingTypeHelper.FromText(exceptionHandlerText);
            }

            // create the configuration string
            XmlElement configurationXml = new XmlElement("cfg");
            IEnumerator iter = nodeElement.GetChildElements("parameter").GetEnumerator();
            while (iter.MoveNext())
            {
                configurationXml.AddChild((XmlElement)iter.Current);
            }
            delegation.Configuration = configurationXml.ToString();
        }

        private void transition(XmlElement nodeElement,TransitionImpl transition) 
        {
            this.definitionObject(nodeElement,transition);
        }

        private void definitionObject(XmlElement nodeElement, DefinitionObjectImpl definitionObject)
        {
            definitionObject.Name = nodeElement.GetProperty("name");
            definitionObject.Description = nodeElement.GetProperty("description");
        }
    }
}
