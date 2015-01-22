using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxKeystrokesUi
{
    public class UrlOpener
    {
        public static void OpenGithub()
        {
            string url = "https://github.com/Phaiax/PxKeystrokesForScreencasts/";
            ProcessStartInfo si = new ProcessStartInfo(url);
            Process.Start(si);
        }
        public static void OpenGithubIssues()
        {
            string url = "https://github.com/Phaiax/PxKeystrokesForScreencasts/issues";
            ProcessStartInfo si = new ProcessStartInfo(url);
            Process.Start(si);
        }
    }
}
