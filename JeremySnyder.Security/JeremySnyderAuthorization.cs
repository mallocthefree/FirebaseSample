/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>March 16, 2021</date>
/////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Security.Claims;
using JeremySnyder.Security.Data;

namespace JeremySnyder.Security
{
    public sealed class JeremySnyderAuthorization
    {
        // If true, then  create users not found in the database.
        // If false, reject the user if not found.
        private const bool AutoCreateUsers = false;
        
        public ClaimsPrincipal Principal { get; }
        public string EmailAddress { get; }
        public string Identifier { get; }
        
        // For now, just using the user's email address, but you should replace this value
        // with something stored without a specific identifying value.
        public string DisplayName => EmailAddress;
        
        public JeremySnyderAuthorization(ClaimsPrincipal principal)
        {
            EmailAddress = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty;
            Identifier = principal.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value ?? string.Empty;
            Principal = principal;
        }

        public long GetUserId()
        {
            var userIdString = Principal.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value ?? string.Empty;

            if (string.IsNullOrWhiteSpace(userIdString))
            {
                return -1;
            }

            return long.TryParse(userIdString, out var result) ? result : -1;
        }

        /// <summary>
        /// Build claims established internal to our system
        /// This is where we take the values discovered in the authentication process and
        /// create the authorization contracts for the individual user
        /// </summary>
        public void BuildInternalClaims()
        {
            var userModel = SecurityDataModelBoundary.FindBySecurityIdentifier(Identifier);

            // If not found by their identifier, it might be their first login attempt
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (userModel == null && AutoCreateUsers)
            {
                userModel = SecurityDataModelBoundary.FindByEmail(EmailAddress);
                userModel.SecurityIdentifier = Identifier;

                SecurityDataModelBoundary.AddUser(userModel);
            }

            // if it's still null, then they did not exist in HubSpot the last time we pulled
            if (userModel == null)
            {
                throw new Exception("Cannot identify or create the user in the database");
            }

            var userId = userModel.ID;

            var identity = new ClaimsIdentity(Principal.Identity.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.GivenName, userModel.FirstName ?? string.Empty));
            identity.AddClaim(new Claim(ClaimTypes.Surname, userModel.LastName ?? string.Empty));
            identity.AddClaim(new Claim("UserID", userId.ToString(), ClaimValueTypes.Integer64));

            var roles = SecurityRepository.GetUserRoles(userId);
            if (roles != null)
            {
                // for every role found in the database, we will add a unique role claim.
                foreach (var role in roles.Where(r => r.UserID == userId && !string.IsNullOrWhiteSpace(r.RoleName)))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role.RoleName));
                }
            }
            
            Principal.AddIdentity(identity);
        }
    }
}
