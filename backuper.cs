using System.IO.Compression;
using System.Text.Json;

//TODO: logging and retainment of folder structure
namespace backuptool
{
    public class Logger
    {

    }

    internal class Backuper
    {
        

        public void Start(string way)
        {
            jsontypes Input = JSONStuff(way);
            if (Input == null || Validator(Input.Sources, Input.Target, Input.LogLevel) == 0)
            {
                Console.WriteLine("Invalid data in JSON");
                return;
            }
            string DestFolder = CreateDestFolder(Input.Target), df2 = String.Concat(DestFolder, ".zip");
            Stage1(Input.Sources, DestFolder);
            MakeZIP(DestFolder, df2);
        }

        private static jsontypes? JSONStuff(string way)
        {
            if (!File.Exists(way))
            {
                Console.WriteLine("File not found");
                return null;
            }
            string jsonData = File.ReadAllText(way);
            jsontypes Input;
            try
            {
                Input = JsonSerializer.Deserialize<jsontypes>(jsonData);
            }
            catch (JsonException)
            {
                Console.WriteLine("Wrong JSON");
                return null;
            }
            return Input;
        }

        private static int Validator(string[] sources, string target, int ll)
        {
            bool sourcesIsOK = true;
            try
            {
                if (sources.Length == 0 || sources == null)
                {
                    return 0;
                }
            }
            catch (NullReferenceException)
            {
                return 0;
            }
            foreach (string source in sources)
            {
                if (!Directory.Exists(source))
                {
                    Console.WriteLine("Error: directory {0} not found", source);//TODO: replace to logging
                    sourcesIsOK = false;
                    //return 0;
                }
            }
            if (!Directory.Exists(target))
            {
                Console.WriteLine("Error: target directory not found");//also TODO
                return 0;
            }
            if (!sourcesIsOK) return 0;
            if (ll > 0 && ll < 4) {
                //LogLevel = ll;
                return ll; 
            }
            if (ll > 4) return 1;//since invalid log level was given, set default
            if (ll < 1) return 1;//same here
            return 0;
        }

        private void Stage1(string[] sources, string target)
        {
            foreach (string source in sources)
            {
                Stage2(source, target);
            }
        }

        private void Stage2(string source, string target)
        {
            string[] files = Directory.GetFiles(source);
            foreach (string file in files) CopyFiles(file, target);
            string[] folders = Directory.GetDirectories(source);
            foreach (string folder in folders) Stage2(folder, target);
        }

        private static void CopyFiles(string from, string to)
        {
            string temp = Path.GetFileName(from);
            to = Path.Combine(to, temp);
            try
            {
                File.Copy(from, to, true);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("File {0} is read-only, if you need to update it, do it manually", from);
            }
        }

        private static string CreateDestFolder(string path)
        {
            DateTime dateTime = DateTime.Now;
            string temp = dateTime.ToString("D");
            //Console.WriteLine(temp);
            string newPath = Path.Combine(path, temp);
            Directory.CreateDirectory(newPath);
            //TODO folder creation log
            return newPath;
        }

        private static void MakeZIP(string DestFolder, string df2)
        {
            try
            {
                ZipFile.CreateFromDirectory(DestFolder, df2);
            }
            catch (IOException)
            {
                Console.WriteLine("Archive with such name already exists, creating copy...");
                Random temp = new Random();
                df2 = String.Concat(DestFolder, temp.Next(1000), ".zip");
                ZipFile.CreateFromDirectory(DestFolder, df2);
            }
        }
    }
}
