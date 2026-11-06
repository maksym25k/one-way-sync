namespace veeam_test_file_sync;
using System.IO;
using System.Security.Cryptography;
using System.Linq;

public class Comparator
{
    private string _sourePath;
    private string _targetPath;
    
    
    public Comparator(string sourcePath, string targetPath)
    {   
        _sourePath = sourcePath;
        _targetPath = targetPath;
    }

    bool Md5Compare(FileInfo targetFileInfo, FileInfo sourceFileInfo)
    {
        using MD5 md5 = MD5.Create();
        using Stream sourceStream = File.OpenRead(sourceFileInfo.FullName);
        using Stream targetStream = File.OpenRead(targetFileInfo.FullName);
        byte[] targetHash = md5.ComputeHash(targetStream);
        byte[] sourceHash = md5.ComputeHash(sourceStream);
        
        return targetHash.SequenceEqual(sourceHash);
    }

    public bool CompareFile(string sourcePath, string targetPath)
    {
        FileInfo sourceFileInfo = new FileInfo(sourcePath);
        FileInfo targetFileInfo = new FileInfo(targetPath);
        
        if (targetFileInfo.Length != sourceFileInfo.Length) return false;
        if (targetFileInfo.LastWriteTime != sourceFileInfo.LastWriteTime) return false;
            
        return Md5Compare(sourceFileInfo, targetFileInfo);
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
