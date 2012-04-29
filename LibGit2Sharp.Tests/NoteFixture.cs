using System;
using System.Linq;
using LibGit2Sharp.Core;
using LibGit2Sharp.Tests.TestHelpers;
using Xunit;

namespace LibGit2Sharp.Tests
{
    public class NoteFixture : BaseFixture
    {
        //* get default note from commit
        //* get all notes (all namespaces, equivalent to --show-notes=*)
        //* indexer with namespace
        //* add a note on a commit
        //* add a note with a namespace on a commit
        //* delete a note from a commit
        //* modify a note

        // TODO we might want to order the notes with the author signature date. If yes, this info should be returned by libgit2?

        private static readonly Signature signatureNullToken = new Signature("nulltoken", "emeric.fermas@gmail.com", DateTimeOffset.UtcNow);
        private static readonly Signature signatureYorah = new Signature("yorah", "yoram.harmelin@gmail.com", Epoch.ToDateTimeOffset(1300557894, 60));

        /*
         * $ git log 8496071c1b46c854b31185ea97743be6a8774479
         * commit 8496071c1b46c854b31185ea97743be6a8774479
         * Author: Scott Chacon <schacon@gmail.com>
         * Date:   Sat May 8 16:13:06 2010 -0700
         *
         *     testing
         *
         * Notes:
         *     Hi, I'm Note.
         */
        [Fact]
        public void CanRetrieveANoteOnDefaultNamespaceFromACommit()
        {
            using (var repo = new Repository(BareTestRepoPath))
            {
                var commit = repo.Lookup<Commit>("8496071c1b46c854b31185ea97743be6a8774479");
                var defaultNote = commit.Notes.Default;

                Assert.Equal("Hi, I'm Note.\n", defaultNote.Message);
                Assert.Equal("commits", defaultNote.Namespace);
            }
        }

        [Fact]
        public void CanRetrieveADefaultNoteByIndexerUsingCanonicalNamespaceOrAbbreviatedNamespace()
        {
            using (var repo = new Repository(BareTestRepoPath))
            {
                var commit = repo.Lookup<Commit>("8496071c1b46c854b31185ea97743be6a8774479");
                var defaultNote = commit.Notes.Default;

                Assert.Equal(defaultNote, commit.Notes["commits"]);
                Assert.Equal(defaultNote, commit.Notes["refs/notes/commits"]);
            }
        }

        [Fact]
        public void RetrievingANoteFromANonExistingNamespaceReturnsNull()
        {
            using (var repo = new Repository(BareTestRepoPath))
            {
                var commit = repo.Lookup<Commit>("8496071c1b46c854b31185ea97743be6a8774479");
                var note = commit.Notes["ironman"];

                Assert.Null(note);
            }
        }

        /*
         * $ git show 5b5b025afb0b4c913b4c338a42934a3863bf3644 --notes=answer
         * commit 5b5b025afb0b4c913b4c338a42934a3863bf3644
         * Author: Scott Chacon <schacon@gmail.com>
         * Date:   Tue May 11 13:38:42 2010 -0700
         * 
         *     another commit
         * 
         * Notes (answer):
         *     Not what?
         */
        [Fact]
        public void CanRetrieveANoteOnACustomNamespace()
        {
            using (var repo = new Repository(BareTestRepoPath))
            {
                var commit = repo.Lookup<Commit>("5b5b025afb0b4c913b4c338a42934a3863bf3644");
                var defaultNote = commit.Notes.Default;
                Assert.Null(defaultNote);

                var answerNote = commit.Notes["answer"];

                Assert.Equal("Not what?\n", answerNote.Message);
                Assert.Equal("answer", answerNote.Namespace);
            }
        }

        /*
         * $ git show 4a202b346bb0fb0db7eff3cffeb3c70babbd2045 --show-notes=*
         * commit 4a202b346bb0fb0db7eff3cffeb3c70babbd2045
         * Author: Scott Chacon <schacon@gmail.com>
         * Date:   Mon May 24 10:19:04 2010 -0700
         * 
         *     a third commit
         * 
         * Notes:
         *     Just Note, don't you understand?
         * 
         * Notes (answer):
         *     Nope
         *     
         * Notes (answer2):
         *     Not Nope, Note!
         */
        [Fact]
        public void CanIterateThroughTheNotesOfACommit()
        {
            var expectedNotes = new[] { "Nope\n", "Not Nope, Note!\n", "Just Note, don't you understand?\n" };

            using (var repo = new Repository(BareTestRepoPath))
            {
                var commit = repo.Lookup<Commit>("4a202b346bb0fb0db7eff3cffeb3c70babbd2045");

                Assert.Equal(expectedNotes, commit.Notes.Select(n => n.Message).ToArray());
            }
        }

