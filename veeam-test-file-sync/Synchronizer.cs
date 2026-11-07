namespace veeam_test_file_sync;
using System.IO;

public class Synchronizer
{
    
    private readonly FileManager _fm;
    private readonly Comparator _comparator;
    public Synchronizer(Logger logger, FileManager fm)
    {
        _fm = fm;
        _comparator  = new Comparator(logger);
    }
    
    private string[] GetRelativePaths(string relativeTo, string[] paths)
    {
        string[] relativePaths = new string[paths.Length];
        for (int i = 0; i < paths.Length; i++)
        {
            relativePaths[i] = Path.GetRelativePath(relativeTo, paths[i]);
        }
        return relativePaths;
    }

    private string[] GetFullPaths(string relativeTo, string[] paths)
    {
        string[] fullPaths = new string[paths.Length];
        for (int i = 0; i < paths.Length; i++)
        {
            fullPaths[i] = Path.Combine(relativeTo, paths[i]);
        }
        return fullPaths;
    }
    
    public void Synchronize(string sourcePath, string targetPath)
    {
        string [] sourceFiles = GetRelativePaths(sourcePath, Directory.GetFiles(sourcePath, "*", SearchOption.TopDirectoryOnly));
        string [] targetFiles = GetRelativePaths(targetPath, Directory.GetFiles(targetPath, "*", SearchOption.TopDirectoryOnly));
        
        string[] sourceFolders = GetRelativePaths(sourcePath,Directory.GetDirectories(sourcePath, "*", SearchOption.TopDirectoryOnly));
        string[] targetFolders = GetRelativePaths(targetPath,Directory.GetDirectories(targetPath, "*", SearchOption.TopDirectoryOnly));
        if(sourceFiles.Length == 0 && sourceFolders.Length == 0 && targetFiles.Length == 0 && targetFolders.Length == 0) return;
        
        string [] filesToAdd = sourceFiles.Except(targetFiles).ToArray();
        string [] filesToRemove = targetFiles.Except(sourceFiles).ToArray();
        string [] filesToCheck = sourceFiles.Intersect(targetFiles).ToArray();
        
        string [] foldersToAdd = sourceFolders.Except(targetFolders).ToArray();
        string [] foldersToRemove = targetFolders.Except(sourceFolders).ToArray();
        string[] filesToCopy = _comparator.Compare(GetFullPaths(sourcePath, filesToCheck), GetFullPaths(targetPath, filesToCheck));
        
        _fm.RemoveFolders(GetFullPaths(targetPath, foldersToRemove));
        _fm.AddFolders(GetFullPaths(targetPath, foldersToAdd));
        
        _fm.CopyFiles(GetFullPaths(sourcePath,filesToAdd), GetFullPaths(targetPath, filesToAdd), "Create");
        _fm.CopyFiles(GetFullPaths(sourcePath, filesToCopy), GetFullPaths(targetPath, filesToCopy), "Update");
        _fm.RemoveFiles(GetFullPaths(targetPath,filesToRemove));
        
        _comparator.RemoveFileCaches(GetFullPaths(sourcePath, filesToRemove), GetFullPaths(targetPath, filesToRemove));
        foreach (string sourceFolder in sourceFolders)
        {
            Synchronize(Path.Combine(sourcePath, sourceFolder), Path.Combine(targetPath, sourceFolder));
        }
        
        
    }
}