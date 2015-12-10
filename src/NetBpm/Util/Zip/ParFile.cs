using NetBpm.CustomException;
using NetBpm.Util.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetBpm.Util.Zip
{
    public class ParFile
    {
        private string path;
        private IDictionary<string, byte[]> entries;

        public ParFile(string path)
        {
            this.path = path;

            if (System.IO.File.Exists(path) == false) 
            {
                throw new FileNotFindException();
            }

            readXml(readFileIntoStream());
        }

        public Xml.XmlElement ProcessDefinition 
        {
            get 
            {
                
                return getXmlElementFromBytes(entries["processdefinition.xml"]);
            }
        }

        private byte[] readFileIntoStream() 
        {
            byte[] buffer = null;
            using (FileStream fs = File.OpenRead(path))
            {
                buffer = new byte[fs.Length];
                int numBytesToRead = (int)fs.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    int n = fs.Read(buffer, numBytesRead, numBytesToRead);

                    if (n == 0)
                        break;

                    numBytesRead += n;
                    numBytesToRead -= n;
                }
            }

            return buffer;
        }

        private void readXml(byte[] processArchiveBytes) 
        {
            Stream processArchiveStream = new MemoryStream(processArchiveBytes);
            IDictionary<string, byte[]> entries = null;
            entries = ZipUtility.ReadEntries(processArchiveStream);
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
