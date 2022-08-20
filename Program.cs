namespace backuptool
{
    class App
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CLI backup app");
            Console.WriteLine();
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: app.exe <path to settings file>");
                return;
            }
            Backuper magic = new Backuper();
            magic.Start(args[0]);


        }
    }
}