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
    public class MyProcessDefinitionBuilder
    {
        private XmlElement xmlElement = null;
        private IDictionary<ReferencableObject, IDictionary<string, object>> referencableObjects = null;
        private IList<UnresolvedReference> unresolvedReferences = null;

        private ProcessDefinitionImpl ProcessDefinition { get; set; }

        public MyProcessDefinitionBuilder(XmlElement xmlElement) 
        {
            this.xmlElement = xmlElement;
            referencableObjects = new Dictionary<ReferencableObject, IDictionary<string, object>>();
            unresolvedReferences = new List<UnresolvedReference>();
        }

        public ProcessDefinitionImpl BuildProcessDefinition() 
        {
            ProcessDefinition = new ProcessDefinitionImpl();
            this.definition();
            ProcessDefinition.StartState = start();
            ProcessDefinition.EndState = end();
            ProcessDefinition.Nodes.Add(ProcessDefinition.StartState);
            ProcessDefinition.Nodes.Add(ProcessDefinition.EndState);

            resolveReferences();

            return ProcessDefinition;
        }

        private void definition() 
        {
            ProcessDefinition.ProcessDefinition = ProcessDefinition;
            this.processBlock(xmlElement, ProcessDefinition);
        }

        private StartStateImpl start() 
        {
            XmlElement startElement = xmlElement.GetChildElement("start-state");
            StartStateImpl startState = new StartStateImpl();
            startState.ProcessDefinition = ProcessDefinition;
            this.activityState(startElement, startState, ProcessDefinition);

            return startState;
        }

        private EndStateImpl end() 
        {
            XmlElement endElement = xmlElement.GetChildElement("end-state");
            EndStateImpl endState = new EndStateImpl();
            endState.ProcessDefinition = ProcessDefinition;
            this.state(endElement, endState, ProcessDefinition);

            return endState;
        }

        private void processBlock(XmlElement processBlockElement, ProcessBlockImpl currentProcessBlock) 
        {
            currentProcessBlock.Nodes = new ListSet();
            currentProcessBlock.Attributes = new ListSet();
            currentProcessBlock.ChildBlocks = new ListSet();

            IEnumerator iterAttr = processBlockElement.GetChildElements("attribute").GetEnumerator();
            while (iterAttr.MoveNext())
            {
                AttributeImpl attribute = new AttributeImpl();
                this.attribute((XmlElement)iterAttr.Current, attribute, currentProcessBlock);
                currentProcessBlock.Attributes.Add(attribute);
            }

            IEnumerator iterActivityState = processBlockElement.GetChildElements("activity-state").GetEnumerator();
            while (iterActivityState.MoveNext())
            {
                ActivityStateImpl activityState = currentProcessBlock.CreateActivityState();
                this.activityState((XmlElement)iterActivityState.Current, activityState, currentProcessBlock);
                currentProcessBlock.Nodes.Add(activityState);
            }

            IEnumerator iterConcurrent = processBlockElement.GetChildElements("concurrent-block").GetEnumerator();
            while (iterConcurrent.MoveNext())
            {
                ConcurrentBlockImpl concurrentBlock = currentProcessBlock.CreateConcurrentBlock();
                this.concurrentBlock((XmlElement)iterConcurrent.Current, concurrentBlock, currentProcessBlock);
            }

            IEnumerator iterDecision = processBlockElement.GetChildElements("decision").GetEnumerator();
            while (iterDecision.MoveNext())
            {
                DecisionImpl decision = currentProcessBlock.CreateDecision();
                this.decision((XmlElement)iterDecision.Current, decision,currentProcessBlock);
            }

            this.definitionObject(processBlockElement, currentProcessBlock, currentProcessBlock);
        }

        private void decision(XmlElement decisionElement, DecisionImpl decision, ProcessBlockImpl currentProcessBlock) 
        {
            this.node(decisionElement, decision, currentProcessBlock);

            decision.DecisionDelegation = new DelegationImpl();
            this.delegation<DecisionImpl>(decisionElement, decision.DecisionDelegation);
        }

        private void concurrentBlock(XmlElement concurrentElement, ConcurrentBlockImpl concurrentBlock, ProcessBlockImpl currentProcessBlock)
        {
            concurrentBlock.ProcessDefinition = ProcessDefinition;
            this.processBlock(concurrentElement, concurrentBlock);

            JoinImpl joinImpl = concurrentBlock.CreateJoin();
            ForkImpl forkImpl = concurrentBlock.CreateFork();

            XmlElement joinElement = concurrentElement.GetChildElement("join");
            this.join(joinElement, joinImpl, concurrentBlock);
            XmlElement forkElement = concurrentElement.GetChildElement("fork");
            this.fork(forkElement, forkImpl, concurrentBlock);

            this.addReferencableObject(joinImpl.Name, currentProcessBlock, typeof(INode), joinImpl);
            this.addReferencableObject(forkImpl.Name, currentProcessBlock, typeof(INode), forkImpl);
        }

        private void join(XmlElement joinElement,JoinImpl joinImpl, ProcessBlockImpl currentProcessBlock) 
        {
            this.node(joinElement, joinImpl, currentProcessBlock);

            if ((Object)joinElement.GetAttribute("handler") != null)
            {
                joinImpl.JoinDelegation = new DelegationImpl();
                this.delegation<JoinImpl>(joinElement, joinImpl.JoinDelegation);
            }
        }

        private void fork(XmlElement forkElement, ForkImpl forkImpl, ProcessBlockImpl currentProcessBlock)
        {
            this.node(forkElement, forkImpl, currentProcessBlock);

            if ((Object)forkElement.GetAttribute("handler") != null)
            {
                forkImpl.ForkDelegation = new DelegationImpl();
                this.delegation<ForkImpl>(forkElement, forkImpl.ForkDelegation);
            }
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

        private void activityState(XmlElement activityStateElement, ActivityStateImpl activityState, ProcessBlockImpl processBlock)
        {
            activityState.ProcessDefinition = ProcessDefinition;
            XmlElement assignmentElement = activityStateElement.GetChildElement("assignment");
            if (assignmentElement != null)
            {
                DelegationImpl delegation = new DelegationImpl();
                delegation.ProcessDefinition = processBlock as IProcessDefinition;
                activityState.AssignmentDelegation = delegation;
                this.delegation<ActivityStateImpl>(assignmentElement, delegation);
            }

            activityState.ActorRoleName = activityStateElement.GetProperty("role");

            this.state(activityStateElement, activityState, processBlock);
        }

        private void state(XmlElement stateElement, StateImpl state, ProcessBlockImpl processBlock)
        {
            this.node(stateElement, state, processBlock);

            IEnumerator iter = xmlElement.GetChildElements("field").GetEnumerator();
            while (iter.MoveNext())
            {
                FieldImpl field = state.CreateField();
                this.field((XmlElement)iter.Current, field, processBlock);
            }
        }

        private void field(XmlElement fieldElement, FieldImpl field, ProcessBlockImpl processBlock) 
        {
            String attributeName = fieldElement.GetProperty("attribute");
            this.addUnresolvedReference(field, attributeName, processBlock, "attribute", typeof(IAttribute));
            String accessText = xmlElement.GetProperty("access");
            field.Access = FieldAccessHelper.fromText(accessText);
        }

        private void node(XmlElement nodeElement, NodeImpl node, ProcessBlockImpl processBlock) 
        {
            node.ArrivingTransitions = new ListSet();
            node.LeavingTransitions = new ListSet();

            IEnumerator iter = nodeElement.GetChildElements("transition").GetEnumerator();
            while (iter.MoveNext())
            {
                XmlElement transitionElement = (XmlElement)iter.Current;
                TransitionImpl transition = new TransitionImpl();
                transition.ProcessDefinition = ProcessDefinition;
                transition.From = node;
                if (node is JoinImpl)
                    this.joinTransition(transitionElement, transition, processBlock);
                else
                    this.transition(transitionElement, transition, processBlock);

                node.LeavingTransitions.Add(transition);
            }

         
            this.definitionObject(nodeElement, node, processBlock);

            //把所有的Node加到集合中
            this.addReferencableObject(node.Name, processBlock, typeof(INode), node);
        }

        private void delegation<T>(XmlElement delegationElement,DelegationImpl delegation) 
        {
            Type delegatingObjectClass = typeof(T);
            if (delegatingObjectClass == typeof(AttributeImpl))
            {
                String type = delegationElement.GetProperty("type");
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
                    delegation.ClassName = delegationElement.GetProperty("serializer");
                }
            }
            else if (delegatingObjectClass == typeof(FieldImpl))
            {
                delegation.ClassName = delegationElement.GetProperty("class");
            }
            else
            {
                delegation.ClassName = delegationElement.GetProperty("handler");
            }

            // parse the exception handler    
            String exceptionHandlerText = delegationElement.GetAttribute("on-exception");
            if ((Object)exceptionHandlerText != null)
            {
                delegation.ExceptionHandlingType = ExceptionHandlingTypeHelper.FromText(exceptionHandlerText);
            }

            // create the configuration string
            XmlElement configurationXml = new XmlElement("cfg");
            IEnumerator iter = delegationElement.GetChildElements("parameter").GetEnumerator();
            while (iter.MoveNext())
            {
                configurationXml.AddChild((XmlElement)iter.Current);
            }
            delegation.Configuration = configurationXml.ToString();
        }

        /// <summary>
        /// 一般Node的Transition To只能跳到同一個ProcessBlock的Node
        /// </summary>
        /// <param name="transitionElement"></param>
        /// <param name="transition"></param>
        /// <param name="processBlock"></param>
        private void transition(XmlElement transitionElement, TransitionImpl transition, ProcessBlockImpl processBlock) 
        {
            this.definitionObject(transitionElement, transition, processBlock);

            this.addUnresolvedReference(transition, transitionElement.GetProperty("to"), processBlock, "to", typeof(INode));
        }

        /// <summary>
        /// Join的Transition To會跳回上一層的ProcessBlock的Node
        /// </summary>
        /// <param name="transitionElement"></param>
        /// <param name="transition"></param>
        /// <param name="processBlock"></param>
        private void joinTransition(XmlElement transitionElement, TransitionImpl transition, ProcessBlockImpl processBlock)
        {
            this.definitionObject(transitionElement, transition, processBlock);

            this.addUnresolvedReference(transition, transitionElement.GetProperty("to"), processBlock.ParentBlock, "to", typeof(INode));
        }

        private void definitionObject(XmlElement definitionObjectElement, DefinitionObjectImpl definitionObject, ProcessBlockImpl processBlock)
        {
            definitionObject.Actions = new ArrayList();
            definitionObject.Name = definitionObjectElement.GetProperty("name");
            definitionObject.Description = definitionObjectElement.GetProperty("description");

            IEnumerator iter = definitionObjectElement.GetChildElements("action").GetEnumerator();
            while (iter.MoveNext())
            {
                XmlElement actionElement = (XmlElement)iter.Current;
                ActionImpl action = new ActionImpl();
                action.DefinitionObjectId = definitionObject.Id;
                this.action(actionElement, action);
                definitionObject.Actions.Add(action);
            }
        }

        private void action(XmlElement actionElement, ActionImpl actionImpl) 
        {
            actionImpl.EventType = EventTypeHelper.fromText(actionElement.GetAttribute("event"));
            DelegationImpl delegation = new DelegationImpl();
            delegation.ProcessDefinition = ProcessDefinition;
            actionImpl.ActionDelegation = delegation;
            this.delegation<ActionImpl>(actionElement, delegation);
        }

        private void addReferencableObject(String name, ProcessBlockImpl scope, Type type, Object referencableObject)
        {
            ReferencableObject referencableObjectScope = new ReferencableObject(scope, type);
            if (referencableObjects.ContainsKey(referencableObjectScope) == false)
            {
                referencableObjects[referencableObjectScope] = new Dictionary<string, object>();
            }

            var referenceObjectsByScope = this.referencableObjects[referencableObjectScope];
            referenceObjectsByScope.Add(name, referencableObject);
        }

        /// <summary>
        /// Transition要找to的Node，Field要找attribute指定的Attribute
        /// </summary>
        /// <param name="referencingObject"></param>
        /// <param name="destinationName"></param>
        /// <param name="destinationScope"></param>
        /// <param name="property"></param>
        /// <param name="destinationType"></param>
        private void addUnresolvedReference(Object referencingObject, String destinationName, ProcessBlockImpl destinationScope, String property, Type destinationType)
        {
            unresolvedReferences.Add(new UnresolvedReference(referencingObject, destinationName, destinationScope, property, destinationType));
        }

        private void resolveReferences()
        {
            IEnumerator iter = unresolvedReferences.GetEnumerator();
            while (iter.MoveNext())
            {
                UnresolvedReference unresolvedReference = (UnresolvedReference)iter.Current;

                Object referencingObject = unresolvedReference.ReferencingObject;
                String referenceDestinationName = unresolvedReference.DestinationName;
                ProcessBlockImpl scope = unresolvedReference.DestinationScope;
                String property = unresolvedReference.Property;

                Object referencedObject = FindInScope(unresolvedReference, unresolvedReference.DestinationScope);
                if (referencedObject == null)
                {
                    //AddError("failed to deploy process archive : couldn't resolve " + property + "=\"" + referenceDestinationName + "\" from " + referencingObject + " in scope " + scope);
                }
                else
                {
                    if (referencingObject is TransitionImpl)
                    {
                        if (property.Equals("to"))
                        {
                            TransitionImpl transition = (TransitionImpl)referencingObject;
                            transition.To = (NodeImpl)referencedObject;
                        }
                    }
                    if (referencingObject is FieldImpl)
                    {
                        if (property.Equals("attribute"))
                        {
                            FieldImpl field = (FieldImpl)referencingObject;
                            field.Attribute = (AttributeImpl)referencedObject;
                        }
                    }
                }
            }
        }

        private Object FindInScope(UnresolvedReference unresolvedReference, ProcessBlockImpl scope)
        {
            Object referencedObject = null;

            if (scope != null)
            {
                ReferencableObject referenceType = new ReferencableObject(scope, unresolvedReference.DestinationType);

                if ((referencableObjects.ContainsKey(referenceType) == true))
                {
                    IDictionary referencables = (IDictionary)referencableObjects[referenceType];
                    if (referencables != null && referencables.Contains(unresolvedReference.DestinationName))
                        referencedObject = referencables[unresolvedReference.DestinationName];
                }
                else
                {
                    referencedObject = FindInScope(unresolvedReference, scope.ParentBlock);
                }
            }

            return referencedObject;
        }
    }
}
