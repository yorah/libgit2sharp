using System.IO;

namespace LibGit2Sharp.Tests.TestHelpers
{
    public class TemporaryCloneOfTestRepo
    {
        public TemporaryCloneOfTestRepo(IPostTestDirectoryRemover directoryRemover, string sourceDirectoryPath)
        {
            var scd = new SelfCleaningDirectory(directoryRemover);
            var source = new DirectoryInfo(sourceDirectoryPath);

            RepositoryPath = Path.Combine(scd.DirectoryPath, source.Name);

            DirectoryHelper.CopyFilesRecursively(source, new DirectoryInfo(RepositoryPath));
        }

        public string RepositoryPath { get; private set; }
    }
}
