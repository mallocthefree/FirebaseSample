/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>April 9, 2021</date>
/////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Linq;
using JeremySnyder.Security.Data.DTO;
using JeremySnyder.Security.Data.Enums;
using JeremySnyder.Shared.Data;

namespace JeremySnyder.Security.Data
{
    internal static class SecurityRepository
    {
        private const string Schema = "security";
        public const IntegrationTypes SecurityIntegration = IntegrationTypes.Firebase;
        
        public static IEnumerable<UserRoleDTO> GetUserRoles(long userId)
        {
            return RepositoryBase.Query<UserRoleDTO>
                (string.Empty, Schema, "GetUserRoles", userId.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public static void UpdateUserRole(long userId, long roleId, bool active)
        {
            // Note that in a strict security environment, this should also log a record for the change.
            // However, this can and probably should be done within the database procedure, itself.
            var parameters = $"{userId}, {roleId}, {(active ? 1 : 0).ToString()}";
            RepositoryBase.Query<UserRoleDTO>
                (string.Empty, Schema, "AddUpdateUserRole", parameters);
        }

        /// <summary>
        /// Insert or update a user object and it's integration parts into the database
        /// </summary>
        /// <param name="userDTO">User Data Transfer Object</param>
        /// <returns>The committed User DTO</returns>
        public static UserDTO Upsert(UserDTO userDTO)
        {
            var type = (int)IntegrationTypes.Email;
            var active = userDTO.Active ? 1 : 0;
            UserDTO result;
            string parameters;

            // If the ID is 0, we should see if it already exists before creating a new user
            // This will also prevent collisions and conflicts
            if (userDTO.ID == 0)
            {
                // If the result is 0, then there is no change anyway
                userDTO.ID = FindUserID(userDTO);
            }

            // Save the integration email, if its legitimate
            // TODO: Validate the form of the email address provided.
            if (!string.IsNullOrWhiteSpace(userDTO.EmailAddress))
            {
                parameters = $"{userDTO.ID}, '{userDTO.FirstName} {userDTO.LastName}', '{userDTO.FirstName}', '{userDTO.LastName}', {active}, {type}, '{userDTO.EmailAddress}'";
                result = RepositoryBase.Query<UserDTO>
                        (string.Empty, Schema, "CreateUpdateUser", parameters)
                    .FirstOrDefault() ?? userDTO;

                if (result.ID > 0)
                {
                    userDTO = result;
                }
            }

            if (string.IsNullOrWhiteSpace(userDTO.SecurityIdentifier))
            {
                return userDTO;
            }
            
            // Save the authentication integration key details
            type = (int) SecurityIntegration;
            parameters = $"{userDTO.ID}, '{userDTO.FirstName} {userDTO.LastName}', '{userDTO.FirstName}', '{userDTO.LastName}', {active}, {type}, '{userDTO.SecurityIdentifier}'";
            result = RepositoryBase.Query<UserDTO>
                    (string.Empty, Schema, "CreateUpdateUser", parameters)
                .FirstOrDefault() ?? userDTO;

            if (result.ID > 0)
            {
                userDTO = result;
            }

            return userDTO;
        }

        /// <summary>
        /// Process business rules for order of preference in locating a user
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns>ID of the user from security.tblUsers</returns>
        public static long FindUserID(UserDTO userDTO)
        {
            UserDTO result = null;
            
            // Prioritize the security identifier (ExternalID) from our authentication provider
            if (!string.IsNullOrWhiteSpace(userDTO.SecurityIdentifier))
            {
                result = FindByExternalId(SecurityIntegration, userDTO.SecurityIdentifier);
            }
            // Email is a secondary / last resort check
            if (result?.ID == 0 && !string.IsNullOrWhiteSpace(userDTO.EmailAddress))
            {
                result = FindByExternalId(IntegrationTypes.Email, userDTO.EmailAddress);
            }

            return result?.ID ?? 0;
        }

        /// <summary>
        /// Find a User DTO using a specific integration key
        /// </summary>
        /// <param name="integrationType">Integration key type (enum) <seealso cref="IntegrationTypes"/></param>
        /// <param name="identifier">The ExternalID in the security.tblUserIntegrations table</param>
        /// <returns>Located User DTO if found. null if not found.</returns>
        public static UserDTO FindByExternalId(IntegrationTypes integrationType, string identifier)
        {
            var type = (int)integrationType;
            var parameters = $"{type.ToString()}, '{identifier}'";
            var userId = RepositoryBase.Query<long>
                (string.Empty, Schema, "GetUserIDByExternalID", parameters)
                .FirstOrDefault();

            if (userId > 0)
            {
                return RepositoryBase.Query<UserDTO>
                        (string.Empty, Schema, "GetUserByID", userId.ToString())
                    .FirstOrDefault();
            }

            return null;
        }
    }
}
