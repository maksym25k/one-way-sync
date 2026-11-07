namespace veeam_test_file_sync;
using System.IO;

public class FileManager
{
    private Logger _logger;


    public FileManager() { }

    public void SetLogger(Logger logger)
    {
        _logger = logger;
    }

    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    public bool ValidPermissions(string path)
    {
        string randFileName = Path.GetRandomFileName();
        string probePath = Path.Combine(path, randFileName);
        try
        {
            File.WriteAllText(probePath, "Test");
            File.Delete(probePath);
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }

        return true;
    }


    public void RemoveFiles(string[] paths)
    {
        foreach (string path in paths)
        {
            try
            {
                File.Delete(path);
                _logger.LogFileDelete(path);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }
    }

    public void CopyFiles(string[] sourcePaths, string[] targetPaths,string mode)
    {
        for (int i = 0; i < targetPaths.Length; i++)
        {
            try
            {
                File.Copy(sourcePaths[i], targetPaths[i], true);
                File.SetLastWriteTime(sourcePaths[i], File.GetLastWriteTime(targetPaths[i])); 
                
                if (mode == "Create")
                {
                    _logger.LogFileCreate(targetPaths[i]);
                }
                else
                {
                    _logger.LogFileUpdate(targetPaths[i]);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }
    }

    public void AddFolders(string[] paths)
    {
        string[] ascSortedPaths = paths.OrderBy(x => x.Length).ToArray();
        foreach (string path in ascSortedPaths)
        {
            try
            {
                Directory.CreateDirectory(path);
                _logger.LogFolderCreate(path);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }
    }

    public void RemoveFolders(string[] paths)
    {
        string[] descSortedPaths = paths.OrderByDescending(x => x.Length).ToArray();
        foreach (string path in descSortedPaths)
        {
            try
            {
                Directory.Delete(path, true);
                _logger.LogFolderDelete(path);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }
    }
}

