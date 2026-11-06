namespace veeam_test_file_sync;
using System;
using System.IO;

class Program
{

    static void Main(string[] args)
    {   
        
        string sourcePath = "/home/maksym/Documents/Projects/test-veeam/test";
        string targetPath = "/home/maksym/Documents/Projects/test-veeam/test1";
        string logPath = "/home/maksym/Documents/Projects/test-veeam/log.txt";
        
        Logger logger = new Logger(logPath);
        Synchronizer sc = new Synchronizer(sourcePath, targetPath, logger);
        sc.Synchronize(sourcePath, targetPath);

        /*
        FileManager fm = new FileManager(sourcePath, targetPath);

        string? logFilePath = Path.GetDirectoryName(logPath);
        if (!Path.Exists(logFilePath))
        {
            Console.WriteLine("Please provide a valid log file path");
            return;
        }

        if (!File.Exists(logPath))
        {
            File.Create(logPath).Close();
        }
        Logger l = new Logger(logPath);

        if (!fm.DirectoryExists(sourcePath))
        {
            l.Error("Source directory does not exist");
            return;
        }
        if (!fm.DirectoryExists(targetPath))
        {
            l.Waring("Target directory does not exist, Creating a new one");
            Directory.CreateDirectory(targetPath);
        }
        l.LogStart();
        Scanner sc = new Scanner(sourcePath, targetPath);
        fm.SetLogger(new Logger(logPath));
        sc.Scan();

        fm.RemoveFiles(sc.GetFilesToRemove());
        fm.RemoveFolders(sc.GetFoldersToRemove());
        fm.AddFolders(sc.GetFoldersToAdd());
        fm.AddFiles(sc.GetFilesToAdd(), "Create");
        fm.AddFiles(sc.GetFilesToUpdate(), "Update");

        l.LogEnd();*/
    }
}