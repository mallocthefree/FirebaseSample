/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>April 22, 2021</date>
/////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Linq;
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
        
        public static UserModel FindByExternalId(IntegrationTypes integrationType, string identifier)
        {
            var userModel = new UserModel();

            var userDTO = SecurityRepository.FindByExternalId(integrationType, identifier);

            userDTO?.ToModel(ref userModel);

            userModel.Identifier = identifier;
            userModel.Roles = GetUserRoles(userModel.ID).ToList();
            
            return userModel;
        }
    }
}
