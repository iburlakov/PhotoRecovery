﻿using System;

using NLog;

using PhotoRecovery.Core.Scanner;
using PhotoRecovery.Core.Restore;

namespace PhotoRecovery.CLI
{
    class Program
    {
        private static ILogger log = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            try
            {

                if (args.HasArgument("-fromScratch"))
                {
                    System.IO.File.Delete("photoRecovery.db");
                    System.IO.File.Delete("photoRecovery.log");
                }

                if (args.HasArgument("-scan"))
                {
                    var pathToScan = args.GetArgument<string>("-path");

                    IScanner scanner;
                    var scanType = args.GetArgument<string>("-scan");
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
                }

                if (args.HasArgument("-recover"))
                {
                    var dirId = args.GetArgument<long>("-recover");

                    var restorer = new Restorer();

                    restorer.Restore(dirId);
                }

            }
            catch (Exception e)
            {
                log.Error(e, "Error!");
            }

            Console.ReadKey();
        }
    }
}
