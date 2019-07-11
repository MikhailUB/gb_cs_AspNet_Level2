using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Xml;

namespace WebStore.Logger
{
	public class Log4NetLoggerProvider : ILoggerProvider
	{
		private readonly string _configFile;

		private readonly ConcurrentDictionary<string, Log4NetLogger> _loggers = new ConcurrentDictionary<string, Log4NetLogger>();

		public Log4NetLoggerProvider(string configFile) => _configFile = configFile;

		public ILogger CreateLogger(string categoryName)
		{
			return _loggers.GetOrAdd(categoryName, category =>
			{
				var xml = new XmlDocument();
				var fileName = _configFile;
				xml.Load(fileName);
				return new Log4NetLogger(category, xml["log4net"]);
			});
		}

		public void Dispose() => _loggers.Clear();
	}
}
