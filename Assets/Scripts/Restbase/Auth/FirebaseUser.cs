using System;

namespace Restbase.Firebase.Auth
{
    public class FirebaseUser
    {
        private FirebaseAuth authProxy;

        public string DisplayName;

        public string Email;

        public bool IsAnonymous;

        public bool IsEmailVerified;

        //public UserMetadata Metadata => GetValidFirebaseUserInternal().Metadata;

        public string PhoneNumber;

        public Uri PhotoUrl;

        //public IEnumerable<IUserInfo> ProviderData => GetValidFirebaseUserInternal().ProviderData;

        public string ProviderId;

        public string UserId;

        internal FirebaseUser(FirebaseAuth auth)
        {
            authProxy = auth;
        }
    }
}
