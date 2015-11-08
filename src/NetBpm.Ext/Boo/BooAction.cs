using System;
using System.Collections;
using log4net;
using NetBpm.Workflow.Delegation;
using Boo.Lang.Interpreter;

namespace NetBpm.Ext.Boo
{
	/// <summary>
	/// a ActionHandler for Boo.
	/// </summary>
	public class BooAction : IActionHandler
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (BooAction));

		public void Run(IActionContext actionContext)
		{
			InteractiveInterpreter interpreter = new InteractiveInterpreter();

			IDictionary configuration = actionContext.GetConfiguration();
			String script = (String) configuration["script"];
			if (script == null)
			{
				throw new ArgumentException("Can’t find boo script in configuration! Please check processdefiniton.xml if action defines <parameter name = \"script\">");
			}

			CopyAttributesToInterpreter(interpreter,actionContext);
			interpreter.Eval(script);
			CopyInterpreterToAttributes(interpreter,actionContext);
		}

		private void CopyAttributesToInterpreter(InteractiveInterpreter interpreter,
												IActionContext context)
		{
			IDictionaryEnumerator configEnum;
			configEnum=context.GetConfiguration().GetEnumerator();
			while(configEnum.MoveNext())
			{
				DictionaryEntry property;
				property = (DictionaryEntry)configEnum.Current;
				if (!property.Key.Equals("script") && property.Value.ToString().IndexOf("In")!=-1)
				{
					object attributeValue;
					attributeValue = context.GetAttribute((String)property.Key);
					if (log.IsDebugEnabled)
					{
						log.Debug("copy to ->interpreter key:"+property.Key+" value:"+attributeValue);
					}
					interpreter.SetValue((String)property.Key,attributeValue);
				}
			}
		}

		private void CopyInterpreterToAttributes(InteractiveInterpreter interpreter,
												IActionContext context)
		{
			IDictionaryEnumerator configEnum;
			configEnum=context.GetConfiguration().GetEnumerator();
			while(configEnum.MoveNext())
			{
				DictionaryEntry property;
				property = (DictionaryEntry)configEnum.Current;
				if (!property.Key.Equals("script"))
				{
					object attributeValue = context.GetAttribute((String)property.Key);
					object interpreterValue = interpreter.GetValue((String)property.Key);
					// Change the attribute only if the value changed and is marked for copying
					if (!property.Key.Equals("script") && attributeValue != null 
						&& ! attributeValue.Equals(interpreterValue)
						&& property.Value.ToString().IndexOf("Out")!=-1 )
					{
						if (log.IsDebugEnabled)
						{
							log.Debug("copy from <-interpreter key:"+property.Key+" oldvalue:"+attributeValue+ " newvalue:"+interpreterValue);
						}
						context.SetAttribute((String)property.Key,interpreterValue);
					}
				}
			}
		}
	}
}
