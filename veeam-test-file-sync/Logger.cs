namespace veeam_test_file_sync;

public class Logger
{
    private string _logPath;
    private int _errorCount = 0;
    private int _foldersAdded = 0;
    private int _filesRemoved = 0;
    private int _foldersRemoved = 0;
    private int _filesUpdated = 0;
    private int _filesCreated = 0;
    
    public Logger(string logPath)
    {
        _logPath = logPath;
        if (!File.Exists(_logPath))
        {
            File.Create(Path.GetDirectoryName(_logPath) + "log.txt").Close();
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
        _foldersAdded++;
        string output = $"{Now()} | Action=Create | Type=Folder | path={path}";
        Console.WriteLine(output);
        WriteIntoLog(output);
    }

    public void LogFileCreate(string path)
    {
        _filesCreated++;
        string output = $"{Now()} | Action=Create | Type=File | path={path}";
        Console.WriteLine(output);
        WriteIntoLog(output);
    }

    public void LogFileDelete(string path)
    {
        _filesRemoved++;
        string output = $"{Now()} | Action=Remove | Type=File | path={path}";
        Console.WriteLine(output);
        WriteIntoLog(output);
    }

    public void LogFileUpdate(string path)
    {
        _filesUpdated++;
        string output = $"{Now()} | Action=Update | Type=File | path={path}";
        Console.WriteLine(output);
        WriteIntoLog(output);
    }

    public void Error(string error)
    {
        _errorCount++;
        string output = Now() + " | ERROR | " + error;
        Console.WriteLine(output);
        WriteIntoLog(output);
    }

    public void LogStart()
    {   
        _foldersAdded = 0; _foldersRemoved = 0;
        _filesCreated = 0; _filesRemoved = 0;
        _filesUpdated = 0;
        string output = Now() + " | SYNC START";
        Console.WriteLine(output);
        WriteIntoLog(output);
    }
    public void LogEnd()
    {
        string output = Now() + $" | SYNC END | " +
                        $"{(_filesCreated > 0? "FILES_CREATED=" + _filesCreated + " | " : "")}" +
                        $"{(_filesUpdated > 0? "FILES_UPDATED=" + _filesUpdated + " | " : "")}" +
                        $"{(_filesRemoved > 0? "FILES_REMOVED=" + _filesRemoved + " | " : "")}" +
                        $"{(_foldersAdded > 0? "FOLDERS_CREATED=" + _foldersAdded + " | " : "")}" +
                        $"{(_foldersRemoved > 0? "FOLDERS_REMOVED=" + _foldersRemoved + " | " : "")}" +
                        $"ERROR COUNT={_errorCount}";
        Console.WriteLine(output);
        WriteIntoLog(output);
        _errorCount = 0;
    }

}