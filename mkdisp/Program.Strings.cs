using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mkdisp
{
    partial class Program
    {
        static string USAGE = @"Usage:
  mkdisp [-h] [-V] [-f] [-w|-c] [--x32|--x64] TARGET [NAME]

If DESTPATH is configured and NAME is not a absolute path, the files will be created in DESTPATH.
If NAME is omitted or is a directory, the base name of the TARGET file will be used.";

        static string CONFIG = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
  <appSettings>
    <add key=""PATH"" value=""{PATH}""/>
{EXTRA_ARGS}
  </appSettings>
</configuration>";

        static string CONFIG_EXTRA_ARG = @"    <add key=""ARGV{0}"" value=""{1}"" />";
    }
}
