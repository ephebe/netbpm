using System;
using System.Runtime.Serialization;
using System.Web;

namespace NetBpm.Workflow.Delegation
{
	/// <summary> is an interface used for the automatic generation of activity-forms in the web-interface.
	/// <b>IMPORTANT NOTE</b> : In the web-interface, the class-name is used as a key for finding 
	/// the internationalized name of this HtmlFormatter.
	/// It is used when a ParseException is thrown in {@link #parseHttpParameter} to inform
	/// the user what kind of data whas excpected.  The error-msg is constructed like
	/// this : "Wrong input for field [field-name].  [received-value] could not be
	/// parsed as [translated text for html-formatter]"
	/// </summary>
	public interface IHtmlFormatter : IConfigurable //@portme, ISerializable
	{
		/// <summary> generates a HTML-fragment that is used to produce the automatically generated 
		/// web-form that allows a user to perform an activity.
		/// </summary>
		/// <param name="valueObject">is the current {@link NetBpm.Workflow.Execution.IAttributeInstance} 
		/// value for the attribute for which html has to be generated.
		/// </param>
		/// <param name="parameterName">is the name of the http-parameter on which the NetBpm 
		/// web-application will expect input for this attribute.  If input is 
		/// present for that parameter-name, it is parsed using the parseHttpParameter-method
		/// </param>
		String ObjectToHtml(Object valueObject, String parameterName, HttpRequest request);

		/// <summary> parses the text that is returned by the web-client to an java-object before it is 
		/// stored in an {@link NetBpm.Workflow.Execution.IAttributeInstance}.
		/// </summary>
		/// <param name="text">is the String that is passed from the web-client to the server
		/// for html-input-control that was generated with the objectToHtml-method.
		/// </param>
		/// <returns> the parsed java-object that will be stored in an 
		/// {@link NetBpm.Workflow.Execution.IAttributeInstance}. 
		/// @throws ParseException if the text is not what this HtmlFormatter expects.  A 
		/// ParseException will cause the web-application to go back to the form with an 
		/// error message as explained above. 
		/// </returns>
		Object ParseHttpParameter(String text, HttpRequest request);
	}
}