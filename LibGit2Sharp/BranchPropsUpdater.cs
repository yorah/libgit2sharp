using System;
using LibGit2Sharp.Core;

namespace LibGit2Sharp
{
    /// <summary>
    ///   Exposes properties of a branch that can be updated.
    /// </summary>
    public class BranchPropsUpdater
    {
        private readonly Repository repo;
        private readonly Branch branch;

        internal BranchPropsUpdater(Repository repo, Branch branch)
        {
            Ensure.ArgumentNotNull(repo, "repo");
            Ensure.ArgumentNotNull(branch, "branch");

            this.repo = repo;
            this.branch = branch;
        }

        /// <summary>
        ///   Sets the upstream information for the branch.
        ///   <para>
        ///     Passing null or string.Empty will unset the upstream.
        ///   </para>
        /// </summary>
        public string Upstream
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    UnsetUpstream();
                    return;
                }

                SetUpstream(value);
            }
        }

        private void UnsetUpstream()
        {
            repo.Config.Unset(string.Format("branch.{0}.remote", branch.Name));
            repo.Config.Unset(string.Format("branch.{0}.merge", branch.Name));
        }

        private void SetUpstream(string value)
        {
            if (branch.IsRemote)
            {
                throw new LibGit2SharpException("Cannot set upstream branch on a remote branch.");
            }

            string remoteName;
            string branchName;

            if (TryParseRemoteBranch(value, out remoteName, out branchName))
            {
                if (!remoteName.Equals(".", StringComparison.Ordinal))
                {
                    // Verify that remote exists.
                    if (repo.Remotes[remoteName] == null)
                    {
                        throw new LibGit2SharpException(string.Format("Could not find remote '{0}' for branch '{1}'.", remoteName, value));
                    }
                }

                repo.Config.Set(string.Format("branch.{0}.remote", branch.Name), remoteName);
                repo.Config.Set(string.Format("branch.{0}.merge", branch.Name), string.Format("refs/heads/{0}", branchName));
            }
            else
            {
                throw new LibGit2SharpException(string.Format("Unable to parse '{0}' into a remote and branch name.", value));
            }
        }

        /// <summary>
        ///   Try to parse a Canonical branch name
        /// </summary>
        private static bool TryParseRemoteBranch(string canonicalName, out string remoteName, out string branchName)
        {
            remoteName = null;
            branchName = null;

            const string localPrefix = "refs/heads/";
            const string remotePrefix = "refs/remotes/";

            if (canonicalName.StartsWith(localPrefix, StringComparison.Ordinal))
            {
                remoteName = ".";
                branchName = canonicalName.Substring(localPrefix.Length);

                return true;
            }

            if (canonicalName.StartsWith(remotePrefix, StringComparison.Ordinal))
            {
                int remoteNameEnd = canonicalName.IndexOf('/', remotePrefix.Length);

                remoteName = canonicalName.Substring(remotePrefix.Length, remoteNameEnd - remotePrefix.Length);
                branchName = canonicalName.Substring(remoteNameEnd);

                return true;
            }

            return false;
        }
    }
}
