using System;
using System.Collections;
using System.Reflection;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using Castle.Model.Configuration;

namespace NetBpm.Web.Presentation.Helper
{
	public class InfoHelper
	{
		public String GetOSVersion()
		{
			return Environment.OSVersion.Version.ToString();
		}

		public String GetDotNetVersion()
		{
			return Environment.Version.ToString();
		}

		public String GetMachineName()
		{
			return Environment.MachineName;
		}

		public String BuildAssemplyInfo(Assembly assembly)
		{
			if (assembly.GetName().CodeBase.EndsWith("/system.dll"))
			{
				return String.Empty;
			}
			
			return String.Format("<tr><td class=\"tableCell\">{0}</td><td class=\"tableCell\">{1}</td><td class=\"tableCell\">{2}</td></tr>",
				assembly.GetName().Name, assembly.GetName().Version,
				assembly.GetName().CodeBase);
		}

		public String BuildFacilityInfo(IFacility facility)
		{
			String returnValue=String.Format("<tr class=\"tableHeaderRow\"><th nowrap class=\"tableHeaderCell\" colspan=\"3\">{0}</th></tr>\n",
				facility.ToString());
			if (typeof(AbstractFacility).IsAssignableFrom(facility.GetType()))
			{
				String indentation = "&nbsp;&nbsp;&nbsp;";
				IConfiguration conf = ((AbstractFacility)facility).FacilityConfig;
				returnValue=WriteAttributes(conf, returnValue,indentation);
			}
			return returnValue;
		}

		private String WriteAttributes(IConfiguration conf,String returnValue,String indentation)
		{
			if ( conf != null)
			{
				IEnumerator keys = conf.Attributes.AllKeys.GetEnumerator();
				while(keys.MoveNext())
				{
					String key = (String)keys.Current;
					returnValue += String.Format("<tr><td class=\"tableCell\">{0}+-{1}</td><td class=\"tableCell\">{2}</td><td class=\"tableCell\">{3}</td></tr>\n",
					                             indentation,key,conf.Attributes[key],conf.Value);

				}
				ConfigurationCollection childConfigs=conf.Children;
				IEnumerator childEnum=childConfigs.GetEnumerator();
				indentation +="&nbsp;&nbsp;&nbsp;"; 
				while(childEnum.MoveNext())
				{
					IConfiguration childConfig = (IConfiguration) childEnum.Current;
					returnValue = WriteAttributes(childConfig,returnValue,indentation);
				}
			}
			return returnValue;
		}

	}
}
