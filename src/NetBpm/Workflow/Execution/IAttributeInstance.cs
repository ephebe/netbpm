using System;
using NetBpm.Workflow.Definition;

namespace NetBpm.Workflow.Execution
{
	/// <summary> is a process-variable associated with one process instance.
	/// The value can be changed during the execution of the flow in following ways :
	/// <ul>
	/// <li><b>field input</b> : In the {@link ProcessDefinition}, 
	/// a {@link Field} associates an attribute with an {@link Activity}.  
	/// When an actor performs that {@link Activity}, it has to provide input 
	/// for the attribute. The value that is provided by the actor is stored in the 
	/// AttributeInstance.</li>
	/// <li><b>role association</b> : A special kind of AttributeInstances is the 
	/// role-attribute-instance.  They are specified by the actorName-property in 
	/// the activity. If an actor performs an activity, the attribute-instance with name 
	/// actorName is set to the actor.</li>
	/// </ul>
	/// </summary>
	public interface IAttributeInstance //: System.Runtime.Serialization.ISerializable
	{
		/// <summary> the meaningless primary-key for this object. </summary>
		Int64 Id { get; }

		/// <summary> gets the text as it is store in the database in serialized form.</summary>
		String ValueText { get; }

		/// <summary> is the {@link Attribute} of this AttributeInstance.</summary>
		IAttribute Attribute { get; }

		/// <summary> is the {@link Flow} to which this AttributeInstance is associated.</summary>
		IFlow Scope { get; }

		/// <summary> is the java-object-value for this AttributeInstance.</summary>
		Object GetValue();
	}
}