using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyNStroke
{
    public class UrlOpener
    {
        public static void OpenGithub()
        {
            string url = "https://github.com/Phaiax/Key-n-Stroke/";
            ProcessStartInfo si = new ProcessStartInfo(url);
            Process.Start(si);
        }

        public static void OpenGithubREADME()
        {
            string url = "https://github.com/Phaiax/Key-n-Stroke/blob/master/README.md";
            ProcessStartInfo si = new ProcessStartInfo(url);
            Process.Start(si);
        }

        public static void OpenGithubIssues()
        {
            string url = "https://github.com/Phaiax/Key-n-Stroke/issues";
            ProcessStartInfo si = new ProcessStartInfo(url);
            Process.Start(si);
        }
    }
}
