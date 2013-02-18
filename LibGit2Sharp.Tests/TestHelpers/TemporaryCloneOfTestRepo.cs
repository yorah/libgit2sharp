using System.IO;

namespace LibGit2Sharp.Tests.TestHelpers
{
    public class TemporaryCloneOfTestRepo
    {
        public TemporaryCloneOfTestRepo(IPostTestDirectoryRemover directoryRemover, string sourceDirectoryPath, params string[] additionalSourcePaths)
        {
            var scd = new SelfCleaningDirectory(directoryRemover);
            var source = new DirectoryInfo(sourceDirectoryPath);

            RepositoryPath = Path.Combine(scd.DirectoryPath, source.Name);

            DirectoryHelper.CopyFilesRecursively(source, new DirectoryInfo(RepositoryPath));

            foreach (var additionalPath in additionalSourcePaths)
            {
                var additional = new DirectoryInfo(additionalPath);
                var targetForAdditional = Path.Combine(scd.DirectoryPath, additional.Name);

                DirectoryHelper.CopyFilesRecursively(additional, new DirectoryInfo(targetForAdditional));
            }
        }

        public string RepositoryPath { get; private set; }
    }
}
