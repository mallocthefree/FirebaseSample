/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>May 11, 2021</date>
/////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using JeremySnyder.Security;
using JeremySnyder.Security.Data;
using JeremySnyder.Security.Data.Models;
using JeremySnyder.Shared.Data.Base;
using JeremySnyder.Shared.Data.Base.DTO;
using JeremySnyder.Shared.Data.Base.Models;

namespace JeremySnyder.Common.Web
{
    public abstract class JeremySnyderController : ControllerBase
    {
        #region User Info

        // I'll cache a dictionary later
        private ClaimsPrincipal _principal;

        private ClaimsPrincipal Principal
        {
            get => _principal;
            set
            {
                Authorization = new JeremySnyderAuthorization(value);
                Authorization.BuildInternalClaims();
                _principal = Authorization.Principal;
            }
        }

        private JeremySnyderAuthorization Authorization { get; set; }

        /// <summary>
        /// Use this within the application in future calls to get the user information after the
        /// authentication process has provided what was needed to get the user ID
        /// </summary>
        /// <param name="userID">The user's ID within the local system</param>
        /// <returns><see cref="UserModel"/> of the user's data stored in the internal system</returns>
        protected UserModel GetUser(long userID = 0)
        {
            if (userID == 0)
            {
                CheckRefresh();
                userID = Authorization.GetUserId();
            }
            
            return SecurityDataModelBoundary.FindById(userID);
        }

        /// <summary>
        /// Retrieves the user's display name
        /// </summary>
        /// <returns></returns>
        protected string GetAuthenticationName()
        {
            // Collect the authentication details if we don't yet have it
            CheckRefresh();
            
            var displayName = Authorization.DisplayName;
                
            return string.IsNullOrWhiteSpace(displayName) ? "World" : displayName;
        }

        /// <summary>
        /// If we don't currently have the user's information, we should pull the authentication information
        /// passed in from the API endpoint to retrieve the information from Firebase, if it exists
        /// Using this information, collect the necessary information from the local database on our system
        /// user, including their access.
        /// </summary>
        private void CheckRefresh()
        {
            var fetch = true;
            
            if (Authorization != null)
            {
                var savedUserId = Authorization.Identifier;
                var auth = new JeremySnyderAuthorization(User);
                var incomingUserId = auth.Identifier;
                fetch = string.IsNullOrWhiteSpace(savedUserId) &&
                        savedUserId != incomingUserId;
            }

            if (fetch)
            {
                Principal = User;
            }
        }
        #endregion
        
        #region Converters

        protected static IEnumerable<BaseLookupModel> ConvertList(IEnumerable<BaseLookupDTO> list)
        {
            var newList = new List<BaseLookupModel>();
            foreach (var dto in list)
            {
                var model = new BaseLookupModel();
                dto.ToModel(ref model);
                newList.Add(model);
            }

            return newList;
        }
        #endregion
        
        #region Responses

        protected IActionResult PreconditionFailed(object obj)
        {
            return StatusCode(412, obj);
        }
        #endregion
    }
}
