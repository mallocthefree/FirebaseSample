/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>March 16, 2021</date>
/////////////////////////////////////////////////////////

using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace JeremySnyder.Security.Firebase
{
    /// <summary>
    /// Internal to prevent access from outside the library
    /// Sealed to prevent extending the class
    /// </summary>
    internal sealed class FirebaseAuthentication : IAuthentication
    {
        private static FirebaseConfiguration Configuration => SecurityFactory.Configuration;
        private FirebaseApp Instance { get; set; }

        public FirebaseAuthentication()
        {
            Configure();
        }

        /// <inheritdoc />
        public void Configure()
        {
            if (Instance != null)
            {
                return;
            }

            var options = new AppOptions
            {
                ProjectId = Configuration.ProjectId,
                ServiceAccountId = Configuration.AppId,
                Credential = GoogleCredential.FromFile(Configuration.KeyPath)
            };

            // This is the true establishment of the relationship with Firebase,
            // and critical to this entire authentication process.
            Instance = FirebaseApp.Create(options);
        }
    }
}
