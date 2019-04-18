using System;

namespace NETWeasel.Windows
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                var netWeasel = new Weasel();
                netWeasel.Run(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }

            return 0;
        }
    }
}
