namespace veeam_test_file_sync;
using System;
using System.IO;

public class Scanner
{
    private readonly string _sourcePath = "/home/maksym/Documents/Projects/test-veeam/test";
    private readonly string _targetPath = "/home/maksym/Documents/Projects/test-veeam/test1";
    private string[]? filesToAdd;
    private string[]? filesToRemove;
    private string[]? filesToUpdate;
    private string[]? foldersToAdd;
    private string[]? foldersToRemove;
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
    

    public void Scan()
    {
        HashSet<string> sourceDirectorySet = new HashSet<string>();
        HashSet<string> targetDirectorySet = new HashSet<string>();
    
        HashSet<string> sourceFileSet = new HashSet<string>();
        HashSet<string> targetFileSet = new HashSet<string>();
        
        foreach (string sourceFolder in Directory.EnumerateDirectories(_sourcePath, "*", SearchOption.AllDirectories))
        {
            sourceDirectorySet.Add(Path.GetRelativePath(_sourcePath, sourceFolder));
        }
        foreach (string targetFolder in Directory.EnumerateDirectories(_targetPath, "*", SearchOption.AllDirectories))
        {
            targetDirectorySet.Add(Path.GetRelativePath(_targetPath, targetFolder));
        }
        
        foreach (string sourceFile in Directory.EnumerateFiles(_sourcePath, "*", SearchOption.AllDirectories))
        {
            sourceFileSet.Add(Path.GetRelativePath(_sourcePath, sourceFile));
        }
        foreach (string targetFile in Directory.EnumerateFiles(_targetPath, "*", SearchOption.AllDirectories))
        {
            targetFileSet.Add(Path.GetRelativePath(_targetPath, targetFile));
        }
        
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