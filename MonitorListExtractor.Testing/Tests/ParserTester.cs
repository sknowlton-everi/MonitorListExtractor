using log4net;
using log4net.Repository;
using MonitorListExtractor;
using log4net.Config;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace MonitorListExtractor.Testing.Tests
{
    public class ParserTester
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Parser sut;

        [SetUp]
        public void Setup()
        {
            ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            log.Info(message: new StackFrame().GetMethod().Name);

            string path = @"..\..\..\..\MonitorListExtractor.Testing\Public\Configurations.xml";

            sut = new Parser(path);
        }

        [Test]
        public void ParseerTest()
        {
            Assert.Pass();
        }
    }
}