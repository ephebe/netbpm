using System;

namespace NetBpm.Workflow.Organisation.Impl
{
	public abstract class ActorImpl : IActor, IComparable
	{
		protected String _id = null;

        public virtual string Id
		{
			set { _id = value; }
			get { return _id; }
		}

		public abstract string Name { get; set; }

		public override String ToString()
		{
			String className = this.GetType().FullName;
			int to = className.Length;
			if (className.EndsWith("Impl"))
			{
				to = to - 4;
			}
			int from = className.LastIndexOf('.') + 1;
			className = className.Substring(from, (to) - (from));
			return className + "[" + _id + "]";
		}

		// equals
		public override bool Equals(Object object_Renamed)
		{
			bool isEqual = false;
			if ((object_Renamed != null) && (object_Renamed is ActorImpl))
			{
				ActorImpl actor = (ActorImpl) object_Renamed;
				if (((Object) this._id == null) && ((Object) actor._id == null))
				{
					isEqual = (this == actor);
				}
				else if (((Object) this._id != null) && ((Object) actor._id != null))
				{
					isEqual = this._id.Equals(actor._id);
				}
			}
			return isEqual;
		}

		// hashCode
		public override int GetHashCode()
		{
			int hashCode = 0;
			if ((Object) _id != null)
			{
				hashCode = _id.GetHashCode();
			}
			else
			{
				base.GetHashCode();
			}
			return hashCode;
		}

		// compareTo  
        public virtual Int32 CompareTo(Object object_Renamed)
		{
			Int32 difference = - 1;

			ActorImpl actor = (ActorImpl) object_Renamed;
			if ((actor != null) && ((Object) this._id != null) && ((Object) actor._id != null))
			{
				difference = this._id.CompareTo(actor._id);
			}
			else
			{
				throw new SystemException("can't compare two actors this(" + this + ") and object(" + object_Renamed + ")");
			}

			return difference;
		}
	}
}