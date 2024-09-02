
namespace Restbase.Firebase.Firestore
{
    public class FirebaseFirestore
    {
        private static FirebaseFirestore _instance;
        public static FirebaseFirestore DefaultInstance
        {
            get
            {
                if (_instance == null)
                {
                    UnityMainThreadDispatcher.Instance();
                    _instance = new FirebaseFirestore();
                }
                return _instance;
            }
        }

        public DocumentReference Document(string path)
        {
            return new DocumentReference(path);
        }

        public CollectionReference Collection(string path)
        {
            return new CollectionReference(path);
        }
    }
}
