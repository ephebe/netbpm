using System;
using System.Collections;
using Iesi.Collections;
using log4net;
using NetBpm.Util.Client;
using NetBpm.Util.DB;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.Impl;
using NetBpm.Workflow.Delegation;
using NetBpm.Workflow.Log.Impl;
using NetBpm.Workflow.Organisation;
using NetBpm.Workflow.Organisation.EComp;
using NetBpm.Workflow.Scheduler;
using NetBpm.Workflow.Scheduler.EComp;
using NHibernate.Type;

namespace NetBpm.Workflow.Execution.Impl
{
	public class ExecutionContextImpl : IAssignmentContext, IDecisionContext, IForkContext, IActionContext, IJoinContext, IProcessInvocationContext, ITaskContext
	{
		private DbSession _dbSession = null;
		private IOrganisationService _organisationComponent = null;
		private String _actorId = null;
		private String _previousActorId = null;
		private ProcessDefinitionImpl _processDefinition = null;
		private ProcessInstanceImpl _processInstance = null;
		private FlowImpl _flow = null;
		private NodeImpl _node = null;
		private LogImpl _logField = null;
		private IDictionary _configuration = null;
		private IActionContext _invokedProcessContext = null;
		private ArrayList _assignedFlows = null;
		private IList _forkedFlows = null;
        private TransitionRepository transitionRepository = TransitionRepository.Instance;
        private FlowRepository flowRepository = FlowRepository.Instance;
        private AttributeRepository attributeRepository = AttributeRepository.Instance;
		private static readonly ServiceLocator serviceLocator;
		private static readonly ILog log = LogManager.GetLogger(typeof (ExecutionContextImpl));

		static ExecutionContextImpl()
		{
			serviceLocator = ServiceLocator.Instance;
		}

		public DbSession DbSession
		{
			get { return _dbSession; }
		}

		public String ActorId
		{
			get { return this._actorId; }
		}

		public IActor Actor
		{
			get { return _organisationComponent.FindActorById(_actorId); }
		}

		public String PreviousActorId
		{
			get { return this._previousActorId; }
		}

		public LogImpl Log
		{
			get { return this._logField; }
			set { this._logField = value; }
		}

		public ArrayList AssignedFlows
		{
			get { return this._assignedFlows; }
			set { this._assignedFlows = value; }
		}

		public IList ForkedFlows
		{
			get { return this._forkedFlows; }
			set { this._forkedFlows = value; }
		}

		/// <summary> the default constructor is necessary for the ActorExpressionTest.</summary>
		public ExecutionContextImpl()
		{
		}

		public ExecutionContextImpl(String actorId, DbSession dbSession, IOrganisationService organisationComponent)
		{
			this._actorId = actorId;
			this._dbSession = dbSession;
			this._organisationComponent = organisationComponent;
		}

		public ExecutionContextImpl(String actorId, FlowImpl flow, DbSession dbSession, IOrganisationService organisationComponent)
		{
			this._actorId = actorId;
			this._flow = flow;
			this._processInstance = (ProcessInstanceImpl) flow.ProcessInstance;
			this._processDefinition = (ProcessDefinitionImpl) _processInstance.ProcessDefinition;
			this._dbSession = dbSession;
			this._organisationComponent = organisationComponent;
			this._assignedFlows = new ArrayList();
		}

		public ExecutionContextImpl(String actorId, ProcessInstanceImpl processInstance, DbSession dbSession, IOrganisationService organisationComponent)
		{
			this._actorId = actorId;
			this._processInstance = processInstance;
			this._processDefinition = (ProcessDefinitionImpl) processInstance.ProcessDefinition;
			this._dbSession = dbSession;
			this._organisationComponent = organisationComponent;
			this._assignedFlows = new ArrayList();
		}

		public virtual IOrganisationService GetOrganisationComponent()
		{
			return _organisationComponent;
		}

