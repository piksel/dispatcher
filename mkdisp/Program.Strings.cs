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
  mkdisp [-h] [-V] [-w|-c] [--x32|--x64] SOURCE [TARGET]
If DESTPATH is configured and TARGET is not a absolute path, the target will be created in DESTPATH.
If TARGET is omitted or is a directory, the base name of the SOURCE file will be used.";

        static string CONFIG = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
  <appSettings>
    <add key=""PATH"" value=""{PATH}""/>
  </appSettings>
</configuration>";
    }
}
