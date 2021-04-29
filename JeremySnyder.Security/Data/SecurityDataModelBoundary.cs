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
    /// <summary>
    /// The purpose of a DataModel Boundary is to create a layer between the user, which can include the system itself,
    /// and the data in the database. It enforces a level of indirection for operational consistency between the
    /// MVC/MVVM parts of the system and the database.
    /// 
    /// It's a place where the developer can implement data transforms between enums and numbers, decimals and integers,
    /// presentation decimal accuracy (rounding), currency presentation (or calculation), and string manipulations.
    /// </summary>
    public static class SecurityDataModelBoundary
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Find the user details for a provided email address
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns>user details <seealso cref="UserModel"/></returns>
        public static UserModel FindByEmail(string emailAddress)
        {
            return FindBySecurityIdentifier(IntegrationTypes.Email, emailAddress);
        }
        
        /// <summary>
        /// Find user details using the defined authentication security integration external identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>user details <seealso cref="UserModel"/></returns>
        public static UserModel FindBySecurityIdentifier(string identifier)
        {
            return FindBySecurityIdentifier(SecurityRepository.SecurityIntegration, identifier);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="integrationType">Integration Type <seealso cref="IntegrationTypes"/></param>
        /// <param name="identifier">String indicating the external id for the given integration type</param>
        /// <returns>user details <seealso cref="UserModel"/></returns>
        private static UserModel FindBySecurityIdentifier(IntegrationTypes integrationType, string identifier)
        {
            var userModel = new UserModel();

            var userDTO = SecurityRepository.FindByExternalId(integrationType, identifier);

            userDTO?.ToModel(ref userModel);

            userModel.Roles = GetUserRoles(userModel.ID).ToList();
            
            return userModel;
        }

        /// <summary>
        /// Add a user
        /// </summary>
        /// <param name="userModel">user details <seealso cref="UserModel"/></param>
        /// <returns>user details <seealso cref="UserModel"/></returns>
        public static UserModel AddUser(UserModel userModel)
        {
            var userDTO = new UserDTO();
            userModel.ToDTO(ref userDTO);
            userDTO.Active = true;
            var resultDTO = SecurityRepository.Upsert(userDTO);
            var resultModel = new UserModel();
            resultDTO.ToModel(ref resultModel);

            return resultModel;
        }

        /// <summary>
        /// Update all roles that should be available to the user.
        /// NOTE: Anything in the system that is not in this list will be disabled.
        /// </summary>
        /// <param name="userId">User ID ( security.tblUsers.ID )</param>
        /// <param name="activeRoles">Complete list of all roles that should be available to the user</param>
        /// <returns>Final list of active roles for the user</returns>
        public static IEnumerable<UserRoleModel> UpdateUserRoles(long userId, IEnumerable<SecurityRoles> activeRoles)
        {
            var currentRoles = SecurityRepository.GetUserRoles(userId);

            // "Distinct" will remove duplicates
            var activeRolesList = activeRoles.Distinct().ToList();

            // Add any roles that are not yet listed
            var currentUserRoleList = currentRoles.ToList();
            foreach (var currentRole in currentUserRoleList)
            {
                var roleId = (SecurityRoles)currentRole.RoleID;
                if (!activeRolesList.Contains(roleId))
                {
                    AddUserRole(userId, roleId);
                }
            }
            
            // Remove any roles not in the new active list
            foreach (var role in activeRolesList)
            {
                if (currentUserRoleList.All(r => r.RoleID != (long)role))
                {
                    RemoveUserRole(userId, role);
                }
            }
            
            return GetUserRoles(userId);
        }

        /// <summary>
        /// Add a user role
        /// </summary>
        /// <param name="userId">user ID</param>
        /// <param name="role">role <seealso cref="SecurityRoles"/></param>
        public static void AddUserRole(long userId, SecurityRoles role)
        {
            // Using an enumeration as a parameter restricts to valid role values within the database.
            // However, this does require updating the enum when the table is updated.
            SecurityRepository.UpdateUserRole(userId, (long)role, true);
        }
        
        /// <summary>
        /// Remove a user role
        /// </summary>
        /// <param name="userId">user ID</param>
        /// <param name="role">role <seealso cref="SecurityRoles"/></param>
        public static void RemoveUserRole(long userId, SecurityRoles role)
        {
            // Using an enumeration as a parameter restricts to valid role values within the database.
            // However, this does require updating the enum when the table is updated.
            SecurityRepository.UpdateUserRole(userId, (long)role, false);
        }
    }
}
