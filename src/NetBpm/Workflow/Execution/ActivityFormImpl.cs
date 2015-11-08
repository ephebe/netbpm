using System;
using System.Collections;
using log4net;
using NetBpm.Workflow.Definition;

namespace NetBpm.Workflow.Execution.Impl
{
	public class ActivityFormImpl : IActivityForm
	{
		private IFlow _flow = null;
		private IProcessDefinition _processDefinition = null;
		private IProcessInstance _processInstance = null;
		private IActivityState _activityState = null;
		private IList _fields = null;
		private IDictionary _attributeValues = null;
		private IList _transitionNames = null;
		private static readonly ILog log = LogManager.GetLogger(typeof (ActivityFormImpl));

		public IFlow Flow
		{
			get { return _flow; }
		}

		public IProcessDefinition ProcessDefinition
		{
			get { return _processDefinition; }
		}

		public IProcessInstance ProcessInstance
		{
			get { return _processInstance; }

		}

		public IActivityState Activity
		{
			get { return _activityState; }
		}

		public IList Fields
		{
			get { return _fields; }
		}

		public IDictionary AttributeValues
		{
			get { return _attributeValues; }
		}

		public IList TransitionNames
		{
			get { return _transitionNames; }
		}

		public ActivityFormImpl(FlowImpl flow, IList fields, IDictionary attributeValues)
		{
			this._flow = flow;
			this._processInstance = flow.ProcessInstance;
			this._processDefinition = _processInstance.ProcessDefinition;
			this._activityState = (IActivityState) flow.Node;
			this._fields = fields;
			this._attributeValues = attributeValues;
			InitTransitionNames(flow.Node);
		}

		public ActivityFormImpl(IProcessDefinition processDefinition, IList fields, IDictionary attributeValues)
		{
			this._processDefinition = processDefinition;
			this._activityState = (IActivityState) processDefinition.StartState;
			this._fields = fields;
			this._attributeValues = attributeValues;
			InitTransitionNames(processDefinition.StartState);
		}

		private void InitTransitionNames(INode node)
		{
			this._transitionNames = new ArrayList();
			IEnumerator iter = node.LeavingTransitions.GetEnumerator();
			while (iter.MoveNext())
			{
				ITransition transition = (ITransition) iter.Current;
				if (transition.Name != null)
				{
					this._transitionNames.Add(transition.Name);
				}
			}

			log.Debug("created following activity form...");
			log.Debug("  flow: " + _flow);
			log.Debug("  processDefinition: " + _processDefinition);
			log.Debug("  processInstance: " + _processInstance);
			log.Debug("  activityState: " + _activityState);
			log.Debug("  fields: " + _fields);
			log.Debug("  attributeValues: " + _attributeValues);
			log.Debug("  transitionNames: " + _transitionNames);
		}

		public override String ToString()
		{
			return "activityForm[" + _activityState.Name + "]";
		}
	}
}