		public void SetActorAsPrevious()
		{
			this._previousActorId = this._actorId;
			this._actorId = null;
			this._flow.ActorId = null;
		}

		public virtual IActor GetPreviousActor()
		{
			return _organisationComponent.FindActorById(_previousActorId);
		}

		public IProcessDefinition GetProcessDefinition()
		{
			return this._processDefinition;
		}

		public void SetProcessDefinition(IProcessDefinition processDefinition)
		{
			this._processDefinition = (ProcessDefinitionImpl) processDefinition;
		}

		public virtual IProcessInstance GetProcessInstance()
		{
			return this._processInstance;
		}

		public void SetProcessInstance(IProcessInstance processInstance)
		{
			this._processInstance = (ProcessInstanceImpl) processInstance;
		}

		public IFlow GetFlow()
		{
			return this._flow;
		}

		public void SetFlow(IFlow flow)
		{
			this._flow = (FlowImpl) flow;
		}

		public INode GetNode()
		{
			return this._node;
		}

		public void SetNode(INode node)
		{
			this._node = (NodeImpl) node;
		}

		public virtual IDictionary GetConfiguration()
		{
			return this._configuration;
		}

		public void SetConfiguration(IDictionary configuration)
		{
			this._configuration = configuration;
		}

		public IActionContext GetInvokedProcessContext()
		{
			return this._invokedProcessContext;
		}

		public void SetInvokedProcessContext(IActionContext invokedProcessContext)
		{
			this._invokedProcessContext = invokedProcessContext;
		}

		public virtual Object GetAttribute(String name)
		{
			AttributeInstanceImpl attributeInstance = FindAttributeInstanceInScope(name);
			if (attributeInstance != null)
			{
				// attribute might not be available (a warning should have been logged previosly)
				return attributeInstance.GetValue();
			}
			return null;
		}

		public void SetAttribute(String name, Object valueObject)
		{
			AttributeInstanceImpl attributeInstance = FindAttributeInstanceInScope(name);
			if (attributeInstance != null)
			{
				// ignore if cannot find attribute instance (warning should have been logged prviously)
				attributeInstance.SetValue(valueObject);
				this.AddLogDetail(new AttributeUpdateImpl(attributeInstance));
			}
		}

        //private const String queryFindAttributeInstanceByName = "select ai " +
        //    "from ai in class NetBpm.Workflow.Execution.Impl.AttributeInstanceImpl, " +
        //    "     f in class NetBpm.Workflow.Execution.Impl.FlowImpl " +
        //    "where ai.Scope = f.id " + "  and ai.Attribute.Name = ? " +
        //    "  and f.id = ? ";

		private AttributeInstanceImpl FindAttributeInstanceInScope(String attributeName)
		{
			AttributeInstanceImpl attributeInstance = null;
			FlowImpl scope = this._flow;
			while (attributeInstance == null)
			{
                IList attributes = attributeRepository.FindAttributeInstanceByName(attributeName, scope.Id, _dbSession);
                IEnumerator iter = attributes.GetEnumerator();
				if (iter.MoveNext())
				{
					attributeInstance = (AttributeInstanceImpl) iter.Current;
					if (iter.MoveNext())
					{
						throw new NetBpm.Util.DB.DbException("duplicate value");
					}
				}
				else
				{
					if (!scope.IsRootFlow())
					{
						scope = (FlowImpl) scope.Parent;
					}
					else
					{
						//throw new RuntimeException( "couldn't find attribute-instance '" + attributeName + "' in scope of flow '" + this.flow + "'" ); 
						// log a warning message (indicate that attribute supplied is not defined in attribute-instance in db)
						log.Warn("couldn't find attribute-instance '" + attributeName + "' in scope of flow '" + this._flow + "'");
						break;
					}
				}
			}
			return attributeInstance;
		}

		public void AddLog(String msg)
		{
			this.AddLogDetail(new MessageImpl(msg));
		}

