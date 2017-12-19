using System;

using NLog;

using PhotoRecovery.Core.Scanner;

namespace PhotoRecovery.CLI
{
    class Program
    {
        private static ILogger log = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            try
            {
                var pathToScan = args.GetArgument<string>("path");

                IScanner scanner;
                var scanType = args.GetArgument<string>("scan");
                switch (scanType)
                {
                    case "structure":
                        scanner = new StructureScanner();
                        break;

                    case "raw":
                        scanner = new RawScanner();
                        break;

                    default:
                        throw new InvalidOperationException(string.Format("Unknown value {0} for {1} argument", scanType, "scan"));
                }

                scanner.Scan(pathToScan);

                log.Info("Done!");
            }
            catch (Exception e)
            {
                log.Error(e, "Error!");
            }

            Console.ReadKey();
        }
    }
}
