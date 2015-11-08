using System;
using System.Collections;
using NetBpm.Util.Client;

namespace NetBpm.Workflow.Definition.EComp
{
	public interface IProcessDefinitionService
	{
		/// <summary> loads a process archive into the NetBpm engine.
		/// The contents of the process archive is parsed and stored in the database.
		/// A process archive contains one process definition.
		/// If a process definition with the same name already exists in the NetBpm-engine,
		/// the added definition will get a new version number (one higher then the highest
		/// existing version number for that name)
		/// </summary>
		/// <param name="processArchiveBytes">must be a byte-array that contains a process archive.  
		/// @throws JpdlException if the processArchiveStream does not contain a
		/// valid process archive. 
		/// </param>
		//@portme
		void DeployProcessArchive(byte[] processArchiveBytes);

		/// <summary> collects the highest version of every {@link ProcessDefinition}. Those are the
		/// {@link ProcessDefinition}s from which a user must choose one to start.
		/// See also <a href="http://netbpm.org/docs/usersmanual.html#versioning">versioning</a>.
		/// </summary>
		/// <returns> a Collection of {@link NetBpm.Workflow.Definition.IProcessDefinition}s.  
		/// For each distinct process-definition-name it will return one ProcessDefinition : the 
		/// one with the highest version number.
		/// </returns>
		IList GetProcessDefinitions();

		/// <summary> collects the highest version of every {@link ProcessDefinition}. Those are the
		/// {@link ProcessDefinition}s from which a user must choose one to start.
		/// </summary>
		/// <param name="relations">specifies which {@link Relations} should be resolved in the 
		/// returned {@link NetBpm.Workflow.Definition.IProcessDefinition}s
		/// </param>
		/// <returns> a Collection of {@link NetBpm.Workflow.Definition.IProcessDefinition}s.  
		/// For each distinct process-definition-name it will return one ProcessDefinition : the 
		/// one with the highest version number.
		/// </returns>
		IList GetProcessDefinitions(Relations relations);


		/// <summary> gets the latest version of the {@link NetBpm.Workflow.Definition.IProcessDefinition} with the given name.</summary>
		IProcessDefinition GetProcessDefinition(String processDefinitionName);

		/// <summary> gets the latest version of the {@link NetBpm.Workflow.Definition.IProcessDefinition} with the given name.</summary>
		/// <param name="relations">specifies which {@link Relations} should be resolved in the 
		/// returned {@link NetBpm.Workflow.Definition.IProcessDefinition}
		/// </param>
		IProcessDefinition GetProcessDefinition(String processDefinitionName, Relations relations);

		/// <summary> gets a specific version of a {@link NetBpm.Workflow.Definition.IProcessDefinition}.</summary>
		IProcessDefinition GetProcessDefinition(Int64 processDefinitionId);

		/// <summary> gets a specific version of a {@link NetBpm.Workflow.Definition.IProcessDefinition}.</summary>
		/// <param name="relations">specifies which {@link Relations} should be resolved in the 
		/// returned {@link NetBpm.Workflow.Definition.IProcessDefinition}
		/// </param>
		IProcessDefinition GetProcessDefinition(Int64 processDefinitionId, Relations relations);

		IList GetAllProcessDefinitions();

		IList GetAllProcessDefinitions(Relations relations);
	}
}