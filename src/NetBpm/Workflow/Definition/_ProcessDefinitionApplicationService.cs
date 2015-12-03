using NetBpm.Util.Xml;
using NetBpm.Util.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Definition
{
    public class ProcessDefinitionApplicationService 
    {
        public void DeployProcessArchive(byte[] processArchiveBytes)
        {
            Stream processArchiveStream = new MemoryStream(processArchiveBytes);
            IDictionary<string, byte[]> entries = null;
            entries = ZipUtility.ReadEntries(processArchiveStream);

            XmlElement xmlElement = getXmlElementFromBytes(entries["processdefinition.xml"]);


        }

        public IList<IProcessDefinition> GetProcessDefinitions()
        {
            throw new NotImplementedException();
        }

        public IList<IProcessDefinition> GetProcessDefinitions(Util.Client.Relations relations)
        {
            throw new NotImplementedException();
        }

        private XmlElement getXmlElementFromBytes(byte[] processDefinitionXml)
        {
            XmlElement xmlElement = null;

            Stream processDefinitionInputStream = new MemoryStream(processDefinitionXml);
            XmlParser xmlParser = new XmlParser(processDefinitionInputStream);
            xmlElement = xmlParser.Parse();
            return xmlElement;
        }
    }
}
