using System;
using log4net;

namespace NetBpm.Workflow.Definition.Impl
{
	public class UnresolvedReference
	{
		private Object _referencingObject = null;
		private String _destinationName = null;
		private ProcessBlockImpl _destinationScope = null;
		private Type _destinationType = null;
		private String _property = null;

		private static readonly ILog log = LogManager.GetLogger(typeof (UnresolvedReference));

		public Object ReferencingObject
		{
			get { return _referencingObject; }

		}

		public String DestinationName
		{
			get { return _destinationName; }

		}

		public ProcessBlockImpl DestinationScope
		{
			get { return _destinationScope; }

		}

		public String Property
		{
			get { return _property; }

		}

		public Type DestinationType
		{
			get { return _destinationType; }

		}

		public UnresolvedReference(Object referencingObject, String destinationName, ProcessBlockImpl destinationScope, String property, Type destinationType)
		{
			this._referencingObject = referencingObject;
			this._destinationName = destinationName;
			this._destinationScope = destinationScope;
			this._property = property;
			this._destinationType = destinationType;
		}

		public override String ToString()
		{
			return "unresolve-reference from " + _referencingObject + ", property " + _property + " to destination " + _destinationName + " in scope " + _destinationScope;
		}
	}
}