﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace NetBpm.Workflow.Definition
{
    public class XmlLoadService
    {
        public IProcessDefinition FetchProcessDefinition(XmlDocument xmlDocument) 
        {
            ProcessDefinitionBuildService buildService = new ProcessDefinitionBuildService(xmlDocument);
            buildService.Id();

        }
    }
}
