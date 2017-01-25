namespace Asynger
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.IO;

    class AsyncExecuter
    {
        public static void Execute(IEnumerable<string> ACommandLines)
        {
            var taskList = new List<Task<Process>>();

            foreach (var commandLine in ACommandLines)
            {
                var task = new Task<Process>(
                    () => 
                    {
                        var idxFirstSpace = commandLine.IndexOf(' ');
                        if (idxFirstSpace > 0)
                        {
                            var info = new ProcessStartInfo();
                            info.FileName = commandLine.Substring(0, idxFirstSpace);
                            info.Arguments = commandLine.Substring(idxFirstSpace + 1);
                            info.WindowStyle = ProcessWindowStyle.Hidden;

                            return Process.Start(info);
                        }
                        else
                        {
                            return Process.Start(commandLine);
                        }
                    });
                task.Start();
                taskList.Add(task);
            }

            Task.WaitAll(taskList.ToArray());

            foreach (var task in taskList)
            {
                task.Result.WaitForExit();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                try
                {
                    var Lines = File.ReadLines(args[0]);

                    AsyncExecuter.Execute(Lines);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                Console.WriteLine("Asynger.exe <file.txt>");
            }
        }
    }
}
