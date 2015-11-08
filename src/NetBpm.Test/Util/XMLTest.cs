using System.Collections;
using System.Xml;
using NetBpm.Util.Xml;
using NUnit.Framework;
using XmlElement = NetBpm.Util.Xml.XmlElement;

namespace NetBpm.Test.Util
{
	[TestFixture]
	public class XMLTest
	{
		public XMLTest()
		{
		}

		[Test]
		public void TestXmlParser()
		{
			XmlTextReader exampleXml = new XmlTextReader(
				TestHelper.GetExampleDir()+"process\\holiday\\processdefinition.xml");
			XmlParser xmlParser = new XmlParser(exampleXml);
			XmlElement element = xmlParser.Parse();

			XmlElement startstate = element.GetChildElement("start-state");
			Assert.IsNotNull(startstate);
			Assert.IsNotNull(startstate.GetAttribute("name"));

			XmlElement endstate = element.GetChildElement("end-state");
			Assert.IsNotNull(endstate);
			Assert.IsNotNull(endstate.GetAttribute("name"));

			IList pdattr = element.GetChildElements("attribute");
			Assert.IsNotNull(pdattr);
			Assert.IsTrue(pdattr.Count > 1);
		}

	}
}