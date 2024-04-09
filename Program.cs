using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace content_warning_recover
{
    internal class Program
    {
        static string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        static string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        static string recPath = Path.Combine(localAppData, "temp", "rec");

        static DirectoryInfo recordsPath;
        static List<string> records = new List<string>();

        static void Exit(string reason)
        {
            Console.WriteLine(reason);
            Thread.Sleep(5000);
            Environment.Exit(1);
        }

        static void Main(string[] args)
        {
            Console.Title = "Content Warning Recover";

            if (!Directory.Exists(recPath))
                Exit("Error: record path not found!");

            try
            {
                DirectoryInfo directory = new DirectoryInfo(recPath);
                DirectoryInfo[] subDirectories = directory.GetDirectories();

                if (subDirectories.Length == 0)
                    Exit("Error: records not found!");

                recordsPath = subDirectories
                    .OrderByDescending(d => d.LastWriteTime)
                    .First();
            } catch (Exception ex)
            {
                Exit($"Error: {ex.Message}");
            }

            string[] directories = Directory.GetDirectories(recordsPath.FullName);
            foreach (string directory in directories)
            {
                string recordPath = Path.Combine(directory, "output.webm");
                records.Add(recordPath);
            }

            string[] recordsSorted = records.OrderBy(path => File.GetLastWriteTime(path)).ToArray();
            string savePath = Path.Combine(desktopPath, $"recover_{recordsPath.Name}");

            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            for (int i = 0; i < recordsSorted.Length; i++)
            {
                string saveName = Path.Combine(savePath, $"{i}.webm");
                File.Copy(recordsSorted[i], saveName, true);
            }

            Exit($"Last video saved to desktop as: recover_{recordsPath.Name}");
        }
    }
}
