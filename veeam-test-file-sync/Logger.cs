namespace veeam_test_file_sync;

public class Logger
{
    private string _logPath;
    
    public Logger(string logPath)
    {
        this._logPath = logPath;
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
        string date = Now();
        string output =$"{Now()} | Action=Remove | Type=Folder path={path}";
        Console.WriteLine(output);
        WriteIntoLog(output);
        
    }
    public void LogFolderCreate(string path)
    {
        string date = Now();
        string output =$"{Now()} | Action=Create | Type=Folder path={path}";
        Console.WriteLine(output);
        WriteIntoLog(output);
    }

    public void LogFileCreate(string path)
    {
        string date = Now();
        string output =$"{Now()} | Action=Copy | Type=File path={path}";
        Console.WriteLine(output);
        WriteIntoLog(output);
    }
    
    public void LogFileDelete(string path)
    {
        string date = Now();
        string output =$"{Now()} | Action=Remove | Type=File path={path}";
        Console.WriteLine(output);
        WriteIntoLog(output);
    }
    
    public void LogFileUpdate(string path)
    {
        string date = Now();
        string output =$"{Now()} | Action=Remove | Type=File path={path}";
        Console.WriteLine(output);
        WriteIntoLog(output);
    }
    
    public void Error(string error)
    {
        Console.WriteLine("ERROR | " + error);
        WriteIntoLog(error);
    }
}