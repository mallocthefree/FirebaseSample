/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>March 16, 2021</date>
/////////////////////////////////////////////////////////

using JeremySnyder.Security.Firebase;
using JeremySnyder.Common;

namespace JeremySnyder.Security
{
    public static class SecurityFactory
    {
        private static FirebaseConfiguration _configuration;
        
        public static FirebaseConfiguration Configuration =>
            _configuration ??= ConfigurationLoader.GetSection<FirebaseConfiguration>("firebase");

        private static IAuthentication _authentication;
        public static IAuthentication Authentication =>
            _authentication ??= new FirebaseAuthentication();
    }
}
