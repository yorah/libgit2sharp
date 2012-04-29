using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp.Core;
using LibGit2Sharp.Core.Handles;

namespace LibGit2Sharp
{
    public class NoteCollection : IEnumerable<Note>
    {
        private readonly ObjectId commitOid;
        private readonly Repository repo;
        private const string refsNotesPrefix = "refs/notes/";
        private const string defaultRefsNotesNamespace = "refs/notes/commits";

        internal NoteCollection(ObjectId commitOid, Repository repo)
        {
            this.commitOid = commitOid;
            this.repo = repo;
        }

        #region Implementation of IEnumerable

        public IEnumerator<Note> GetEnumerator()
        {
            return (from reference in repo.Refs 
                    where reference.CanonicalName.StartsWith(refsNotesPrefix) 
                    select RetrieveNote(reference.CanonicalName) into note where note != null select note).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public Note Default
        {
            get
            {
                return RetrieveNote(defaultRefsNotesNamespace);
            }
        }

        public Note this[string @namespace]
        {
            get 
            {
                Ensure.ArgumentNotNullOrEmptyString(@namespace, "name");
                var canonicalRefsNamespace = NormalizeToCanonicalName(@namespace);
                return RetrieveNote(canonicalRefsNamespace);
            }
        }

        private Note RetrieveNote(string refsNotesNamespace)
        {
            NoteSafeHandle noteHandle;
            var oid = commitOid.Oid;
            int res = NativeMethods.git_note_read(out noteHandle, repo.Handle, refsNotesNamespace, ref oid);

            if (res == (int)GitErrorCode.GIT_ENOTFOUND)
            {
                return null;
            }

            Ensure.Success(res);
            return Note.BuildFromPtr(UnCanonicalizeName(refsNotesNamespace), noteHandle);
        }

        private static string NormalizeToCanonicalName(string name)
        {
            Ensure.ArgumentNotNullOrEmptyString(name, "name");

            if (name.StartsWith(refsNotesPrefix, StringComparison.Ordinal))
            {
                return name;
            }

            return string.Concat(refsNotesPrefix, name);
        }

        private static string UnCanonicalizeName(string name)
        {
            Ensure.ArgumentNotNullOrEmptyString(name, "name");

            if (!name.StartsWith(refsNotesPrefix, StringComparison.Ordinal))
            {
                return name;
            }

            return name.Substring(refsNotesPrefix.Length);
        }

        public Note Add(string message, Signature author, Signature committer)
        {
            return Add(message, author, committer, defaultRefsNotesNamespace);
        }

        public Note Add(string message, Signature author, Signature committer, string @namespace)
        {
            Ensure.ArgumentNotNullOrEmptyString(message, "message");
            Ensure.ArgumentNotNull(author, "author");
            Ensure.ArgumentNotNull(committer, "committer");
            Ensure.ArgumentNotNullOrEmptyString(@namespace, "@namespace");

            string canonicalName = NormalizeToCanonicalName(@namespace);

            GitOid noteOid;
            GitOid oid = commitOid.Oid;

            Ensure.Success(NativeMethods.git_note_create(out noteOid, repo.Handle, author.BuildHandle(), committer.BuildHandle(), canonicalName, ref oid, message));

            return this[canonicalName];
        }


        public void Delete(Signature author, Signature committer)
        {
            Delete(defaultRefsNotesNamespace, author, committer);
        }

        public void Delete(string @namespace, Signature author, Signature committer)
        {
            Ensure.ArgumentNotNull(author, "author");
            Ensure.ArgumentNotNull(committer, "committer");
            Ensure.ArgumentNotNullOrEmptyString(@namespace, "@namespace");

            GitOid oid = commitOid.Oid;
            var res = NativeMethods.git_note_remove(repo.Handle, NormalizeToCanonicalName(@namespace), author.BuildHandle(), committer.BuildHandle(), ref oid);

            if (res == (int)GitErrorCode.GIT_ENOTFOUND)
            {
                return;
            }

            Ensure.Success(res);
        }
    }
}
