using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace NetBpm.Util.Xml
{
	public class XmlElement
	{
		private String _name = null;
		private IList _content;
		private IDictionary _children;
		private IDictionary _attributes;

		private static readonly String LINESEPARATOR;

		static XmlElement()
		{
			LINESEPARATOR = UnicodeCategory.LineSeparator.ToString();
		}

		private void InitBlock()
		{
			_content = new ArrayList();
			_children = new Hashtable();
			_attributes = new Hashtable();
		}

		public String Name
		{
			get { return _name; }
		}

		public IDictionary Attributes
		{
			get { return _attributes; }
			set { this._attributes = value; }
		}

		public IList Content
		{
			get { return _content; }
		}

		public XmlElement(String name)
		{
			InitBlock();
			this._name = name;
		}

		public void RemoveAttribute(String attributeName)
		{
			_attributes.Remove(attributeName);
		}

		public String GetAttribute(String attributeName)
		{
			return (String) _attributes[attributeName];
		}

		public void AddChild(XmlElement child)
		{
			String childName = child.Name;
			IList namedChildren = (IList) _children[childName];
			if (namedChildren == null)
			{
				namedChildren = new ArrayList();
				_children[childName] = namedChildren;
			}
			namedChildren.Add(child);
			_content.Add(child);
		}

		public void RemoveXmlElement(XmlElement delegateXmlElement)
		{
			IList namedChildren = (IList) _children[delegateXmlElement.Name];
			namedChildren.Remove(delegateXmlElement);
			_content.Remove(delegateXmlElement);
		}

		public IList GetChildElements(String childName)
		{
			IList childElements = (IList) _children[childName];
			if (childElements == null)
			{
				childElements = new ArrayList(0);
			}
			return childElements;
		}

		public XmlElement GetChildElement(String childName)
		{
			XmlElement child = null;
			IList namedChildren = (IList) _children[childName];
			if (namedChildren != null)
			{
				if (namedChildren.Count == 1)
				{
					child = (XmlElement) namedChildren[0];
				}
				else if (namedChildren.Count > 1)
				{
					throw new SystemException("expected only one child-element '" + childName + "' of element '" + _name + "' while there were " + namedChildren.Count);
				}
			}
			return child;
		}

		public void AddText(String text)
		{
			this._content.Add(text);
		}

		public String GetContentString()
		{
			StringBuilder buffer = new StringBuilder();
			GetContentString(buffer, "");
			return buffer.ToString();
		}

		public String GetProperty(String propertyName)
		{
			String propertyValue = null;
			if (_attributes.Contains(propertyName))
			{
				propertyValue = ((String) _attributes[propertyName]);
			}
			else
			{
				XmlElement child = this.GetChildElement(propertyName);

				if ((child != null) && (child._content.Count == 1))
				{
					IEnumerator e = child._content.GetEnumerator();
					e.MoveNext();
					Object contentsString = e.Current;
					if (!(contentsString is String))
					{
						throw new SystemException("can't get property '" + propertyName + "' from element '" + _name + "' : child-element with that name contains an element instead of text");
					}
					propertyValue = ((String) contentsString);
				}
			}
			return propertyValue;
		}

		public void GetContentString(StringBuilder buffer, String indentation)
		{
			IEnumerator iter = _content.GetEnumerator();
			while (iter.MoveNext())
			{
				Object contentItem = iter.Current;
				if (contentItem is XmlElement)
				{
					XmlElement element = (XmlElement) contentItem;
					element.ToString(buffer, indentation);
				}
				else
				{
					buffer.Append(contentItem.ToString());
				}

			}
		}

		public override String ToString()
		{
			StringBuilder buffer = new StringBuilder();
			ToString(buffer, "");
			return buffer.ToString().Trim();
		}

		private void ToString(StringBuilder buffer, String indentation)
		{
			buffer.Append('<');
			buffer.Append(_name);

			IEnumerator iter = _attributes.GetEnumerator();
			while (iter.MoveNext())
			{
				DictionaryEntry entry = (DictionaryEntry) iter.Current;
				buffer.Append(' ');
				buffer.Append((String) entry.Key);
				buffer.Append("=\"");
				buffer.Append((String) entry.Value);
				buffer.Append("\"");
			}

			if (_content.Count > 0)
			{
				buffer.Append('>');
				GetContentString(buffer, indentation + "  ");
				buffer.Append("</");
				buffer.Append(_name);
				buffer.Append('>');
			}
			else
			{
				buffer.Append("/>");
			}
		}
	}
}