        [Fact]
        public void CanAddADefaultNoteOnACommit()
        {
            TemporaryCloneOfTestRepo path = BuildTemporaryCloneOfTestRepo();
            using (var repo = new Repository(path.RepositoryPath))
            {
                var commit = repo.Lookup<Commit>("9fd738e8f7967c078dceed8190330fc8648ee56a");

                var note = commit.Notes.Add("woot!\n", signatureNullToken, signatureYorah);

                var defaultNote = commit.Notes.Default;
                Assert.Equal(note, defaultNote);

                Assert.Equal("woot!\n", defaultNote.Message);
                Assert.Equal("commits", defaultNote.Namespace);
            }
        }

        [Fact]
        public void CanAddANoteWithACustomNamespaceOnACommit()
        {
            TemporaryCloneOfTestRepo path = BuildTemporaryCloneOfTestRepo();
            using (var repo = new Repository(path.RepositoryPath))
            {
                var commit = repo.Lookup<Commit>("9fd738e8f7967c078dceed8190330fc8648ee56a");

                var note = commit.Notes.Add("I'm batman!\n", signatureNullToken, signatureYorah, "batmobile");
                
                var batmobileNote = commit.Notes["batmobile"];
                Assert.Equal(note, batmobileNote);

                Assert.Equal("I'm batman!\n", batmobileNote.Message);
                Assert.Equal("batmobile", batmobileNote.Namespace);
            }
        }

        /*
         * $ git log 8496071c1b46c854b31185ea97743be6a8774479
         * commit 8496071c1b46c854b31185ea97743be6a8774479
         * Author: Scott Chacon <schacon@gmail.com>
         * Date:   Sat May 8 16:13:06 2010 -0700
         *
         *     testing
         *
         * Notes:
         *     Hi, I'm Note.
         */
        [Fact]
        public void CanRemoveANoteWithTheDefaultNamespaceOnACommit()
        {
            TemporaryCloneOfTestRepo path = BuildTemporaryCloneOfTestRepo();
            using (var repo = new Repository(path.RepositoryPath))
            {
                var commit = repo.Lookup<Commit>("8496071c1b46c854b31185ea97743be6a8774479");

                Assert.NotNull(commit.Notes.Default);

                commit.Notes.Delete(signatureNullToken, signatureYorah);

                Assert.Null(commit.Notes.Default);
            }
        }

        /*
         * $ git show 5b5b025afb0b4c913b4c338a42934a3863bf3644 --notes=answer
         * commit 5b5b025afb0b4c913b4c338a42934a3863bf3644
         * Author: Scott Chacon <schacon@gmail.com>
         * Date:   Tue May 11 13:38:42 2010 -0700
         * 
         *     another commit
         * 
         * Notes (answer):
         *     Not what?
         */
        [Fact]
        public void CanRemoveANoteOnACommitByPassingItsNamespace()
        {
            TemporaryCloneOfTestRepo path = BuildTemporaryCloneOfTestRepo();
            using (var repo = new Repository(path.RepositoryPath))
            {
                var commit = repo.Lookup<Commit>("5b5b025afb0b4c913b4c338a42934a3863bf3644");

                Assert.NotNull(commit.Notes["answer"]);

                commit.Notes.Delete("answer", signatureNullToken, signatureYorah);

                Assert.Null(commit.Notes["answer"]);
            }
        }

        [Fact]
        public void RemovingANonExistingNoteDoesntThrow()
        {
            TemporaryCloneOfTestRepo path = BuildTemporaryCloneOfTestRepo();
            using (var repo = new Repository(path.RepositoryPath))
            {
                var commit = repo.Lookup<Commit>("5b5b025afb0b4c913b4c338a42934a3863bf3644");

                commit.Notes.Delete("answer2", signatureNullToken, signatureYorah);
            }
        }

        [Fact]
        public void CanEditANoteOnACommitByPassingItsNamespace()
        {
            TemporaryCloneOfTestRepo path = BuildTemporaryCloneOfTestRepo();
            using (var repo = new Repository(path.RepositoryPath))
            {
                var commit = repo.Lookup<Commit>("5b5b025afb0b4c913b4c338a42934a3863bf3644");

                var note = commit.Notes.Edit("I'm a new note!", signatureNullToken, signatureYorah, "answer");

                Assert.Equal(note, commit.Notes["answer"]);
            }
        }
    }
}
