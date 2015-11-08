using System;
using System.Reflection;
using System.IO;

namespace NetBpm.Test
{
	/// <summary>
	/// Basic help functions for unit tests
	/// </summary>
	public class TestHelper
	{
		private static readonly string CONFIG_PATH = "testconf";
		/// <summary>
		/// Search for the config directory. 
		/// The problem is that the test can be run in the GUI and NANT and the directory differs.
		/// </summary>
		public static string GetConfigDir()
		{
			string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
			if (path.EndsWith("bin"))
			{
				return ("..\\");
			} 
			else 
			{
				return ("..\\..\\");
			}
		}
		/// <summary>
		/// Search for the config directory. 
		/// The problem is that the test can be run in the GUI and NANT and the directory differs.
		/// </summary>
		public static string GetExampleDir()
		{
			string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
			if (path.EndsWith("bin"))
			{
				return ("..\\example\\");
			} 
			else 
			{
				return ("..\\..\\..\\..\\src\\NetBpm.Example\\");
			}
		}
	}
}
