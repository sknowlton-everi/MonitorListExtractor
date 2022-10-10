using log4net;
using log4net.Config;
using log4net.Repository;
using MonitorListExtractor.Utility;
using System.Diagnostics;
using System.Reflection;

namespace MonitorListExtractor
{
    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main(string[] args)
        {
            ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            log.Debug("Starting Application");
            log.Info(new StackFrame().GetMethod().Name);

            try
            {
                //Guard.Against.IsNotNullOrEmpty(args[0]).Throw(log).WithMessage("Argument 0 cannot be null").Assert();
                //Guard.Against.IsNotNullOrEmpty(args[0]).Throw(log).WithMessage("File is invalid").Assert();

                if (args[0].Equals(string.Empty) || File.Exists(args[0]) == false)
                {
                    log.Debug($"No processing happened.  Unable to find the specified file:  {args[0]}");
                }

                Parser parser = new(args[0]);
                parser.Parse();
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            log.Debug("Ending Application");
            Console.ReadKey();
        }
    }
}