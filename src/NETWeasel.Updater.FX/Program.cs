using System;

namespace NETWeasel.Updater.FX
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            Console.ReadLine();

            var bootstrapper = new Bootstrapper();

            bootstrapper.Run(args);

            return 0;
        }
    }
}
