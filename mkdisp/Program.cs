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
        static string source = "";
        static string target = "";
        static bool info = false;
        static bool force = false;

        static int Main(string[] args)
        {
            arch = ConfigurationManager.AppSettings["ARCH"];
            destPath = ConfigurationManager.AppSettings["DESTPATH"];
            srcPath = ConfigurationManager.AppSettings["SRCPATH"];

            if (!processArgs(args))
            {
                printUsage();
                return 1;
            }

            if(!info)
            {
                if (string.IsNullOrEmpty(srcPath))
                {
                    Console.WriteLine("Error: SRCPATH is not configured in mkdisp.exe.config");
                    return 1;
                }

                if (!File.Exists(source))
                {
                    Console.WriteLine($"Error: Source file '{source}' does not exist.");
                    return 1;
                }

                if (!force && File.Exists(target))
                {
                    Console.WriteLine($"Error: Target file '{target}' exists, use -f to overwrite file.");
                    return 1;
                }

                var configFile = target + ".config";
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


                var config = CONFIG.Replace("{PATH}", source);

                Console.Write($"Copying '{srcFile}' to '{target}'... ");
                try
                {
                    File.Copy(srcFile, target);
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
                Console.WriteLine("Error: No TARGET specified and DESTPATH is not configured.");
                return false;
            }

            if(anonArgs.Count < 1)
            {
                Console.WriteLine("Error: No SOURCE specified.");
                return false;
            }

            if (anonArgs.Count > 2)
            {
                Console.WriteLine("Error: Too many arguments.");
                return false;
            }

            source = Path.GetFullPath(anonArgs[0]);
            target = (anonArgs.Count == 2) ? anonArgs[1] : destPath;

            if(Directory.Exists(target))
            {
                target = Path.Combine(target, Path.GetFileName(source));
            }
            else
            {
                if(!target.Contains("\\"))
                {
                    if(string.IsNullOrEmpty(destPath))
                    {
                        Console.WriteLine($"Error: DESTPATH is not set. If you want to use the current directory as target, use .\\{target}");
                        return false;
                    }
                    else
                    {
                        target = Path.Combine(destPath, target);
                    }
                }
            }

            if (Path.GetExtension(name) != ".exe")
                name += ".exe";


            return true;
        }
    }
}
