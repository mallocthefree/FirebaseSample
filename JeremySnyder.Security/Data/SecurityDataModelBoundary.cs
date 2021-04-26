/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>April 22, 2021</date>
/////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Linq;
using JeremySnyder.Security.Data.DTO;
using JeremySnyder.Security.Data.Enums;
using JeremySnyder.Security.Data.Models;
using JeremySnyder.Shared.Data.Base;

namespace JeremySnyder.Security.Data
{
    public static class SecurityDataModelBoundary
    {
        public static IEnumerable<UserRoleModel> GetUserRoles(long userId)
        {
            IList<UserRoleModel> userRoleModels = new List<UserRoleModel>();

            var userRoleDTOs = SecurityRepository.GetUserRoles(userId);

            foreach (var dto in userRoleDTOs)
            {
                var model = new UserRoleModel();
                dto.ToModel(ref model);
                userRoleModels.Add(model);
            }

            return userRoleModels;
        }
        
        public static UserModel FindByEmail(string emailAddress)
        {
            return FindBySecurityIdentifier(IntegrationTypes.Email, emailAddress);
        }
        
        public static UserModel FindBySecurityIdentifier(string identifier)
        {
            return FindBySecurityIdentifier(IntegrationTypes.Firebase, identifier);
        }
        
        private static UserModel FindBySecurityIdentifier(IntegrationTypes integrationType, string identifier)
        {
            var userModel = new UserModel();

            var userDTO = SecurityRepository.FindByExternalId(integrationType, identifier);

            userDTO?.ToModel(ref userModel);

            userModel.Identifier = identifier;
            userModel.Roles = GetUserRoles(userModel.ID).ToList();
            
            return userModel;
        }

        public static void AddUser(UserModel userModel)
        {
            var userDTO = new UserDTO();
            userModel.ToDTO(ref userDTO);
            SecurityRepository.Upsert(userDTO);
        }
    }
}
