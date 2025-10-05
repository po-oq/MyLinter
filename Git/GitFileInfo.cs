namespace MyLinter.Git;

using LibGit2Sharp;

public class GitFileInfo
{
    public string FilePath { get; set; }
    public bool IsNew { get; set; }
    public bool IsModified { get; set; }
    public bool IsDeleted { get; set; }
    public FileStatus State { get; set; }
}