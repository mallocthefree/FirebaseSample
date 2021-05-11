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

        protected UserModel GetUser(long userID = 0)
        {
            if (userID == 0)
            {
                CheckRefresh();
                userID = Authorization.GetUserId();
            }
            
            return SecurityDataModelBoundary.FindById(userID);
        }

        protected string GetAuthenticationName()
        {
            CheckRefresh();
            
            var email = Authorization.EmailAddress;
                
            return string.IsNullOrWhiteSpace(email) ? "World" : email;
        }

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
