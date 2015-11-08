using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Definition.Impl
{
    internal class ReferencableObject
    {
        private Type _type;
        private ProcessBlockImpl _scope;

        public ProcessBlockImpl Scope
        {
            get { return _scope; }
            set { _scope = value; }
        }

        public Type Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public ReferencableObject(ProcessBlockImpl scope, Type type)
        {
            _type = type;
            _scope = scope;
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;

            ReferencableObject refObject = obj as ReferencableObject;
            if (refObject == null) return false;

            return (refObject.Type.Equals(this.Type)
                && refObject.Scope.Equals(this.Scope));
        }

        public override int GetHashCode()
        {
            return _type.GetHashCode();
        }
    }
}
