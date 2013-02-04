using System;
using System.Diagnostics;
using System.Globalization;
using LibGit2Sharp.Core;
using LibGit2Sharp.Core.Compat;
using LibGit2Sharp.Core.Handles;

namespace LibGit2Sharp
{
    /// <summary>
    ///   A Submodule.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Submodule : IEquatable<Submodule>
    {
        private static readonly LambdaEqualityHelper<Submodule> equalityHelper =
            new LambdaEqualityHelper<Submodule>(x => x.Name, x => x.HeadCommitId);

        private readonly SubmoduleSafeHandle handle;
        private readonly string name;
        private readonly Lazy<string> lazyPath;

        /// <summary>
        ///   Needed for mocking purposes.
        /// </summary>
        protected Submodule()
        { }

        private Submodule(SubmoduleSafeHandle handle, string name)
        {
            this.handle = handle;
            this.name = name;

            lazyPath = new Lazy<string>(() => Proxy.git_submodule_path(handle));
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
        public virtual ObjectId IndexCommitId { get { return Proxy.git_submodule_index_id(handle); } }

        /// <summary>
        ///   The commit ID for this submodule in the current HEAD tree.
        /// </summary>
        public virtual ObjectId HeadCommitId { get { return Proxy.git_submodule_head_id(handle); } }

        /// <summary>
        ///   The commit ID for this submodule in the current working directory.
        /// </summary>
        public virtual ObjectId WorkDirCommitId { get { return Proxy.git_submodule_wd_id(handle); } }

        /// <summary>
        ///   The URL of the submodule.
        /// </summary>
        public virtual string Url
        {
            get { return Proxy.git_submodule_url(handle); }
        }

        /// <summary>
        ///   The fetchRecurseSubmodules rule for the submodule.
        ///
        ///   Note that at this time, LibGit2Sharp does not honor this setting and the
        ///   fetch functionality current ignores submodules.
        /// </summary>
        public virtual bool FetchRecurseSubmodules
        {
            get { return Proxy.git_submodule_fetch_recurse_submodules(handle); }
        }

        /// <summary>
        ///   The ignore rule of the submodule.
        /// </summary>
        public virtual SubmoduleIgnore Ignore
        {
            get { return Proxy.git_submodule_ignore(handle); }
        }

        /// <summary>
        ///   The update rule of the submodule.
        /// </summary>
        public virtual SubmoduleUpdate Update
        {
            get { return Proxy.git_submodule_update(handle); }
        }

        /// <summary>
        ///   Add current submodule HEAD commit to index of superproject.
        /// </summary>
        public virtual void Stage()
        {
            Stage(true);
        }

        internal virtual void Stage(bool writeIndex)
        {
            Proxy.git_submodule_add_to_index(handle, writeIndex);
        }

        /// <summary>
        ///   Retrieves the state of this submodule in the working directory compared to the staging area and the latest commmit.
        /// </summary>
        /// <returns></returns>
        public virtual SubmoduleStatus RetrieveStatus()
        {
            return Proxy.git_submodule_status(handle);
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

        internal static Submodule BuildFromPtr(SubmoduleSafeHandle handle, string name)
        {
            return handle == null ? null : new Submodule(handle, name);
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
