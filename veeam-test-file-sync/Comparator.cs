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
    private List<FileCache> _sourceFilesCache = new();
    private List<FileCache> _targetFilesCache = new();
    private string _sourePath;
    private string _targetPath;
    
    
    
    
    public Comparator(string sourcePath, string targetPath)
    {   
        _sourePath = sourcePath;
        _targetPath = targetPath;
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
        /*
         * if there is no corresponding cached file for at least one of the files, we create the record and run full compare, otherwise the same
         * If we know that at least one did not change by checking the metadata, we go, and calculate new hash
         * for the unknown one. If it comes out the same, return true and update the hash, if it comes out different, return false and delete the hash
         */
        FileCache? sourceFileCache = _sourceFilesCache.FirstOrDefault(x => x.Path == sourceFileInfo.FullName);
        FileCache? targetFileCache = _targetFilesCache.FirstOrDefault(x => x.Path == targetFileInfo.FullName);

        if (sourceFileCache == null || targetFileCache == null)
        {
            if (sourceFileCache == null)
            {
                sourceFileCache = CreateFileCache(sourceFileInfo);
                _sourceFilesCache.Add(sourceFileCache);
            }

            if (targetFileCache == null)
            {
                targetFileCache = CreateFileCache(targetFileInfo);
                _targetFilesCache.Add(targetFileCache);
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
    
    /*
    bool Md5Compare(FileInfo targetFileInfo, FileInfo sourceFileInfo)
    {
        using Stream sourceStream = File.OpenRead(sourceFileInfo.FullName);
        using Stream targetStream = File.OpenRead(targetFileInfo.FullName);
        
        int bufferSize = 32 * 1024;
        byte[] sourceBuffer = new byte[bufferSize];
        byte[] targetBuffer = new byte[bufferSize];
        long length = sourceStream.Length;
        long position = 0;
        Console.WriteLine($"Comparing {sourceFileInfo.Name} and {targetFileInfo.Name}");
        while (true)
        {
            int bytesToRead = (int)Math.Min(bufferSize, length - position);
            sourceStream.ReadExactly(sourceBuffer, 0, bytesToRead);
            targetStream.ReadExactly(targetBuffer, 0, bytesToRead);
            
            if(sourceBuffer.SequenceCompareTo(targetBuffer) != 0)
            {
                return false;
            }
            
            position += bytesToRead;
            if (position == length)
            {
                return true;
            }
        }
    }*/

    public bool CompareFile(string sourcePath, string targetPath)
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
