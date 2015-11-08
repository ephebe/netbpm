using System;
using System.Collections;
using System.IO;
using System.Xml;
using log4net;

namespace NetBpm.Util.Xml
{
	/// <summary> convenience class for parsing and/or validating xml documents.
	/// </summary>
	public class XmlParser
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (XmlParser));

		private XmlReader _xmlreader;
		private bool isConsumed = false;
		private bool validate_Renamed_Field = true;
		private IList _elementStack;

		public bool Validation
		{
			set { this.validate_Renamed_Field = value; }
		}

		/// <summary> creates an XmlParser for an inputStream (conaining an xml-document).
		/// @throws NullPointerException if inputStream is null. 
		/// @portme
		/// </summary>
		public XmlParser(Stream xmlstream)
		{
			if (xmlstream == null)
			{
				throw new NullReferenceException("couldn't create an XmlParser with a null-value for xmlreader");
			}
			this._xmlreader = new XmlTextReader(xmlstream);
		}

		/// <summary> creates an XmlParser for an inputStream (conaining an xml-document).
		/// @throws NullPointerException if inputStream is null. 
		/// @portme
		/// </summary>
		public XmlParser(XmlReader xmlreader)
		{
			if (xmlreader == null)
			{
				throw new NullReferenceException("couldn't create an XmlParser with a null-value for xmlreader");
			}
			this._xmlreader = xmlreader;
		}

		/// <summary> creates an XmlParser for an inputStream (conaining an xml-document).
		/// @throws NullPointerException if reader is null. 
		/// </summary>
		public XmlParser(String xmlstring)
		{
			if (xmlstring == null)
			{
				throw new NullReferenceException("couldn't create an XmlParser with a null-value for xml");
			}
			StringReader ts = new StringReader(xmlstring);
			this._xmlreader = new XmlTextReader(ts);
		}

		/// <summary> validates an xml-document without building a XmlElement's for the parsed contents.</summary>
		public void Validate()
		{
//			this.validate_Renamed_Field = true;
//			this.parse_Renamed_Field = false;
			Parse();
		}

		/// <summary> parses and/or validates an xml document.
		/// @throws IllegalStateException if this parser has already been used before.  It can only be
		/// used once because the inputSource can be read only once.
		/// </summary>
		public XmlElement Parse()
		{
			if (isConsumed)
			{
				throw new SystemException("this XmlParser-instance has already been used, please create a new one for each usage");
			}
			XmlElement rootElement = null;
			_elementStack = new ArrayList();

			while (_xmlreader.Read())
			{
				switch (_xmlreader.NodeType)
				{
					case XmlNodeType.Element:
						XmlElement newElement = new XmlElement(_xmlreader.Name);
						bool isEmptyElement = _xmlreader.IsEmptyElement;

						Hashtable attributes = new Hashtable();
						if (_xmlreader.HasAttributes)
						{
							for (int i = 0; i < _xmlreader.AttributeCount; i++)
							{
								_xmlreader.MoveToAttribute(i);
								attributes.Add(_xmlreader.Name, _xmlreader.Value);
							}
						}

						newElement.Attributes = attributes;

						int elementStackSize = _elementStack.Count;
						if (elementStackSize > 0)
						{
							XmlElement containingElement = (XmlElement) _elementStack[_elementStack.Count - 1];
							containingElement.AddChild(newElement);
						}
						else
						{
							rootElement = newElement;
						}

						_elementStack.Add(newElement);

						if (isEmptyElement)
						{
							_elementStack.RemoveAt(_elementStack.Count - 1);
						}
						break;
					case XmlNodeType.EndElement:
						_elementStack.RemoveAt(_elementStack.Count - 1);
						break;
					case XmlNodeType.Text:
						if (_xmlreader.Value.Length > 0)
						{
							XmlElement element = (XmlElement) _elementStack[_elementStack.Count - 1];
							element.AddText(_xmlreader.Value);
						}
						break;
				}
			}
			isConsumed = true;
			return rootElement;
		}
	}
}