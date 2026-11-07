namespace veeam_test_file_sync;

public class Logger
{
    private string _logPath;
    private int _errorCount = 0;
    
    public Logger(string logPath)
    {
        _logPath = logPath;
        if (!File.Exists(_logPath))
        {
            File.Create(_logPath).Close();
        }
    }

    private string Now()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    private void WriteIntoLog(string log)
    {
        File.AppendAllText(_logPath, log + Environment.NewLine);
    }

    public void LogFolderDelete(string path)
    {
        string output = $"{Now()} | Action=Remove | Type=Folder path={path}";
        Console.WriteLine(output);
        WriteIntoLog(output);
    }

    public void LogFolderCreate(string path)
    {
        string output = $"{Now()} | Action=Create | Type=Folder path={path}";
        Console.WriteLine(output);
        WriteIntoLog(output);
    }

    public void LogFileCreate(string path)
    {
        string output = $"{Now()} | Action=Create | Type=File path={path}";
        Console.WriteLine(output);
        WriteIntoLog(output);
    }

    public void LogFileDelete(string path)
    {
        string output = $"{Now()} | Action=Remove | Type=File path={path}";
        Console.WriteLine(output);
        WriteIntoLog(output);
    }

    public void LogFileUpdate(string path)
    {
        string output = $"{Now()} | Action=Update | Type=File path={path}";
        Console.WriteLine(output);
        WriteIntoLog(output);
    }

    public void Error(string error)
    {
        string output = Now() + " | ERROR | " + error;
        Console.WriteLine(output);
        WriteIntoLog(output);
    }

    public void LogStart()
    {   
        string output = Now() + " | SYNC START";
        Console.WriteLine(output);
        WriteIntoLog(output);
    }
    public void LogEnd()
    {
        string output = Now() + $" | SYNC END | ERROR COUNT={_errorCount}";
        Console.WriteLine(output);
        WriteIntoLog(output);
        _errorCount = 0;
    }

}