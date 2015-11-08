using System;
using System.Collections;
using System.Reflection;
using Iesi.Collections;
using log4net;
using NetBpm.Util.DB;
using NetBpm.Util.Xml;
using NetBpm.Workflow.Delegation.Impl;
using NetBpm.Workflow.Organisation;
using NHibernate.Type;
using System.Collections.Generic;

namespace NetBpm.Workflow.Definition.Impl
{
    /// <summary>
    /// 只負責讀取StartState，EndState
    /// </summary>
	public class ProcessDefinitionImpl : ProcessBlockImpl, IProcessDefinition
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (ProcessDefinitionImpl));
		private Int32 _version;
		private ISet _classFiles = null;
		private EndStateImpl _endState = null;
		private StartStateImpl _startState = null;
		private String _responsibleUserName = null;
		private byte[] _image = null;
		private String _imageMimeType = null;
		private Int32 _imageHeight;
		private Int32 _imageWidth;

		[NonSerialized()] private DelegationImpl authorizationDelegation = null;

        public virtual DelegationImpl AuthorizationDelegation
		{
			get { return this.authorizationDelegation; }
			set { this.authorizationDelegation = value; }
		}

        public virtual String ResponsibleUserName
		{
			get { return this._responsibleUserName; }
			set { this._responsibleUserName = value; }
		}

        public virtual Int32 Version
		{
			get { return this._version; }
			set { this._version = value; }
		}

        public virtual IStartState StartState
		{
			get { return this._startState; }
			set { this._startState = (StartStateImpl) value; }
		}

        public virtual IEndState EndState
		{
			get { return this._endState; }
			set { this._endState = (EndStateImpl) value; }
		}

        public virtual byte[] Image
		{
			get { return this._image; }
			set { this._image = value; }
		}

        public virtual String ImageMimeType
		{
			get { return this._imageMimeType; }
			set { this._imageMimeType = value; }
		}

        public virtual Int32 ImageHeight
		{
			get { return this._imageHeight; }
			set { this._imageHeight = value; }
		}

        public virtual Int32 ImageWidth
		{
			get { return this._imageWidth; }
			set { this._imageWidth = value; }
		}

        public virtual ISet ClassFiles
		{
			get { return this._classFiles; }
			set { this._classFiles = value; }
		}

		public ProcessDefinitionImpl()
		{
		}

        public virtual StartStateImpl CreateStartState()
		{
			_startState = new StartStateImpl(this);
			AddNode(_startState);
			return _startState;
		}

        public virtual EndStateImpl CreateEndState()
		{
			_endState = new EndStateImpl(this);
			AddNode(_endState);
			return _endState;
		}

		public override void ReadProcessData(XmlElement xmlElement, ProcessDefinitionBuildContext creationContext)
		{
			this._endState = new EndStateImpl();
			this._startState = new StartStateImpl();

			// read the process-block contents, the start- and the end-state 
			creationContext.ProcessBlock = this;
			base.ReadProcessData(xmlElement, creationContext);
			XmlElement startElement = xmlElement.GetChildElement("start-state");
			creationContext.Check((startElement != null), "element start-state is missing");
			XmlElement endElement = xmlElement.GetChildElement("end-state");
			creationContext.Check((endElement != null), "element end-state is missing");
			_startState.ReadProcessData(startElement, creationContext);
			_endState.ReadProcessData(endElement, creationContext);
			creationContext.ProcessBlock = null;

			// add the start & end state to the nodes of this process definition
			this._nodes.Add(_startState);
			this._nodes.Add(_endState);

			// add the end state as referencable object
			creationContext.AddReferencableObject(this._endState.Name, this, typeof (INode), this._endState);

			// read the optional authorization handler
			XmlElement authorizationElement = xmlElement.GetChildElement("authorization");
			if (authorizationElement != null)
			{
				creationContext.DelegatingObject = this;
				this.authorizationDelegation = new DelegationImpl();
				this.authorizationDelegation.ReadProcessData(authorizationElement, creationContext);
				creationContext.DelegatingObject = null;
			}

			// read the optional responsible for this process definition
			this._responsibleUserName = xmlElement.GetProperty("responsible");

			// calculate the version of this process definition
            //todo 這行應該移到ProcessDefinitionService
			//this._version = GetVersionNr(creationContext);

			// attach the class files to this process definitions
            //todo 這行應該移到ProcessDefinitionService
			//this._classFiles = GetAssemblyFiles(creationContext);
		}

		private const String queryFindState = "select s " +
			"from s in class NetBpm.Workflow.Definition.Impl.StateImpl " +
			"where s.Name = ? " +
			"  and s.ProcessDefinition.id = ?";

        public virtual void ReadWebData(XmlElement xmlElement, ProcessDefinitionBuildContext creationContext)
		{
			// first read the image
			XmlElement imageElement = xmlElement.GetChildElement("image");
			creationContext.Check((imageElement != null), "element image is missing");
			// reading the image-file     
			String imageFileName = imageElement.GetProperty("name");
			creationContext.Check(((Object) imageFileName != null), "image name is missing");
			this._image = (byte[]) creationContext.Entries[imageFileName];

			if (this._image == null)
			{
				creationContext.AddError("couldn't find image file '" + imageFileName + "' in the process archive. (make sure the specified path is relative to the archive-root)");
			}

			this._imageMimeType = imageElement.GetProperty("mime-type");
			creationContext.Check(((Object) _imageMimeType != null), "image mime-type is missing");
			try
			{
				_imageHeight = Int32.Parse(imageElement.GetProperty("height"));
				creationContext.Check(((Object) _imageHeight != null), "image height is missing");
				_imageWidth = Int32.Parse(imageElement.GetProperty("width"));
				creationContext.Check(((Object) _imageWidth != null), "image width is missing");
			}
			catch (FormatException e)
			{
				creationContext.AddError("image height or width contains unparsable numbers : height=\"" + imageElement.GetProperty("height") + "\" width=\"" + imageElement.GetProperty("width") + "\". Exception: " + e.Message);
			}

			DbSession dbSession = creationContext.DbSession;

			// then the activity-states
			IEnumerator iter = xmlElement.GetChildElements("activity-state").GetEnumerator();
			while (iter.MoveNext())
			{
				XmlElement activityStateElement = (XmlElement) iter.Current;
				String activityStateName = activityStateElement.GetProperty("name");
				creationContext.Check(((Object) activityStateName != null), "property name in activity state is missing");

				Object[] values = new Object[] {activityStateName, _id};
				IType[] types = new IType[] {DbType.STRING, DbType.LONG};
				StateImpl state = null;

				try
				{
					state = (StateImpl) dbSession.FindOne(queryFindState, values, types);
				}
				catch (DbException e)
				{
					creationContext.AddError("activity-state '" + activityStateName + "' was referenced from the webinterface.xml but not defined in the processdefinition.xml. Exception:" + e.Message);
				}

				state.ReadWebData(activityStateElement, creationContext);
			}
		}

		public override void Validate(ValidationContext validationContext)
		{
			try
			{
				// validate the processBlock constraints
				base.Validate(validationContext);

				// the startState is required
				validationContext.Check((_startState != null), "the process definition does not have a start-state");
				validationContext.Check((_endState != null), "the process definition does not have an end-state");

				// the endState is not allowed to have leaving transitions
				validationContext.Check((_endState.LeavingTransitions.Count == 0), "the end-state of the process definition has leaving transitions");
				validationContext.Check((_startState.ArrivingTransitions.Count == 0), "the start-state of the process definition has arriving transitions");

				// check if all image attributes are supplied or none at all
				validationContext.Check(((_image == null) && ((Object) _imageMimeType == null) && (_imageHeight == 0) && (_imageWidth == 0)) || ((_image != null) && ((Object) _imageMimeType != null) && (_imageHeight != 0) && (_imageWidth != 0)), "all image attributes (name,mime-type,width&height) must be supplied or none at all (@see web/webinterface.xml)");

				// this catch allows the validation to throw runtime exceptions if the cause
				// is already added as an error.  (mainly to avoid if-then-else in the
				// validation code to avoid NullPointerExceptions)
			}
			catch (SystemException e)
			{
				validationContext.AddError("couldn't continue validation : " + e.GetType().FullName + " : " + e.Message);
			}
		}

        public virtual String WriteProcessData()
		{
			// TODO : add xml-serialisation of the process-logic of this definition
			// should generate the processdefinition.xml in a process archive
			// This method should do +- the inverse of the readProcessData.
			return null;
		}

        public virtual String WriteWebData()
		{
			// TODO : add xml-serialisation of the web-interface of this definition
			// should generate the web/webinterface.xml in a process archive
			// This method should do +- the inverse of the readWebData.
			return null;
		}

        public virtual IUser GetResponsible()
		{
			return (IUser) OrganisationUtil.Instance.GetActor(_responsibleUserName);
		}

		public override String ToString()
		{
			return "ProcessDefinitionImpl[" + _id + "|" + _name + "|" + _version + "]";
		}
	}
}