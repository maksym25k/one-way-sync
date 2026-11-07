using System.Runtime.InteropServices.JavaScript;

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
    }

    public bool CompareFile(string sourcePath, string targetPath)
    {
        FileInfo sourceFileInfo = new FileInfo(sourcePath);
        FileInfo targetFileInfo = new FileInfo(targetPath);
        
        if (targetFileInfo.Length != sourceFileInfo.Length) return false;
        if (targetFileInfo.LastWriteTimeUtc != sourceFileInfo.LastWriteTimeUtc) return false;
            
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
