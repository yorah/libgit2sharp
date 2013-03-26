using System;
using System.Diagnostics;
using System.Globalization;
using LibGit2Sharp.Core;

namespace LibGit2Sharp
{
    /// <summary>
    ///   A Submodule.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Submodule : IEquatable<Submodule>
    {
        private readonly SubmoduleLazyGroup group;

        private static readonly LambdaEqualityHelper<Submodule> equalityHelper =
            new LambdaEqualityHelper<Submodule>(x => x.Name, x => x.HeadCommitId);

        private readonly string name;
        private readonly ILazy<string> lazyPath;
        private readonly ILazy<ObjectId> lazyIndexCommitId;
        private readonly ILazy<ObjectId> lazyHeadCommitId;
        private readonly ILazy<ObjectId> lazyWorkdirCommitId;
        private readonly ILazy<string> lazyUrl;
        private readonly ILazy<bool> lazyFetchRecurseSubmodulesRule;
        private readonly ILazy<SubmoduleIgnore> lazyIgnoreRule;
        private readonly ILazy<SubmoduleUpdate> lazyUpdateRule;
        private readonly ILazy<SubmoduleStatus> lazyStatus;

        /// <summary>
        ///   Needed for mocking purposes.
        /// </summary>
        protected Submodule()
        { }

        internal Submodule(Repository repo, string name)
        {
            this.name = name;

            group = new SubmoduleLazyGroup(repo, name);
            lazyPath = group.AddLazy(Proxy.git_submodule_path);
            lazyIndexCommitId = group.AddLazy(Proxy.git_submodule_index_id);
            lazyHeadCommitId = group.AddLazy(Proxy.git_submodule_head_id);
            lazyWorkdirCommitId = group.AddLazy(Proxy.git_submodule_wd_id);
            lazyUrl = group.AddLazy(Proxy.git_submodule_url);

            lazyFetchRecurseSubmodulesRule = group.AddLazy(Proxy.git_submodule_fetch_recurse_submodules);
            lazyIgnoreRule = group.AddLazy(Proxy.git_submodule_ignore);
            lazyUpdateRule = group.AddLazy(Proxy.git_submodule_update);
            lazyStatus = group.AddLazy(Proxy.git_submodule_status);
        }

        /// <summary>
        ///   The name of the submodule.
        /// </summary>
        public virtual string Name { get { return name; } }

        /// <summary>
        ///   The path of the submodule.
        /// </summary>
        public virtual string Path { get { return lazyPath.Value; } }

        /// <summary>
        ///   The commit ID for this submodule in the index.
        /// </summary>
        public virtual ObjectId IndexCommitId { get { return lazyIndexCommitId.Value; } }

        /// <summary>
        ///   The commit ID for this submodule in the current HEAD tree.
        /// </summary>
        public virtual ObjectId HeadCommitId { get { return lazyHeadCommitId.Value; } }

        /// <summary>
        ///   The commit ID for this submodule in the current working directory.
        /// </summary>
        public virtual ObjectId WorkDirCommitId { get { return lazyWorkdirCommitId.Value; } }

        /// <summary>
        ///   The URL of the submodule.
        /// </summary>
        public virtual string Url { get { return lazyUrl.Value; } }

        /// <summary>
        ///   The fetchRecurseSubmodules rule for the submodule.
        ///
        ///   Note that at this time, LibGit2Sharp does not honor this setting and the
        ///   fetch functionality current ignores submodules.
        /// </summary>
        public virtual bool FetchRecurseSubmodulesRule
        {
            get { return lazyFetchRecurseSubmodulesRule.Value; }
        }

        /// <summary>
        ///   The ignore rule of the submodule.
        /// </summary>
        public virtual SubmoduleIgnore IgnoreRule { get { return lazyIgnoreRule.Value; } }

        /// <summary>
        ///   The update rule of the submodule.
        /// </summary>
        public virtual SubmoduleUpdate UpdateRule { get { return lazyUpdateRule.Value; } }

        /// <summary>
        ///   Retrieves the state of this submodule in the working directory compared to the staging area and the latest commmit.
        /// </summary>
        /// <returns></returns>
        public virtual SubmoduleStatus RetrieveStatus()
        {
            // Should be a property? Should it be dynamic?
            return lazyStatus.Value;
        }

        /// <summary>
        ///   Determines whether the specified <see cref = "Object" /> is equal to the current <see cref = "Submodule" />.
        /// </summary>
        /// <param name = "obj">The <see cref = "Object" /> to compare with the current <see cref = "Submodule" />.</param>
        /// <returns>True if the specified <see cref = "Object" /> is equal to the current <see cref = "Submodule" />; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Submodule);
        }

        /// <summary>
        ///   Determines whether the specified <see cref = "Submodule" /> is equal to the current <see cref = "Submodule" />.
        /// </summary>
        /// <param name = "other">The <see cref = "Submodule" /> to compare with the current <see cref = "Submodule" />.</param>
        /// <returns>True if the specified <see cref = "Submodule" /> is equal to the current <see cref = "Submodule" />; otherwise, false.</returns>
        public bool Equals(Submodule other)
        {
            return equalityHelper.Equals(this, other);
        }

        /// <summary>
        ///   Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return equalityHelper.GetHashCode(this);
        }

        /// <summary>
        ///   Returns the <see cref = "Name" />, a <see cref = "String" /> representation of the current <see cref = "Submodule" />.
        /// </summary>
        /// <returns>The <see cref = "Name" /> that represents the current <see cref = "Submodule" />.</returns>
        public override string ToString()
        {
            return Name;
        }

        private string DebuggerDisplay
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture,
                    "{0} => {1}", Name, Url);
            }
        }
    }
}
