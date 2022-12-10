using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitner
{
    public static class Tools {
        public static void Print(string value, string end = "\n")
        {
            Console.Write(value + end);
        }
        public static void Print(double value, string end = "\n")
        {
            Console.Write(string.Format("{0:0.000}", value) + end);
        }
        public static void Print()
        {
            Console.Write("\n");
        }
        public static void Print(string[] values, string end = "\n")
        {
            Print("[", end: "");
            foreach (string v in values)
            {
                Print(v, end: ", ");
            }
            Print("]", end: end);
        }
        public static string LPrint(string value, int pad = 20, bool returnAsVariable = false)
        {
            var maxLenght = Math.Min(pad, value.Length);
            var s = value.Substring(0, maxLenght).PadRight(pad);
            if (returnAsVariable)
            {
                return s;
            }
            Console.Write(s) ;
            return null;
        }
        public static void LPrint(double value, int pad = 20 )
        {
            LPrint(string.Format("{0:0.000}", value), pad);
        }

        public static void Print(string[] values, int pad = 20)
        {
            var s = "[";
            foreach (string v in values)
            {
                s += v + ", ";
            }
            s += "]";
            LPrint(s, pad);
        }

        public static void ProgressBar(int value, int size, string prefix = "", string postfix = "", char fill = '#', char space = ' ')
        {
            string bar = new(fill, value);
            string empty = new(space, Math.Max(size - value, 0) );
            Print("\r" + prefix + "[" + bar + empty + "] " + postfix, "");
        }

        public static void ProgressBar(string prefix, int value, int size, string postfix = "", char fill = '#', char space = ' ')
        {
            string bar = new(fill, value);
            string empty = new(space, size - value);
            Print("\r" + prefix + "[" + bar + empty + "] " + postfix, "");
        }

    }
}
