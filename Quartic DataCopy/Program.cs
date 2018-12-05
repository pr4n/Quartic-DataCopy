using System;
using System.Collections.Generic;
using CommandLine;
using NLog;
using NLog.Targets;
using NLog.Config;

namespace Quartic_DataCopy
{
    class Program
    {

        private static LoggingConfiguration logConfig = new LoggingConfiguration();

        public class Options
        {
            [Option('v', "verbose", Required = false, Default =false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }

            [Option('s', "source", Required = true, HelpText = "Source directory.")]
            public string Source { get; set; }

            [Option('d', "dest", Required = true, HelpText = "Destination Directory.")]
            public string Dest { get; set; }

            [Option('p', "pattern", Required = false, Default = "*", HelpText = "Regex pattern to filter files in source directory.")]
            public string Pattern { get; set; }

            [Option('r', "remove-source", Required = false, Default = false, HelpText = "If provided, will delete the source files once they're copied.")]
            public bool Clean { get; set; }

            [Option('l', "logdir", Required = false, Default = null, HelpText = "Specify the directory for logging. If not specified, no logs will be written.")]
            public string Logdir { get; set; }
        }

        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args).
                WithParsed<Options>(opts => RunOptionsAndReturnExitCode(opts)).
                WithNotParsed<Options>((errs) => HandleParseError(errs));
        }

        static int RunOptionsAndReturnExitCode(Options opts)
        {
            var consoleTarget = new ColoredConsoleTarget("target1")
            {
                Layout = @"${date:format=HH\:mm\:ss} ${level} ${message} ${exception}"
            };
            logConfig.AddTarget(consoleTarget);
            logConfig.AddRuleForAllLevels(consoleTarget); // all to console

            if (opts.Logdir != null)
            {
                if (!System.IO.Directory.Exists(opts.Logdir))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(opts.Dest);
                    }
                    catch (System.IO.IOException e)
                    {
                        Console.WriteLine("Unable to create log dir. Logging to file disabled.");
                    }
                }
                var fileTarget = new FileTarget("target2")
                {
                    FileName = opts.Logdir + "/DataCopyLog.txt",
                    Layout = "${longdate} ${level} ${message}  ${exception}"
                };
                logConfig.AddTarget(fileTarget);
                logConfig.AddRuleForAllLevels(fileTarget); // all to file
            }
            
            LogManager.Configuration = logConfig;
            return Copy(opts.Source, opts.Dest, opts.Pattern, opts.Clean, opts.Verbose);
        }

        private static int Copy(string source, string dest, string pattern = "*", 
            bool clean=true, bool verbose=false)
        {
            Logger logger = LogManager.GetLogger("Copy");
            logger.Info("Starting to copy from: " + source + " to: " + dest);
            if (!System.IO.Directory.Exists(source))
            {
                logger.Fatal("Source Directory does not exist.");
                LogManager.Shutdown();
                return -1;
            }

            if (!System.IO.Directory.Exists(dest))
            {
                logger.Info("Destination Dir does not exist. Creating.");
                System.IO.Directory.CreateDirectory(dest);
            }

            var files = System.IO.Directory.EnumerateFiles(source, pattern, System.IO.SearchOption.TopDirectoryOnly);
            foreach (string s in files)
            {
                // Use static Path methods to extract only the file name from the path.
                var fileName = System.IO.Path.GetFileName(s);
                var destFile = System.IO.Path.Combine(dest, fileName);
                System.IO.File.Copy(s, destFile, true);
                if (verbose)
                {
                    logger.Info("Copied file " + s + " to " + destFile);
                }
                if (clean)
                {
                    try
                    {
                        System.IO.File.Delete(s);
                    }
                    catch(System.IO.IOException e)
                    {
                        logger.Error(e, "Can not delete file " + s);
                    }
                    if (verbose)
                        logger.Info("Cleaned up source file: " + s);
                }
            }
            logger.Info("Completed Copying");
            LogManager.Shutdown();
            return 0;
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {

        }
    }
}
