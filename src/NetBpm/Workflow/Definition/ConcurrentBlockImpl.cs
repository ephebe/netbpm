using System;
using NetBpm.Util.Xml;

namespace NetBpm.Workflow.Definition.Impl
{
	public class ConcurrentBlockImpl : ProcessBlockImpl, IConcurrentBlock
	{
		private IJoin _join = null;
		private IFork _fork = null;

		public virtual IFork Fork
		{
			set { _fork = value; }
			get { return _fork; }
		}

        public virtual IJoin Join
		{
			set { _join = value; }
			get { return _join; }
		}

		public ConcurrentBlockImpl()
		{
		}

		public ConcurrentBlockImpl(IProcessDefinition processDefinition) : base(processDefinition)
		{
		}

        public virtual ForkImpl CreateFork()
		{
			ForkImpl fork = new ForkImpl(_processDefinition);
			this._fork = fork;
			this.AddNode(fork);
			return fork;
		}

        public virtual JoinImpl CreateJoin()
		{
			JoinImpl join = new JoinImpl(_processDefinition);
			this._join = join;
			this.AddNode(join);
			return join;
		}

		public override void ReadProcessData(XmlElement xmlElement, ProcessDefinitionBuildContext creationContext)
		{
			// the fork and join are created in the parent block but we'll add them
			// also as referencable objects to this block's scope a few lines below
			_parentBlock = creationContext.ProcessBlock;
			this._join = new JoinImpl();
			this._fork = new ForkImpl();

			creationContext.ProcessBlock = this;
			base.ReadProcessData(xmlElement, creationContext);
			XmlElement joinElement = xmlElement.GetChildElement("join");
			creationContext.Check((joinElement != null), "element join is missing");
			XmlElement forkElement = xmlElement.GetChildElement("fork");
			creationContext.Check((joinElement != null), "element fork is missing");
			((JoinImpl) this._join).ReadProcessData(joinElement, creationContext);
			((ForkImpl) this._fork).ReadProcessData(forkElement, creationContext);
			creationContext.ProcessBlock = _parentBlock;

			this._nodes.Add(_join);
			this._nodes.Add(_fork);

			// add the fork and join as referencable objects in the proper scope
			creationContext.AddReferencableObject(_fork.Name, _parentBlock, typeof (INode), _fork);
			creationContext.AddReferencableObject(_join.Name, this, typeof (INode), _join);
		}

		public override void Validate(ValidationContext validationContext)
		{
			// the fork & join are required
			validationContext.Check((_fork != null), "a concurrent block does not have a fork");
			validationContext.Check((_join != null), "a concurrent block does not have a join");

			validationContext.PushScope("in concurrent-block [" + _fork.Name + "|" + _join.Name + "]");

			base.Validate(validationContext);

			validationContext.PopScope();
		}

		public override String ToString()
		{
			return "ConcurrentBlockImpl[" + _fork + "|" + _join + "]";
		}
	}
}