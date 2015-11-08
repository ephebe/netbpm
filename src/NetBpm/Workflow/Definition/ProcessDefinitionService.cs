using System;
using System.Collections;
using NetBpm.Util.Client;
using NetBpm.Util.DB;
using NetBpm.Util.EComp;
using NetBpm.Workflow.Definition.Impl;
using Castle.Services.Transaction;
using Castle.Facilities.NHibernateIntegration;
using System.IO;
using log4net;
using ICSharpCode.SharpZipLib.Zip;
using NetBpm.Util.Xml;
using System.Collections.Generic;
using NetBpm.Util.Zip;
using Iesi.Collections;
using System.Reflection;
using NetBpm.Workflow.Delegation.Impl;

namespace NetBpm.Workflow.Definition.EComp.Impl
{
	/// <summary> reads the process-definition-jar-file (= .par) and
	/// stores all content in the database. 
	/// </summary>
	[Transactional]
	public class ProcessDefinitionService : NHSessionOpener, IProcessDefinitionService
	{
		private static readonly ProcessDefinitionRepository processDefinitionRepository = ProcessDefinitionRepository.Instance;
        private static readonly ILog log = LogManager.GetLogger(typeof(ProcessDefinitionService));

		public ProcessDefinitionService(ISessionManager sessionManager) : base(sessionManager)
		{
		}

		/// <summary> this method is exposed only by the remote-interface and not the local-interface.</summary>
		[Transaction(TransactionMode.Requires)]
		public virtual void DeployProcessArchive(byte[] processArchiveBytes)
		{
			DbSession dbSession = null;
			try
			{
				dbSession = OpenSession();
				DeployProcessArchive(processArchiveBytes, dbSession);
			}
			catch (NpdlException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new SystemException("uncaught exception : " + e.Message, e);
			}
		}

        public void DeployProcessArchive(byte[] processArchiveBytes, DbSession dbSession)
        {
            try
            {
                Stream mstream = new MemoryStream(processArchiveBytes);
                DeployProcessArchive(mstream, dbSession);
            }
            catch (IOException e)
            {
                throw new NpdlException("couldn't deploy process archive, the processArchiveBytes do not seem to be a valid par-file : " + e.Message, e);
            }
        }

        public void DeployProcessArchive(Stream processArchiveStream, DbSession dbSession)
        {
            log.Debug("reading process archive...");

            // construct an empty process definition
            ProcessDefinitionImpl processDefinition = new ProcessDefinitionImpl();

            // Then save the process definition
            // This is done so hibernate will assign an id to this object.
            dbSession.Save(processDefinition);

            IDictionary<string, byte[]> entries = null;
            entries = ZipUtility.ReadEntries(processArchiveStream);

            ProcessDefinitionBuildContext processDefinitionBuilder = new ProcessDefinitionBuildContext(processDefinition, entries, dbSession);
            try
            {
                if (!entries.ContainsKey("processdefinition.xml"))
                {
                    processDefinitionBuilder.AddError("entry '" + "processdefinition.xml" + "' found not found in the process archive");
                    throw new SystemException("entry '" + "processdefinition.xml" + "' found not found in the process archive");
                }
                // parse the  processdefinition.xml
                XmlElement xmlElement = getXmlElementFromBytes(entries["processdefinition.xml"]);
                // build the object model from the xml
                processDefinitionBuilder.PushScope("in processdefinition.xml");
                processDefinition.ReadProcessData(xmlElement, processDefinitionBuilder);
                processDefinition.Version = GetVersionNr(processDefinitionBuilder, processDefinition.Name);
                processDefinition.ClassFiles = GetAssemblyFiles(processDefinitionBuilder, processDefinition);
                processDefinitionBuilder.PopScope();
                // resolve all forward references
                processDefinitionBuilder.ResolveReferences();

                processDefinition.Validate(processDefinitionBuilder);

                if (processDefinitionBuilder.HasErrors())
                {
                    throw new NpdlException(processDefinitionBuilder.Errors);
                }
                // read the optional web-interface information
                if (entries.ContainsKey("web/webinterface.xml"))
                {
                    log.Debug("processing webinterface.xml...");
                    xmlElement = getXmlElementFromBytes(entries["web/webinterface.xml"]);
                    processDefinitionBuilder.PushScope("in webinterface.xml");
                    processDefinition.ReadWebData(xmlElement, processDefinitionBuilder);
                    processDefinitionBuilder.PopScope();
                }
                else
                {
                    log.Debug("no web/webinterface.xml was supplied");
                }

                processDefinitionRepository.Save(processDefinition, dbSession);
            }
            catch (SystemException e)
            {
                log.Error("xml parsing error :", e);
                processDefinitionBuilder.AddError(e.GetType().FullName + " : " + e.Message);
                processDefinitionBuilder.AddError("couldn't continue to parse the process archive");
                throw new NpdlException(processDefinitionBuilder.Errors);
            }
        }

