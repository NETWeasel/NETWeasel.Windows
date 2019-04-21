namespace NETWeasel.Updater
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            var bootstrapper = new Bootstrapper();

            bootstrapper.Run(args);

            return 0;
        }
    }
}
