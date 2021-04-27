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

        public static UserDTO Upsert(UserDTO userDTO)
        {
            var type = (int)IntegrationTypes.Email;
            var active = userDTO.Active ? 1 : 0;
            UserDTO result = null;
            string parameters;

            if (userDTO.ID == 0)
            {
                if (!string.IsNullOrWhiteSpace(userDTO.EmailAddress))
                {
                    result = FindByExternalId(IntegrationTypes.Email, userDTO.EmailAddress);
                }
                else if (!string.IsNullOrWhiteSpace(userDTO.SecurityIdentifier))
                {
                    result = FindByExternalId(SecurityIntegration, userDTO.SecurityIdentifier);
                }

                if (result?.ID > 0)
                {
                    userDTO = result;
                }
            }

            if (!string.IsNullOrWhiteSpace(userDTO.EmailAddress))
            {
                parameters = $"{userDTO.ID}, '{userDTO.FirstName} {userDTO.LastName}', '{userDTO.FirstName}', '{userDTO.LastName}', {active}, {type}, '{userDTO.EmailAddress}'";
                // CALL security.CreateUpdateUser ( 1, 'Test User', 'Jeremy', 'Snyder', 1, 1, 'v2OcfN1HtPVm30JrSpfpnDhN3Tg1' );
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
