namespace Restbase.Firebase.Storage
{
    public class FirebaseStorage
    {
        private static FirebaseStorage _instance;
        public static FirebaseStorage DefaultInstance
        {
            get
            {
                if (_instance == null)
                {
                    UnityMainThreadDispatcher.Instance();
                    _instance = new FirebaseStorage();
                }
                return _instance;
            }
        }

        public StorageReference GetReference()
        {
            return new StorageReference(string.Empty);
        }
    }
}
