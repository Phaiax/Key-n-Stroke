using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxKeystrokesUi
{
    public class Log
    {
        static string TagFilter = "";
        static string[] allowed = new string[]{};
        public static void SetTagFilter(string tagfilter)
        {
            TagFilter = tagfilter;
            allowed = TagFilter.Split(new char[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries);
        }

        static bool filter(string tag)
        {
            return tag == TagFilter || TagFilter == "" || allowed.Contains(tag);
        }

        public static void e(string tag, string msg)
        {
            if(filter(tag))
            {
                Console.WriteLine(tag.ToUpper() + " " + msg);
            }
        }

        public static void e(string msg)
        {
            e("ERR", msg);
        }
    }
}
