using LibGit2Sharp.Core;
using LibGit2Sharp.Core.Handles;

namespace LibGit2Sharp
{
    public class Note : GitObject
    {
        private Note(ObjectId id, string @namespace, string message) : base(id)
        {
            Namespace = @namespace;
            Message = message;
        }

        public string Namespace { get; private set; }
        public string Message { get; private set; }

        internal static Note BuildFromPtr(string @namespace, NoteSafeHandle note)
        {
            ObjectId oid = NativeMethods.git_note_oid(note).MarshalAsObjectId();
            var message = NativeMethods.git_note_message(note);

            return new Note(oid, @namespace, message);
        }
    }
}
