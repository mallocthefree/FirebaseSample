/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>April 9, 2021</date>
/////////////////////////////////////////////////////////

using System.Collections.Generic;
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
            return RepositoryBase.QueryWithWhere<UserRoleDTO>
                (string.Empty, Schema, "GetUserRoles", userId.ToString());
        }

        public static UserDTO FindByEmail(string emailAddress)
        {
            throw new System.NotImplementedException();
        }

        public static void Upsert(UserDTO userDTO)
        {
            throw new System.NotImplementedException();
        }

        public static UserDTO FindByExternalId(IntegrationTypes firebase, string identifier)
        {
            throw new System.NotImplementedException();
        }
    }
}
