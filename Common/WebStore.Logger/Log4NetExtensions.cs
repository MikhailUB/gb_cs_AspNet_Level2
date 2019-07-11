using Microsoft.Extensions.Logging;
using System.IO;
using System.Reflection;

namespace WebStore.Logger
{
	public static class Log4NetExtensions
	{
		public static ILoggerFactory AddLog4Net(this ILoggerFactory factory, string configFile = "log4net.config")
		{
			if (!Path.IsPathRooted(configFile))
			{
				var assembly = Assembly.GetEntryAssembly();
				var dir = Path.GetDirectoryName(assembly.Location);
				configFile = Path.Combine(dir, configFile);
			}

			factory.AddProvider(new Log4NetLoggerProvider(configFile));

			return factory;
		}
	}
}
