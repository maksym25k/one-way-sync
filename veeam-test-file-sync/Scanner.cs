namespace veeam_test_file_sync;
using System;
using System.IO;

public class Scanner
{
    private readonly string _sourcePath = "/home/maksym/Documents/Projects/test-veeam/test";
    private readonly string _targetPath = "/home/maksym/Documents/Projects/test-veeam/test1";
    private string[] filesToAdd;
    private string[] filesToRemove;
    private string[] filesToUpdate;
    private string[] foldersToAdd;
    private string[] foldersToRemove;
    private readonly Comparator _comparator;

    public string[] GetFilesToAdd()
    {
        return filesToAdd;
    }

    public string[] GetFilesToRemove()
    {
        return filesToRemove;
    }

    public string[] GetFilesToUpdate()
    {
        return filesToUpdate;
    }

    public string[] GetFoldersToAdd()
    {
        return foldersToAdd;
    }

    public string[] GetFoldersToRemove()
    {
        return foldersToRemove;
    }

public Scanner(string sourcePath, string targetPath)
    {
        this._sourcePath = sourcePath;
        this._targetPath = targetPath;
        this._comparator = new Comparator(sourcePath, targetPath);
    }
    
    string[] TransformToRelative(string[] paths, string relativeTo)
    {   
        string[] relativePaths = new string[paths.Length];
        for (int i = 0; i < relativePaths.Length; i++)
        {
            relativePaths[i] = Path.GetRelativePath(relativeTo,paths[i]);
        }
        return relativePaths;
    }
    
    public void Scan()
    {
    
    string [] sourceDirectories = TransformToRelative(Directory.GetDirectories(_sourcePath, "*", SearchOption.AllDirectories), _sourcePath);
    string [] targetDirectories = TransformToRelative(Directory.GetDirectories(_targetPath, "*", SearchOption.AllDirectories), _targetPath);
    
    string [] sourceFiles= TransformToRelative(Directory.GetFiles(_sourcePath, "*", SearchOption.AllDirectories), _sourcePath);
    string [] targetFiles = TransformToRelative(Directory.GetFiles(_targetPath, "*", SearchOption.AllDirectories), _targetPath);
    
    
    HashSet<string> sourceDirectorySet = new HashSet<string>(sourceDirectories);
    HashSet<string> targetDirectorySet = new HashSet<string>(targetDirectories);
    
    HashSet<string> sourceFileSet = new HashSet<string>(sourceFiles);
    HashSet<string> targetFileSet = new HashSet<string>(targetFiles);

    foldersToAdd = sourceDirectorySet.Except(targetDirectorySet).ToArray();
    foldersToRemove = targetDirectorySet.Except(sourceDirectorySet).ToArray();
    
    filesToAdd = sourceFileSet.Except(targetFileSet).ToArray();
    filesToRemove = targetFileSet.Except(sourceFileSet).ToArray();
    
    string [] filesToCheck = sourceFileSet.Intersect(targetFileSet).ToArray();
    filesToUpdate = this._comparator.Compare(filesToCheck).ToArray();
    
    }

    public override string ToString()
    {
        string foldersToAdd = "Folders to add: " + string.Join(", ", this.foldersToAdd);
        string foldersToRemove = "Folders to remove: " + string.Join(", ", this.foldersToRemove);
        string filesToAdd = "Files that will be added: " + string.Join(", ", this.filesToAdd);
        string filesToRemove = "Files that will be removed: " + string.Join(", ", this.filesToRemove);
        string filesToUpdate = "Files that will be copied over: " + string.Join(", ", this.filesToUpdate);
        
        return string.Join(Environment.NewLine, [foldersToAdd, foldersToRemove, filesToAdd, filesToRemove, filesToUpdate]);
    }
}