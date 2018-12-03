using System;
using System.Collections.Generic;
using CommandLine;

namespace Quartic_DataCopy
{
    class Program
    {
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

            [Option('c', "clean", Required = false, Default = false, HelpText = "If set, will clean the files once they're copied.")]
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
            return Copy(opts.Source, opts.Dest, opts.Pattern, opts.Logdir, opts.Clean, opts.Verbose);
        }

        private static int Copy(string source, string dest, string pattern = "*", 
            string logdir=null, bool clean=true, bool verbose=false)
        {
            if (!System.IO.Directory.Exists(source))
            {
                Console.WriteLine("Source Directory does not exist.");
                return -1;
            }

            if (!System.IO.Directory.Exists(dest))
            {
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
                    Console.WriteLine("Copied file " + s + " to " + destFile);
                }
                if (clean)
                {
                    try
                    {
                        System.IO.File.Delete(s);
                    }
                    catch(System.IO.IOException e)
                    {
                        Console.WriteLine("Can not delete file " + s + ". Check below error");
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.InnerException);
                    }
                    if (verbose)
                        Console.WriteLine("Cleaned up source file: " + s);
                }
            }
            return 0;
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {

        }
    }
}