		public void CreateLog(EventType eventType)
		{
			this._logField = _flow.CreateLog(eventType);
		}

		public void CreateLog(String actorId, EventType eventType)
		{
			this._logField = _flow.CreateLog(actorId, eventType);
		}

		public void AddLogDetail(LogDetailImpl logDetail)
		{
			if (_logField != null)
			{
				this._logField.Details.Add(logDetail);
				logDetail.Log = this._logField;
			}
		}

		public void Schedule(Job job)
		{
			Schedule(job, null);
		}

		public void Schedule(Job job, String reference)
		{
			ISchedulerSessionLocal schedulerComponent = null;
			try
			{
				schedulerComponent = (ISchedulerSessionLocal) serviceLocator.GetService(typeof (ISchedulerSessionLocal));
				job.Context = _processInstance.RootFlow;
				schedulerComponent.ScheduleJob(job, reference);
				serviceLocator.Release(schedulerComponent);
			}
			finally
			{
				serviceLocator.Release(schedulerComponent);
			}
		}

		public void ForkFlow(String transitionName)
		{
			ForkFlow(transitionName, null);
		}

        //private const String queryFindLeavingTransitionByName = "select t " +
        //    "from t in class NetBpm.Workflow.Definition.Impl.TransitionImpl, " +
        //    "     n in class NetBpm.Workflow.Definition.Impl.NodeImpl " +
        //    "where n.id = ? " +
        //    "  and t.From.id = n.id " +
        //    "  and t.Name = ? ";

		public void ForkFlow(String transitionName, IDictionary attributeValues)
		{
			// find the transition
			TransitionImpl transition = null;
			try
			{
                transition = transitionRepository.FindLeavingTransitionByName(_node.Id, transitionName,_dbSession);
			}
			catch (NotUniqueException e)
			{
				throw new SystemException("transition with name '" + transitionName + "' was not found for creating sub-flow on fork '" + _node.Name + "' : " + e.Message);
			}
			ForkFlow(transition, attributeValues);
		}

		public void ForkFlow(TransitionImpl transition, IDictionary attributeValues)
		{
			// create the subflow 
			FlowImpl subFlow = new FlowImpl(transition.Name, _flow, (ProcessBlockImpl) _node.ProcessBlock);
			_flow.Children.Add(subFlow);

			// save it 
			_dbSession.Save(subFlow);

			// store the attributeValues
			this._flow = subFlow;
			StoreAttributeValues(attributeValues);
			this._flow = (FlowImpl) this._flow.Parent;

			// add the transition and the flow to the set of created sub-flows
			this._forkedFlows.Add(new ForkedFlow(transition, subFlow));
		}

		public void StoreRole(String authenticatedActorId, ActivityStateImpl activityState)
		{
			String role = activityState.ActorRoleName;
			if ((Object) role != null)
			{
				IActor authenticatedActor = OrganisationUtil.Instance.GetActor(authenticatedActorId);
				log.Debug("assigning " + authenticatedActor + " to role " + role);
				SetAttribute(role, authenticatedActor);
			}
		}

		public void StoreAttributeValues(IDictionary attributeValues)
		{
			if (attributeValues != null)
			{
				// loop over all provided attributeValues
				IEnumerator iter = attributeValues.GetEnumerator();
				while (iter.MoveNext())
				{
					DictionaryEntry entry = (DictionaryEntry) iter.Current;
					String attributeName = (String) entry.Key;
					// and store it
					SetAttribute(attributeName, entry.Value);
				}
			}
		}

