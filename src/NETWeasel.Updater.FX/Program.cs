using System;

namespace NETWeasel.Updater.FX
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                var bootstrapper = new Bootstrapper();
                bootstrapper.Run(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 1;
            }

            return 0;
        }
    }
}
