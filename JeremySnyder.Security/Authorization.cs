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

namespace JeremySnyder.Security
{
    public sealed class Authorization
    {
        public ClaimsPrincipal Principal { get; }
        public string EmailAddress { get; }
        public string Identifier { get; }
        
        public Authorization(ClaimsPrincipal principal)
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
        /// </summary>
        public void BuildInternalClaims()
        {
            var userDTO = SecurityRepository.FindByExternalID(IntegrationTypes.Firebase, Identifier);

            // If not found by their identifier, it might be their first login attempt
            if (userDTO == null)
            {
                userDTO = SecurityRepository.FindByEmail(EmailAddress);
                userDTO.Identifier = Identifier;
                SecurityRepository.Upsert(userDTO);
            }

            // if it's still null, then they did not exist in HubSpot the last time we pulled
            if (userDTO == null)
            {
                throw new Exception("Cannot identify or create the user in the database");
            }

            var userId = userDTO.ID;

            var identity = new ClaimsIdentity(Principal.Identity.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.GivenName, userDTO.FirstName ?? string.Empty));
            identity.AddClaim(new Claim(ClaimTypes.Surname, userDTO.LastName ?? string.Empty));
            identity.AddClaim(new Claim(ClaimTypes.MobilePhone, userDTO.PhoneNumber ?? string.Empty));
            identity.AddClaim(new Claim(ClaimTypes.HomePhone, userDTO.PhoneNumber ?? string.Empty));
            identity.AddClaim(new Claim("UserID", userId.ToString(), ClaimValueTypes.Integer64));

            var roles = SecurityRepository.GetUserRoles(userId);
            if (roles != null)
            {
                foreach (var role in roles.Where(r => r.UserID == userId && !string.IsNullOrWhiteSpace(r.RoleName)))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role.RoleName));
                }
            }
            
            Principal.AddIdentity(identity);
        }
    }
}