		public void CheckAccess(IDictionary attributeValues, StateImpl state)
		{
			IDictionary fields = new Hashtable();

			// first we check if a value is supplied for all required fields
			IEnumerator iter = state.Fields.GetEnumerator();
			log.Debug(iter);
			while (iter.MoveNext())
			{
				FieldImpl field = (FieldImpl) iter.Current;
				String attributeName = field.Attribute.Name;
				fields[attributeName] = field;

				// if a field is found required and no attribute value is supplied throw 
				// RequiredFieldException 
				log.Debug(field);
				log.Debug(field.Access);
				if ((FieldAccessHelper.IsRequired(field.Access)) && (attributeValues == null))
				{
					throw new RequiredFieldException(field);
				}
				// OR
				// if field is found required and attribute value of it is not available
				// throw RequiredFieldException
				if ((FieldAccessHelper.IsRequired(field.Access)) && (attributeValues != null) && (!attributeValues.Contains(attributeName)))
				{
					throw new RequiredFieldException(field);
				}
			}

			// then we check if the access of all supplied values is writable
			IList attributeNamesToBeRemoved = new ArrayList(); // store attribute name of attribute to be removed
			if (attributeValues != null)
			{
				iter = attributeValues.GetEnumerator();
				while (iter.MoveNext())
				{
					DictionaryEntry entry = (DictionaryEntry) iter.Current;
					String attributeName = (String) entry.Key;

					FieldImpl field = (FieldImpl) fields[attributeName];
					if ((field != null) && (!FieldAccessHelper.IsWritable(field.Access)))
					{
						log.Warn("ignoring attributeValue for unwritable attribute '" + attributeName + "'");
						// commented out cause will result in ConcurrentModificationException
						// instead copy its attribute name and remove later OR do a deep copy of the
						// attributeValues so there is one set that gets iterated and another that 
						//gets deleted???
						//attributeValues.remove( attributeName );
						attributeNamesToBeRemoved.Add(attributeName);
					}
				}
				// now removed collected to be removed attribute
				IEnumerator itr = attributeNamesToBeRemoved.GetEnumerator();
				while (itr.MoveNext())
				{
					String an = (String) itr.Current;
					attributeValues.Remove(an);
				}
			}
		}

        //private const String queryFindConcurrentFlows = "select cf " +
        //    "from cf in class NetBpm.Workflow.Execution.Impl.FlowImpl," +
        //    "     f in class NetBpm.Workflow.Execution.Impl.FlowImpl " +
        //    "where f.id = ? " + "  and cf.Parent = f.Parent " +
        //    "  and cf.EndNullable is null " + "  and cf.id <> f.id ";

        public IList GetOtherActiveConcurrentFlows()
        {
            //return _dbSession.Find(queryFindConcurrentFlows, _flow.Id, DbType.LONG);
            return flowRepository.GetOtherActiveConcurrentFlows( _flow.Id,_dbSession);
        }

        //private const String queryFindTransitionByName = "select t " +
        //    "from t in class NetBpm.Workflow.Definition.Impl.TransitionImpl, " +
        //    "     s in class NetBpm.Workflow.Definition.Impl.StateImpl " +
        //    "where t.From = s.id " +
        //    "  and t.Name = ? " +
        //    "  and s.id = ? ";

        ///* package private */

        //internal virtual TransitionImpl GetTransition(String transitionName, StateImpl state, DbSession dbSession)
        //{
        //    TransitionImpl transition = null;
        //    if ((Object) transitionName != null)
        //    {
        //        Object[] values = new Object[] {transitionName, state.Id};
        //        IType[] types = new IType[] {DbType.STRING, DbType.LONG};
        //        transition = (TransitionImpl) dbSession.FindOne(queryFindTransitionByName, values, types);
        //    }
        //    else
        //    {
        //        ISet leavingTransitions = state.LeavingTransitions;
        //        if (leavingTransitions.Count == 1)
        //        {
        //            IEnumerator transEnum = leavingTransitions.GetEnumerator();
        //            transEnum.MoveNext();
        //            transition = (TransitionImpl) transEnum.Current;
        //        }
        //        else
        //        {
        //            throw new SystemException("no transitionName was specified : this is only allowed if the state (" + state.Name + ") has exactly 1 leaving transition (" + leavingTransitions.Count + ")");
        //        }
        //    }
        //    return transition;
        //}
	}
}