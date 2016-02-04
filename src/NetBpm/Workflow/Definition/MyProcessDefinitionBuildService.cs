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
    public class MyProcessDefinitionBuildService
    {
        private XmlElement xmlElement = null;
        private IList<UnresolveTransition> unresolveTransitions = null;

        public MyProcessDefinitionBuildService(XmlElement xmlElement) 
        {
            this.xmlElement = xmlElement;
            unresolveTransitions = new List<UnresolveTransition>();
        }

        public ProcessDefinitionImpl BuildProcessDefinition() 
        {
            ProcessDefinitionImpl processDefinition = definition();
            processDefinition.StartState = start(processDefinition);
            processDefinition.EndState = end(processDefinition);
            processDefinition.Nodes.Add(processDefinition.StartState);
            processDefinition.Nodes.Add(processDefinition.EndState);

            foreach(var transition in unresolveTransitions)
            {
                var enumer = processDefinition.Nodes.GetEnumerator();
                while(enumer.MoveNext())
                {
                    INode node = (enumer.Current as INode);
                    if(transition.XmlValue == node.Name)
                    {
                        transition.To = node;
                        break;
                    }
                }
            }

            return processDefinition;
        }

        private ProcessDefinitionImpl definition() 
        {
            ProcessDefinitionImpl processDefinition = new ProcessDefinitionImpl();
            processDefinition.ProcessDefinition = processDefinition;
            this.processBlock(xmlElement, processDefinition);
            return processDefinition;
        }

        private StartStateImpl start(ProcessDefinitionImpl processDefinition) 
        {
            XmlElement startElement = xmlElement.GetChildElement("start-state");
            StartStateImpl startState = new StartStateImpl();
            startState.ProcessDefinition = processDefinition;
            this.activityState(startElement, startState, processDefinition);

            return startState;
        }

        private EndStateImpl end(ProcessDefinitionImpl processDefinition) 
        {
            XmlElement endElement = xmlElement.GetChildElement("end-state");
            EndStateImpl endState = new EndStateImpl();
            endState.ProcessDefinition = processDefinition;
            this.state(endElement, endState, processDefinition);

            return endState;
        }

        private void processBlock(XmlElement nodeElement, ProcessBlockImpl processBlock) 
        {
            processBlock.Nodes = new ListSet();
            processBlock.Attributes = new ListSet();
            processBlock.ChildBlocks = new ListSet();

            IEnumerator iterAttr = xmlElement.GetChildElements("attribute").GetEnumerator();
            while (iterAttr.MoveNext())
            {
                AttributeImpl attribute = new AttributeImpl();
                this.attribute((XmlElement)iterAttr.Current, attribute, processBlock);
                processBlock.Attributes.Add(attribute);
            }

            IEnumerator iter = nodeElement.GetChildElements("activity-state").GetEnumerator();
            while (iter.MoveNext())
            {
                ActivityStateImpl activityState = new ActivityStateImpl();
                activityState.ProcessDefinition = processBlock as IProcessDefinition;
                this.activityState((XmlElement)iter.Current, activityState, processBlock);
                processBlock.Nodes.Add(activityState);
            }

            this.definitionObject(nodeElement, processBlock, processBlock);
        }

        private void attribute(XmlElement attributeElement, AttributeImpl attribute, ProcessBlockImpl processBlock) 
        {
            attribute.InitialValue = attributeElement.GetProperty("initial-value");
            attribute.ProcessDefinition = processBlock as IProcessDefinition;
            DelegationImpl delegation = new DelegationImpl();
            attribute.SerializerDelegation = delegation;
            delegation.ProcessDefinition = processBlock as IProcessDefinition;
            this.definitionObject(attributeElement, attribute, processBlock);
            this.delegation<AttributeImpl>(attributeElement, (DelegationImpl)attribute.SerializerDelegation);
        }

        private void activityState(XmlElement nodeElement, ActivityStateImpl activityState, ProcessBlockImpl processBlock)
        {
            XmlElement assignmentElement = nodeElement.GetChildElement("assignment");
            if (assignmentElement != null)
            {
                DelegationImpl delegation = new DelegationImpl();
                delegation.ProcessDefinition = processBlock as IProcessDefinition;
                activityState.AssignmentDelegation = delegation;
                this.delegation<ActivityStateImpl>(assignmentElement, delegation);
            }

            this.state(nodeElement, activityState, processBlock);
        }

        private void state(XmlElement nodeElement, StateImpl state, ProcessBlockImpl processBlock)
        {
            //this.field();
            this.node(nodeElement, state, processBlock);
        }

        private void node(XmlElement nodeElement, NodeImpl node, ProcessBlockImpl processBlock) 
        {
            node.ArrivingTransitions = new ListSet();
            node.LeavingTransitions = new ListSet();

            IEnumerator iter = nodeElement.GetChildElements("transition").GetEnumerator();
            while (iter.MoveNext())
            {
                XmlElement transitionElement = (XmlElement)iter.Current;
                UnresolveTransition transition = new UnresolveTransition();
                transition.ProcessDefinition = processBlock as IProcessDefinition;
                transition.From = node;
                this.unresolveTransitions.Add(transition);
                this.transition(transitionElement, transition, processBlock);
                node.LeavingTransitions.Add(transition);
            }

            this.definitionObject(nodeElement, node, processBlock);
        }

        private void delegation<T>(XmlElement nodeElement,DelegationImpl delegation) 
        {
            Type delegatingObjectClass = typeof(T);
            if (delegatingObjectClass == typeof(AttributeImpl))
            {
                String type = nodeElement.GetProperty("type");
                if (string.IsNullOrEmpty(type) == false)
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
                    delegation.ClassName = nodeElement.GetProperty("serializer");
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

        private void transition(XmlElement transitionElement, UnresolveTransition transition, ProcessBlockImpl processBlock) 
        {
            transition.XmlValue = transitionElement.GetProperty("to");
            this.definitionObject(transitionElement, transition, processBlock);
        }

        private void definitionObject(XmlElement definitionObjectElement, DefinitionObjectImpl definitionObject, ProcessBlockImpl processBlock)
        {
            definitionObject.Name = definitionObjectElement.GetProperty("name");
            definitionObject.Description = definitionObjectElement.GetProperty("description");

            IEnumerator iter = definitionObjectElement.GetChildElements("action").GetEnumerator();
            while (iter.MoveNext())
            {
                XmlElement actionElement = (XmlElement)iter.Current;
                ActionImpl action = new ActionImpl();
                action.DefinitionObjectId = definitionObject.Id;
                this.action(actionElement, action, processBlock);
            }
        }

        private void action(XmlElement actionElement, ActionImpl actionImpl, ProcessBlockImpl processBlock) 
        {
            actionImpl.EventType = EventTypeHelper.fromText(actionElement.GetAttribute("event"));
            DelegationImpl delegation = new DelegationImpl();
            delegation.ProcessDefinition = processBlock as IProcessDefinition;
            actionImpl.ActionDelegation = delegation;
            this.delegation<ActionImpl>(actionElement, delegation);
        }
    }
}
