using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace mkdisp
{
    partial class Program
    {
        static string arch;
        static string mode = "cmd";
        static string destPath;
        static string srcPath;
        static string target = "";
        static string name = "";
        static bool info = false;
        static bool force = false;
        static string[] extraArgs = new string[0];

        static int Main(string[] args)
        {
            arch = ConfigurationManager.AppSettings["ARCH"];
            destPath = ConfigurationManager.AppSettings["DESTPATH"];
            srcPath = ConfigurationManager.AppSettings["SRCPATH"];

            // Output a single blank line for eligibility
            Console.WriteLine();

            if (!processArgs(args))
            {
                // Output a single blank line for eligibility
                Console.WriteLine();
                printUsage();
                return 1;
            }

            // Don't do any work if an "info" switch has been supplied
            if(!info)
            {
                if (string.IsNullOrEmpty(srcPath))
                {
                    Console.WriteLine("Error: SRCPATH is not configured in mkdisp.exe.config");
                    return 1;
                }

                if (!File.Exists(target))
                {
                    Console.WriteLine($"Error: Target file '{target}' does not exist.");
                    return 1;
                }

                if (!force && File.Exists(name))
                {
                    Console.WriteLine($"Error: File '{name}' exists, use -f to overwrite file.");
                    return 1;
                }

                var configFile = name + ".config";
                if(!force && File.Exists(configFile))
                {
                    Console.WriteLine($"Error: Config file '{configFile}' exists, use -f to overwrite file.");
                    return 1;
                }

                var srcFile = Path.Combine(srcPath, $"dispatcher_{mode}_{arch}.exe");
                if (!File.Exists(srcFile))
                {
                    Console.WriteLine($"Error: Dispatcher binary '{srcFile}' does not exist.");
                    return 1;
                }

                // Add configuration for extra arguments
                var sbea = new StringBuilder();
                for(int i = 0; i < extraArgs.Length; i++)
                {
                    sbea.AppendFormat(CONFIG_EXTRA_ARG, i, extraArgs[i]);
                }

                // Create configuration string
                var config = CONFIG
                    .Replace("{PATH}", target)
                    .Replace("{EXTRA_ARGS}", sbea.ToString());

                Console.Write($"Copying '{srcFile}' to '{name}'... ");
                try
                {
                    File.Copy(srcFile, name);
                    Console.WriteLine("OK!");
                }
                catch (Exception x)
                {
                    Console.WriteLine($"Failed!\nError: {x.GetType().Name}: {x.Message}");
                    return 1;
                }

                Console.Write($"Writing config file '{configFile}'...");
                try
                {
                    using (var fs = new FileStream(configFile, FileMode.Create))
                    using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
                    {
                        sw.Write(config);
                        Console.WriteLine("OK!");
                    }
                }
                catch (Exception x)
                {
                    Console.WriteLine($"Failed!\nError: {x.GetType().Name}: {x.Message}");
                    return 1;
                }
            }

            return 0;
        }

        private static void printUsage()
        {
            Console.WriteLine(USAGE);
        }

        private static void printVersion()
        {
            var _v = Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine($"mkdisp v{_v.Major}.{_v.Minor}.{_v.Revision}");
        }

        private static bool processArgs(string[] args)
        {
            var anonArgs = new List<string>();

            foreach (var arg in args)
            {
                if (arg[0] == '-')
                {
                    switch (arg[1])
                    {
                        case '-':
                            switch (arg.Substring(2))
                            {
                                case "x32": arch = "32"; break;
                                case "x64": arch = "64"; break;
                                default:
                                    Console.WriteLine($"Error: Unknown argument '{arg}'.");
                                    return false;
                            }
                            break;
                        case 'w': mode = "win"; break;
                        case 'c': mode = "cmd"; break;
                        case 'f': force = true; break;
                        case 'h':
                            printUsage();
                            info = true;
                            return true;
                        case 'V':
                            printVersion();
                            info = true;
                            return true;

                    }
                }
                else
                {
                    anonArgs.Add(arg);
                }
            }

            if(string.IsNullOrEmpty(destPath) && anonArgs.Count < 2)
            {
                Console.WriteLine("Error: No NAME specified and DESTPATH is not configured.");
                return false;
            }

            if(anonArgs.Count < 1)
            {
                Console.WriteLine("Error: No TARGET specified.");
                return false;
            }

            if (anonArgs.Count > 2)
            {
                Console.WriteLine("Error: Too many arguments.");
                return false;
            }

            var targetArgs = anonArgs[0].Split(' ');

            target = targetArgs[0];
            extraArgs = targetArgs.Skip(1).ToArray();

            target = Path.GetFullPath(target);
            name = (anonArgs.Count == 2) ? anonArgs[1] : destPath;

            if(Directory.Exists(name))
            {
                name = Path.Combine(name, Path.GetFileName(target));
            }
            else
            {
                if(!name.Contains("\\"))
                {
                    if(string.IsNullOrEmpty(destPath))
                    {
                        Console.WriteLine($"Error: DESTPATH is not set. If you want to use the current directory, use .\\{name}");
                        return false;
                    }
                    else
                    {
                        name = Path.Combine(destPath, name);
                    }
                }
            }

            if (Path.GetExtension(name) != ".exe")
                name += ".exe";


            return true;
        }
    }
}
