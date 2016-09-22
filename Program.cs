using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace check_line_endings
{
    [Flags]
    enum Endings
    {
        none,
        binary = 1,
        dos = 2,
        mac = 4,
        unix = 8,
    };

    class Program
    {
        static void help()
        {
            Console.WriteLine("Check what kind of line endings are used in files");
            Console.WriteLine("");
            Console.WriteLine("check-line-endings [patterns]");
            Console.WriteLine("");
            Console.WriteLine("Patterns as in:");
            Console.WriteLine("https://github.com/mganss/Glob.cs");
        }

        static Endings endings(string path)
        {
            byte[] bytes = null;
            try
            {
                bytes = File.ReadAllBytes(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            var r = Endings.none;
            for (int i = 0; i < bytes.Length; i++)
                switch (bytes[i])
                {
                    case 0:
                        return Endings.binary;
                    case (byte)'\n':
                        r |= Endings.unix;
                        break;
                    case (byte)'\r':
                        if (i + 1 < bytes.Length && bytes[i + 1] == '\n')
                        {
                            r |= Endings.dos;
                            i++;
                            break;
                        }
                        r |= Endings.mac;
                        break;
                }
            return r;
        }

        static int Main(string[] args)
        {
            var options = true;
            var patterns = new List<string>();
            foreach (var arg in args)
            {
                var s = arg;
                if (!options)
                {
                    patterns.Add(s);
                    continue;
                }
                if (s == "")
                    continue;
                if (s == "--")
                {
                    options = false;
                    continue;
                }
                if (Path.DirectorySeparatorChar == '\\' && s[0] == '/')
                    s = "-" + s.Substring(1);
                if (s[0] != '-')
                {
                    patterns.Add(s);
                    continue;
                }
                if (s.StartsWith("--"))
                    s = s.Substring(1);
                switch (s)
                {
                    case "-?":
                    case "-h":
                    case "-help":
                        help();
                        return 0;
                    case "-V":
                    case "-v":
                    case "-version":
                        Console.WriteLine("dir-date {0}", Assembly.GetExecutingAssembly().GetName().Version);
                        return 0;
                    default:
                        Console.WriteLine("{0}: unknown option", arg);
                        return 1;
                }
            }
            if (patterns.Count == 0)
                patterns.Add("*");
            var files = new List<string>();
            foreach (var pattern in patterns)
            {
                var expanded = Glob.Glob.ExpandNames(pattern);
                if (expanded.Count() == 0)
                {
                    files.Add(pattern);
                    continue;
                }
                foreach (var file in expanded)
                    files.Add(file);
            }
            foreach (var path in files)
            {
                if (Directory.Exists(path))
                    continue;
                var e = endings(path);
                var s = "Mixed";
                switch (e)
                {
                    case Endings.none:
                        s = "None";
                        break;
                    case Endings.binary:
                        s = "Binary";
                        break;
                    case Endings.dos:
                        s = "DOS";
                        break;
                    case Endings.mac:
                        s = "OldMac";
                        break;
                    case Endings.unix:
                        s = "Unix";
                        break;
                }
                Console.WriteLine("{0,-6} {1}", s, path);
            }
            return 0;
        }
    }
}