        private const String queryFindVersionNumbers = "select max( pd.Version ) " +
            "from pd in class NetBpm.Workflow.Definition.Impl.ProcessDefinitionImpl " +
            "where pd.Name = ? ";

        private Int32 GetVersionNr(ProcessDefinitionBuildContext creationContext,string name)
        {
            int newVersion = 1;
            IEnumerator iter = creationContext.DbSession.Iterate(queryFindVersionNumbers, name, DbType.STRING).GetEnumerator();
            if (iter.MoveNext())
            {
                Int32 highestVersionNumber = (Int32)iter.Current;
                if ((Object)highestVersionNumber != null)
                {
                    newVersion = highestVersionNumber + 1;
                }
            }
            if (iter.MoveNext())
            {
                throw new NetBpm.Util.DB.DbException("duplicate value");
            }
            return newVersion;
        }

        public virtual ISet GetAssemblyFiles(ProcessDefinitionBuildContext creationContext,IProcessDefinition processDefinition)
        {
            ISet classFiles = new HashedSet();
            IDictionary<string, byte[]> entries = creationContext.Entries;
            IEnumerator iter = entries.Keys.GetEnumerator();
            while (iter.MoveNext())
            {
                String entryName = (String)iter.Current;
                if ((entryName.StartsWith("lib")) && (entryName.EndsWith(".dll")))
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("attaching assembly file " + entryName);
                    }

                    byte[] classBytes = (byte[])entries[entryName];

                    if (log.IsDebugEnabled)
                    {
                        log.Debug("found assembly " + entryName);
                    }

                    Assembly assemply = Assembly.Load(classBytes);
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("attaching assembly name: " + assemply.GetName().Name + " Version:" + assemply.GetName().Version.ToString());
                    }

                    AssemblyFileImpl assemblyFile = new AssemblyFileImpl();
                    assemblyFile.FileName = entryName;
                    assemblyFile.Bytes = classBytes;
                    assemblyFile.AssemblyName = assemply.GetName().Name;
                    assemblyFile.AssemblyVersion = assemply.GetName().Version.ToString();
                    assemblyFile.ProcessDefinition = processDefinition;
                    classFiles.Add(assemblyFile);
                }
            }
            return classFiles;
        }

        private XmlElement getXmlElementFromBytes(byte[] processDefinitionXml)
        {
            XmlElement xmlElement = null;

            Stream processDefinitionInputStream = new MemoryStream(processDefinitionXml);
            XmlParser xmlParser = new XmlParser(processDefinitionInputStream);
            xmlElement = xmlParser.Parse();
            return xmlElement;
        }

		[Transaction(TransactionMode.Requires)]
		public virtual IList GetProcessDefinitions()
		{
			return GetProcessDefinitions(null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IList GetProcessDefinitions(Relations relations)
		{
			IList processDefinitions = null;
			DbSession dbSession = null;
			dbSession = OpenSession();
			processDefinitions = processDefinitionRepository.GetProcessDefinitions(relations, dbSession);
			return processDefinitions;
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IProcessDefinition GetProcessDefinition(String processDefinitionName)
		{
			return GetProcessDefinition(processDefinitionName, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IProcessDefinition GetProcessDefinition(String processDefinitionName, Relations relations)
		{
			IProcessDefinition processDefinition = null;
			DbSession dbSession = null;
			dbSession = OpenSession();
			processDefinition = processDefinitionRepository.GetProcessDefinition(processDefinitionName, relations, dbSession);
			return processDefinition;
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IProcessDefinition GetProcessDefinition(Int64 processDefinitionId)
		{
			return GetProcessDefinition(processDefinitionId, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IProcessDefinition GetProcessDefinition(Int64 processDefinitionId, Relations relations)
		{
			IProcessDefinition processDefinition = null;
			DbSession dbSession = null;
			dbSession = OpenSession();
			processDefinition = processDefinitionRepository.GetProcessDefinition(processDefinitionId, relations, dbSession);
			return processDefinition;
		}


		[Transaction(TransactionMode.Requires)]
		public virtual IList GetAllProcessDefinitions()
		{
			return GetAllProcessDefinitions(null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IList GetAllProcessDefinitions(Relations relations)
		{
			IList allProcessDefinitions = null;
			DbSession dbSession = null;
			dbSession = OpenSession();
			allProcessDefinitions = processDefinitionRepository.GetAllProcessDefinitions(relations, dbSession);
			return allProcessDefinitions;
		}
	}
}