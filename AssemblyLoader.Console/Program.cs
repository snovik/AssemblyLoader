using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AssemblyLoader.Console.Properties;
using MathLibrary;
using TextLibrary;

namespace AssemblyLoader.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ResolveAssemblies<BinaryBlobs>();

            Start();
        }

        public static void Start()
        {
            MathClass math = new MathClass();
            System.Console.WriteLine(math.Addition(2, 3));

            TextClass text = new TextClass();
            System.Console.WriteLine(@"Hello World");
        }
    }
}
