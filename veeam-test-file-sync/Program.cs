using System.ComponentModel;

namespace veeam_test_file_sync;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    private static int interval;
    private static string sourcePath;
    private static string targetPath;
    private static string logPath;
    private static bool syncRunning = false;
    private static bool cancelled = false;
    private static ManualResetEvent mre = new ManualResetEvent(false);
    static void ParseArgs(string[] args)
    {
        if (args.Length < 3)
        {
            throw new Exception("Not enough arguments supplied.");
        }
        
        sourcePath = args[0];
        targetPath = args[1];
        logPath = args[2];

        if (args.Length > 3)
        {
            interval = int.Parse(args[3]);
        }
        else
        {
            interval = 10000;
        }
        
    }
    static void Main(string[] args)
    {
        try
        {
            ParseArgs(args);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return;
        }
        
        FileManager fm = new FileManager();

        if (!fm.DirectoryExists(Path.GetDirectoryName(logPath)))
        {
            Console.WriteLine("Please provide valid path to a log file.");
            return;
        }
        if (!fm.DirectoryExists(sourcePath))
        {
            Console.WriteLine("Please provide valid source path.");
            return;
        }

        if (!fm.DirectoryExists(targetPath))
        {
            Console.WriteLine("Please provide valid target path.");
            return;
        }

        if (!fm.ValidPermissions(targetPath))
        {
            Console.WriteLine("You need read and write permission for the target folder to run this program.");
            return;
        }
        
        Logger logger = new Logger(logPath);
        Synchronizer sc = new Synchronizer(sourcePath, targetPath, logger, fm);
        Console.CancelKeyPress += CancelHandler;
 
        int i = 0;
        while (!cancelled)
        {
            logger.LogStart();
            sc.Synchronize(sourcePath, targetPath);
            logger.LogEnd();
            if (!cancelled)
            {
                mre.WaitOne(interval);
            }
        }

        static void CancelHandler(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine(Environment.NewLine + "Exiting...");
            e.Cancel = true;
            
            cancelled = true;
            mre.Set();
            
            
        }
    }
    
    
}