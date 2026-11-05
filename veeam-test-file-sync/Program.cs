namespace veeam_test_file_sync;
using System;
using System.IO;

class Program
{

    static void Main(string[] args)
    {
        string sourcePath = "/home/maksym/Documents/Projects/test-veeam/test";
        string targetPath = "/home/maksym/Documents/Projects/test-veeam/test3";
        string logPath = "/home/maksym/Documents/Projects/test-veeam/log.txt";
        
        FileManager fm = new FileManager(sourcePath, targetPath);
        
        if (!fm.DirectoryExists(sourcePath))
        {
            Console.WriteLine("Source directory does not exist");
            return;
        }
        if (!fm.DirectoryExists(targetPath))
        {
            Console.WriteLine("Target directory does not exist");
            return;
        }
        if (!fm.FileExists(logPath))
        {
            Console.WriteLine("Please insert valid log file");
            return;
        }
        
        Scanner sc = new Scanner(sourcePath, targetPath);
        fm.SetLogger(new Logger(logPath));
        sc.Scan();
        
        fm.RemoveFiles(sc.GetFilesToRemove());
        fm.RemoveFolders(sc.GetFoldersToRemove());
        fm.AddFolders(sc.GetFoldersToAdd());
        fm.AddFiles(sc.GetFilesToAdd(), "Create");
        fm.AddFiles(sc.GetFilesToUpdate(), "Update");
        
    }
}