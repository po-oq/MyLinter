namespace MyLinter.Git;

using LibGit2Sharp;

public class GitHelper
{
    private readonly string repoPath;

    public GitHelper(string repoPath)
    {
        this.repoPath = repoPath;
    }

    // ファイルステータス付きで取得
    public IEnumerable<GitFileInfo> GetStagedFilesWithStatus()
    {
        using var repo = new Repository(repoPath);
        var status = repo.RetrieveStatus();

        return status
            .Where(s => s.State.HasFlag(FileStatus.NewInIndex) ||
                       s.State.HasFlag(FileStatus.ModifiedInIndex))
            .Select(s => new GitFileInfo
            {
                FilePath = Path.GetFullPath(Path.Combine(repoPath, s.FilePath)),
                IsNew = s.State.HasFlag(FileStatus.NewInIndex),
                IsModified = s.State.HasFlag(FileStatus.ModifiedInIndex),
                State = s.State
            });
    }

    // コミットで追加されたファイルを取得
    public IEnumerable<GitFileInfo> GetCommittedFilesWithStatus()
    {
        using var repo = new Repository(repoPath);
        var commit = repo.Head.Tip;
        var parent = commit.Parents.FirstOrDefault();

        if (parent == null)
        {
            // 初回コミットの場合は全て新規
            return commit.Tree
                .Select(e => new GitFileInfo
                {
                    FilePath = Path.GetFullPath(Path.Combine(repoPath, e.Path)),
                    IsNew = true,
                    IsModified = false
                });
        }

        // 差分を取得
        var diff = repo.Diff.Compare<TreeChanges>(parent.Tree, commit.Tree);

        return diff.Select(change => new GitFileInfo
        {
            FilePath = Path.GetFullPath(Path.Combine(repoPath, change.Path)),
            IsNew = change.Status == ChangeKind.Added,
            IsModified = change.Status == ChangeKind.Modified,
            IsDeleted = change.Status == ChangeKind.Deleted,
            State = ConvertToFileStatus(change.Status)
        })
        .Where(f => !f.IsDeleted);
    }

    private FileStatus ConvertToFileStatus(ChangeKind changeKind)
    {
        return changeKind switch
        {
            ChangeKind.Added => FileStatus.NewInIndex,
            ChangeKind.Modified => FileStatus.ModifiedInIndex,
            ChangeKind.Deleted => FileStatus.DeletedFromIndex,
            _ => FileStatus.Unaltered
        };
    }
}
