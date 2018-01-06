using System;

using NLog;

using PhotoRecovery.Core.Scanner;
using PhotoRecovery.Core.Restore;
using PhotoRecovery.Core.Data;
using System.Linq;
using System.Collections.Generic;

namespace PhotoRecovery.CLI
{
    class Program
    {
        private static ILogger log = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            using (var context = new Context())
            {
                Console.WriteLine(context.GetStatString());
            }

            while (true)
            {
                try
                {
                    Console.WriteLine("Input command...");
                    var commandArgs = Console.ReadLine().Split(' ');

                    if (commandArgs.HasArgument("-fromScratch"))
                    {
                        System.IO.File.Delete("photoRecovery.db");
                        System.IO.File.Delete("photoRecovery.log");
                    }
                    else if (commandArgs.HasArgument("-scan"))
                    {
                        var pathToScan = commandArgs.GetArgument<string>("-path");

                        IScanner scanner;
                        var scanType = commandArgs.GetArgument<string>("-scan");
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
                    else if (commandArgs.HasArgument("-recover"))
                    {
                        var idSeparators = new char[] { ' ', ',', '|' };
                       long[] ids = null;
                        try
                        {
                            var idsStr = string.Join(" ", commandArgs.Skip(1).ToArray());
                            ids = idsStr.Split(idSeparators, StringSplitOptions.RemoveEmptyEntries).Select(s => long.Parse(s)).ToArray();
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("Could not parse dir ids, use [{0}] as separators", string.Join(",", idSeparators.Select(c => string.Format("'{0}'", c)).ToArray()));
                            log.Error(e);
                            continue;
                        }

                        Restorer.Instanse.Restore(ids);
                    }
                    else if (commandArgs.HasArgument("-help"))
                    {
                        Console.WriteLine(@"Supported commands:
    -fromScratch                              deletes database anad log file;
    -scan structure|raw -path folder_path     scans foldder_path;
    -recover list_of_dir_ids                  recovers folders with given ids;
    -quit                                     exits;
    -help                                     prints this message.");
                    }
                    else if (commandArgs.HasArgument("-quit"))
                    {
                        Console.WriteLine("Bye");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Unknown command, type '-help' for the list of commands.");
                    }
                }
                catch (Exception e)
                {
                    log.Error(e, "Error!");
                }
            }

        }
    }
}
