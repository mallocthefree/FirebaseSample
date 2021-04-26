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
        
        public static IEnumerable<UserRoleDTO> GetUserRoles(long userId)
        {
            return RepositoryBase.Query<UserRoleDTO>
                (string.Empty, Schema, "GetUserRoles", userId.ToString());
        }

        public static void Upsert(UserDTO userDTO)
        {
            throw new System.NotImplementedException();
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
