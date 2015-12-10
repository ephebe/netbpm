using Castle.Facilities.NHibernateIntegration;
using NetBpm.Util.DB;
using NetBpm.Util.EComp;
using NetBpm.Util.Xml;
using NetBpm.Util.Zip;
using NetBpm.Workflow.Definition.Impl;
using NHibernate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Definition
{
    public class ProcessDefinitionApplicationService
    {
        ProcessDefinitionRepository repository = null;
        public ProcessDefinitionApplicationService()
        {
            repository = ProcessDefinitionRepository.Instance;
        }

        public void DeployProcessArchive(ParFile parfile)
        {
            ProcessDefinitionBuildService buildService = new ProcessDefinitionBuildService(parfile.ProcessDefinition);
            ProcessDefinitionImpl processDefinition =  buildService.BuildProcessDefinition();

            using (ISession session = NHibernateHelper.OpenSession())
            {
                DbSession nhSession = new DbSession(session);
                repository.Save(processDefinition, nhSession);
            }

        }

        public IList<IProcessDefinition> GetProcessDefinitions()
        {
            throw new NotImplementedException();
        }

        public IList<IProcessDefinition> GetProcessDefinitions(Util.Client.Relations relations)
        {
            throw new NotImplementedException();
        }

       
    }
}
