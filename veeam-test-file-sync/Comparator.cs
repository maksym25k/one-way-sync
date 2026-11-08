namespace veeam_test_file_sync;
using System.IO;
using System.Security.Cryptography;
using System.Linq;

public class FileCache
{
    public required string Path { get; set; }
    public required byte[] Hash { get; set; }
    public DateTime LastWriteTimeUtc { get; set; }
    public long Size { get; set; }
}

public class Comparator
{
    private Dictionary<string, FileCache> _sourceFilesCache = new();
    private Dictionary<string, FileCache> _targetFilesCache = new();
    private Logger _logger;

    public void RemoveFileCaches(string[] sourcePaths, string[] targetPaths)
    {
        foreach (var sourcePath in sourcePaths)
        {
            _sourceFilesCache.Remove(sourcePath);
        }

        foreach (var targetPath in targetPaths)
        {
            _targetFilesCache.Remove(targetPath);
        }
    }

    public Comparator(Logger logger)
    {
        _logger = logger;
    }

    private FileCache CreateFileCache(FileInfo fi)
    {
        FileCache fc = new FileCache()
        {
            Path = fi.FullName,
            Hash = CalculateMd5(fi),
            LastWriteTimeUtc = fi.LastWriteTimeUtc,
            Size = fi.Length
        };
        return fc;
    }

    private void UpdateFileCache(FileCache fc, FileInfo fi)
    {
        fc.LastWriteTimeUtc = fi.LastWriteTimeUtc;
        fc.Size = fi.Length;
        fc.Hash = CalculateMd5(fi);
    }
    
    
    bool CacheCompare(FileInfo sourceFileInfo, FileInfo targetFileInfo)
    {
        FileCache? sourceFileCache = _sourceFilesCache.GetValueOrDefault(sourceFileInfo.FullName);
        FileCache? targetFileCache = _targetFilesCache.GetValueOrDefault(targetFileInfo.FullName);
        //when files have to be compared in detail, then they are cached first, and after that, their MD5 hash is compared
        try
        {
            if (sourceFileCache == null || targetFileCache == null)
            {
                if (sourceFileCache == null)
                {
                    sourceFileCache = CreateFileCache(sourceFileInfo);
                    _sourceFilesCache.Add(sourceFileInfo.FullName,  sourceFileCache);
                }

                if (targetFileCache == null)
                {
                    targetFileCache = CreateFileCache(targetFileInfo);
                    _targetFilesCache.Add(targetFileInfo.FullName, targetFileCache);
                }
            }

            if (sourceFileCache.Size != sourceFileInfo.Length ||
                sourceFileCache.LastWriteTimeUtc != sourceFileInfo.LastWriteTimeUtc)
            {
                UpdateFileCache(sourceFileCache, sourceFileInfo);
            }
            if (targetFileCache.Size != targetFileInfo.Length ||
                targetFileCache.LastWriteTimeUtc != targetFileInfo.LastWriteTimeUtc)
            {
                UpdateFileCache(targetFileCache, targetFileInfo);
            }
        }
        catch (IOException)
        {
            _logger.Error($"Could not compare files {sourceFileInfo.FullName} to {targetFileInfo.FullName}");
            _sourceFilesCache.Remove(sourceFileInfo.FullName);
            _targetFilesCache.Remove(targetFileInfo.FullName);
            //True because then there will be no attempt to update the files until the next run
            return false;
        }

        return sourceFileCache.Hash.SequenceEqual(targetFileCache.Hash);
    }

    byte[] CalculateMd5(FileInfo fileInfo)
    {
        using var md5 = MD5.Create();
        
        using var fileStream = fileInfo.OpenRead();
        byte[] hash = md5.ComputeHash(fileStream);
        return hash;
        
    }

    private bool CompareFile(string sourcePath, string targetPath)
    {
        FileInfo sourceFileInfo = new FileInfo(sourcePath);
        FileInfo targetFileInfo = new FileInfo(targetPath);
        
        if (targetFileInfo.Length != sourceFileInfo.Length) return false;
        if (targetFileInfo.Length == sourceFileInfo.Length && targetFileInfo.LastWriteTimeUtc == sourceFileInfo.LastWriteTimeUtc) return true;
        
        return CacheCompare(sourceFileInfo, targetFileInfo);
    }

    public string[] Compare(string[] sourcePaths, string[] targetPaths)
    {
        List<string> toCopy = new List<string>();
        for (int i = 0; i < targetPaths.Length; i++)
        {
            if (!this.CompareFile(sourcePaths[i], targetPaths[i]))
            {
                toCopy.Add(Path.GetFileName(sourcePaths[i]));
            }
        }
        return toCopy.ToArray();
    }

}
