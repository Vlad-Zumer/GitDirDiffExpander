using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GitDirCompareExpander
{
    class Program
    {

        static Func<int> RunDiffTool(string leftFile, string rightFile, string tool)
        {
            Process process = new Process();
            process.StartInfo.FileName = tool;
            process.StartInfo.Arguments = $"{leftFile} {rightFile}";
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = true;
            return () =>
            {
                process.Start();
                process.WaitForExit();
                return process.ExitCode;
            };
        }

        static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} <LEFT_FOLDER> <RIGHT_FOLDER>");
                Console.WriteLine("Press ENTER to close process.");
                Console.ReadLine();
                return 1;
            }

            const string toolEnvVar = "GitParallelDifftool";

            string tool = Environment.GetEnvironmentVariable(toolEnvVar);
            if (string.IsNullOrWhiteSpace(tool))
            {
                Console.WriteLine($"ENV VAR NOT SET: {toolEnvVar}\nPlease set to a tool that will be used to show the diff files in parrallel.");
                Console.WriteLine("Press ENTER to close process.");
                Console.ReadLine();
                return 1;
            }


            string leftFolder = Path.GetFullPath(args[0]);
            string rightFolder = Path.GetFullPath(args[1]);

            string[] leftFiles = Directory.GetFiles(leftFolder, "*", SearchOption.AllDirectories);
            string[] rightFiles = Directory.GetFiles(rightFolder, "*", SearchOption.AllDirectories);

            List<(string, string)> desiredLeft = leftFiles.Select(file => (file, file.Replace(leftFolder, rightFolder))).ToList();
            List<(string, string)> desiredRight = leftFiles.Select(file => (file.Replace(rightFolder, leftFolder), file)).ToList();

            List<(string, string)> filesToDiff = desiredLeft;

            foreach (var item in desiredRight)
            {
                if (!filesToDiff.Contains(item))
                {
                    filesToDiff.Add(item);
                }
            }

            Task<int>[] diffTasks = filesToDiff.Select(fileTuple =>
           {
               string leftFile = fileTuple.Item1;
               string rightFile = fileTuple.Item2;
               return new Task<int>(RunDiffTool(leftFile, rightFile, tool));
           }).ToArray();

            foreach (var task in diffTasks)
            {
                task.Start();
            }

            Task.WaitAll(diffTasks);

            return diffTasks.Aggregate(0, (acc, next) => { acc |= next.Result; return acc; });

        }
    }
}
