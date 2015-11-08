using System;
using log4net;
using NetBpm.Workflow.Delegation;

namespace NetBpm.Workflow.Log.Impl
{
	public class DelegateCallImpl : LogDetailImpl, IDelegateCall
	{
		[NonSerialized()] private IDelegation _delegation = null;
		private String _interfaceClassName = null;
		private static readonly log4net.ILog log = LogManager.GetLogger(typeof (DelegateCallImpl));

        public virtual String InterfaceClassName
		{
			get { return this._interfaceClassName; }
			set { this._interfaceClassName = value; }
		}

        public virtual IDelegation Delegation
		{
			get { return this._delegation; }
			set { this._delegation = value; }
		}

		public DelegateCallImpl()
		{
		}

		public DelegateCallImpl(IDelegation delegation, Type interfaceClass)
		{
			this._delegation = delegation;
			this._interfaceClassName = interfaceClass.FullName;
			if (this._interfaceClassName.StartsWith("interface "))
			{
				this._interfaceClassName = this._interfaceClassName.Substring(10);
			}
		}

        public virtual Type GetInterface()
		{
			Type clazz = null;
			//@portme
/*			try
			{
				clazz = typeof(DelegateCallImpl).getClassLoader().loadClass(interfaceClassName);
			}
			catch (System.Exception e)
			{
				log.Error("", e);
			}*/
			return clazz;
		}
	}
}