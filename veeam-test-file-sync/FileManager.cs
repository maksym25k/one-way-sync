namespace veeam_test_file_sync;
using System.IO;

public class FileManager
{
    private string _sourcePath;
    private string _targetPath;
    private Logger _logger;


    public FileManager(string sourcePath, string targetPath)
    {
        _sourcePath = sourcePath;
        _targetPath = targetPath;
    }

    public void SetLogger(Logger logger)
    {
        this._logger = logger;
    }

    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public bool ValidPermissions(string path)
    {
        string randFileName = Path.GetRandomFileName();
        string probePath = Path.Combine(_targetPath, randFileName);
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
                string fullTargetPath = Path.Combine(this._targetPath, path);
                File.Delete(fullTargetPath);
                _logger.LogFileDelete(fullTargetPath);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }
    }

    public void AddFiles(string[] paths, string mode)
    {
        foreach (string path in paths)
        {
            try
            {
                string fullTargetPath = Path.Combine(this._targetPath, path);
                string fullSourcePath = Path.Combine(this._sourcePath, path);
                File.Copy(fullSourcePath, fullTargetPath, true);
                File.SetLastWriteTime(fullTargetPath, File.GetLastWriteTime(fullSourcePath)); 
                
                if (mode == "Create")
                {
                    this._logger.LogFileCreate(fullTargetPath);
                }
                else
                {
                    this._logger.LogFileUpdate(fullTargetPath);
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
                Directory.CreateDirectory(Path.Combine(this._targetPath, path));
                _logger.LogFolderCreate(Path.Combine(this._targetPath, path));
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
                Directory.Delete(Path.Combine(this._targetPath, path));
                _logger.LogFolderDelete(Path.Combine(this._targetPath, path));
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }
    }
}

