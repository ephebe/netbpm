using System.Collections;
using NetBpm.Workflow.Definition;

namespace NetBpm.Workflow.Execution
{
	/// <summary> contains all data necessary for the web-application to create a
	/// html-form that enables a human-user to perform an {@link ActivityState}. 
	/// </summary>
	///@portme ISerializable
	public interface IActivityForm //: System.Runtime.Serialization.ISerializable
	{
		/// <summary> gets the Flow which is waiting in the {@link ActivityState} to be performed.</summary>
		IFlow Flow { get; }

		/// <summary> gets the {@link ProcessDefinition} of this {@link ActivityState}.</summary>
		IProcessDefinition ProcessDefinition { get; }

		/// <summary> gets the {@link ActivityState} for which all this fuzz is about.</summary>
		IActivityState Activity { get; }

		/// <summary> contains the information to build up the form.</summary>
		/// <returns> a List of {@link Fields} that is ordered in the same
		/// way as in the web-interface.xml file.  
		/// </returns>
		IList Fields { get; }

		/// <summary> for attributes that are readable in this activity, 
		/// the returned object will map attribute-names to attribute-values.
		/// </summary>
		IDictionary AttributeValues { get; }

		/// <summary> gets the list of all named transitions leaving the {@link ActivityState}.</summary>
		IList TransitionNames { get; }
	}
}