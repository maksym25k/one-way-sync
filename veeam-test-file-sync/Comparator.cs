using System.Runtime.InteropServices.JavaScript;

namespace veeam_test_file_sync;
using System.IO;
using System.Text.Json;
using System.Security.Cryptography;
using System.Linq;

public class FileCache
{
    public string Path { get; set; }
    public byte[] Hash { get; set; }
    public DateTime LastWriteTimeUtc { get; set; }
    public long Size { get; set; }
}

public class Comparator
{
    private Dictionary<string, FileCache> _sourceFilesCache = new();
    private Dictionary<string, FileCache> _targetFilesCache = new();

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

    public Comparator() {}

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
        /*
         * if there is no corresponding cached file for at least one of the files, we create the record and run full compare, otherwise the same
         * If we know that at least one did not change by checking the metadata, we go, and calculate new hash
         * for the unknown one. If it comes out the same, return true and update the hash, if it comes out different, return false and delete the hash
         */
        FileCache? sourceFileCache = _sourceFilesCache.GetValueOrDefault(sourceFileInfo.FullName);
        FileCache? targetFileCache = _targetFilesCache.GetValueOrDefault(targetFileInfo.FullName);

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
