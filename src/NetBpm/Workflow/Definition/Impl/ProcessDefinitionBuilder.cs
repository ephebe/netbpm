using System;
using System.Collections;
using NetBpm.Util.DB;
using System.Collections.Generic;
using NetBpm.Util.Xml;

namespace NetBpm.Workflow.Definition.Impl
{
    public class ProcessDefinitionBuildContext : ValidationContext,IProcessDefinitionBuilder
	{
		// invariable members  
		private ProcessDefinitionImpl _processDefinition = null;
		private IDictionary<string,byte[]> _entries = null;
		private DbSession _dbSession = null;

		// context members
		private ProcessBlockImpl _processBlock = null;
		private ProcessBlockImpl _transitionDestinationScope = null;
		private DefinitionObjectImpl _definitionObject = null;
		private StateImpl _state = null;
		private NodeImpl _node = null;
		private Object _delegatingObject = null;
		private int _index = - 1;

		private IDictionary _referencableObjects;
		private IList _unresolvedReferences;

//		static private IList _errors = new ArrayList();
//		static private IList _scope = new ArrayList();
		private void InitBlock()
		{
			_referencableObjects = new Hashtable();
			_unresolvedReferences = new ArrayList();
		}

		public ProcessDefinitionImpl ProcessDefinition
		{
			get { return this._processDefinition; }
		}

		public DbSession DbSession
		{
			get { return this._dbSession; }
		}

		public ProcessBlockImpl ProcessBlock
		{
			get { return this._processBlock; }
			set { this._processBlock = value; }
		}

		public ProcessBlockImpl TransitionDestinationScope
		{
			get { return this._transitionDestinationScope; }
			set { this._transitionDestinationScope = value; }
		}

		public DefinitionObjectImpl DefinitionObject
		{
			get { return this._definitionObject; }
			set { this._definitionObject = value; }
		}

		public StateImpl State
		{
			get { return this._state; }
			set { this._state = value; }
		}

		public NodeImpl Node
		{
			get { return this._node; }
			set { this._node = value; }

		}

		public Object DelegatingObject
		{
			get { return this._delegatingObject; }
			set { this._delegatingObject = value; }
		}

		public int Index
		{
			get { return this._index; }
			set { this._index = value; }
		}

		public IDictionary<string,byte[]> Entries
		{
			get { return _entries; }
		}

		public ProcessDefinitionBuildContext(ProcessDefinitionImpl processDefinition, IDictionary<string,byte[]> entries, DbSession dbSession)
		{
			InitBlock();
			this._processDefinition = processDefinition;
			this._processBlock = processDefinition;
			this._dbSession = dbSession;
			this._entries = entries;
		}

		public void IncrementIndex()
		{
			this._index++;
		}

		public void AddUnresolvedReference(Object referencingObject, String destinationName, ProcessBlockImpl destinationScope, String property, Type destinationType)
		{
			_unresolvedReferences.Add(new UnresolvedReference(referencingObject, destinationName, destinationScope, property, destinationType));
		}

		public void AddReferencableObject(String name, ProcessBlockImpl scope, Type type, Object referencableObject)
		{
			ReferencableObject referenceType = new ReferencableObject(scope, type);
			IDictionary referencables = (IDictionary) _referencableObjects[referenceType];
			if (referencables == null)
			{
				referencables = new Hashtable();
				_referencableObjects[referenceType] = referencables;
			}
			referencables[name] = referencableObject;
		}

		public void ResolveReferences()
		{
			IEnumerator iter = _unresolvedReferences.GetEnumerator();
			while (iter.MoveNext())
			{
				UnresolvedReference unresolvedReference = (UnresolvedReference) iter.Current;

				Object referencingObject = unresolvedReference.ReferencingObject;
				String referenceDestinationName = unresolvedReference.DestinationName;
				ProcessBlockImpl scope = unresolvedReference.DestinationScope;
				String property = unresolvedReference.Property;

				Object referencedObject = FindInScope(unresolvedReference, unresolvedReference.DestinationScope);
				if (referencedObject == null)
				{
					AddError("failed to deploy process archive : couldn't resolve " + property + "=\"" + referenceDestinationName + "\" from " + referencingObject + " in scope " + scope);
				}
				else
				{
					if (referencingObject is TransitionImpl)
					{
						if (property.Equals("to"))
						{
							TransitionImpl transition = (TransitionImpl) referencingObject;
							transition.To = (NodeImpl) referencedObject;
						}
					}
					if (referencingObject is FieldImpl)
					{
						if (property.Equals("attribute"))
						{
							FieldImpl field = (FieldImpl) referencingObject;
							field.Attribute = (AttributeImpl) referencedObject;
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

				IDictionary referencables = (IDictionary) _referencableObjects[referenceType];

				if ((referencables != null) && (referencables.Contains(unresolvedReference.DestinationName)))
				{
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