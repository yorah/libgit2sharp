using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using LibGit2Sharp.Core;

namespace LibGit2Sharp
{
    /// <summary>
    ///   The collection of submodules in a <see cref = "Repository" />
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SubmoduleCollection : IEnumerable<Submodule>
    {
        internal readonly Repository repo;

        /// <summary>
        ///   Needed for mocking purposes.
        /// </summary>
        protected SubmoduleCollection()
        { }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LibGit2Sharp.SubmoduleCollection" /> class.
        /// </summary>
        /// <param name = "repo">The repo.</param>
        internal SubmoduleCollection(Repository repo)
        {
            this.repo = repo;
        }

        /// <summary>
        ///   Gets the <see cref = "LibGit2Sharp.Submodule" /> with the specified name.
        /// </summary>
        public virtual Submodule this[string name]
        {
            get
            {
                Ensure.ArgumentNotNullOrEmptyString(name, "name");

                return Submodule.BuildFromPtr(repo.RegisterForCleanup(Proxy.git_submodule_lookup(repo.Handle, name)), name);
            }
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An <see cref = "IEnumerator{T}" /> object that can be used to iterate through the collection.</returns>
        public IEnumerator<Submodule> GetEnumerator()
        {
            return Proxy.git_submodule_foreach(repo.Handle, (h, n) => Utf8Marshaler.FromNative(n))
                        .Select(n => this[n])
                        .GetEnumerator();
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An <see cref = "IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private string DebuggerDisplay
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture,
                                     "Count = {0}", this.Count());
            }
        }
    }
}
