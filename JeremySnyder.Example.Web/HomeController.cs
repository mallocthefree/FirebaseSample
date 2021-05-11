/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>May 11, 2021</date>
/////////////////////////////////////////////////////////

using System;
using JeremySnyder.Common;
using JeremySnyder.Common.Web;
using JeremySnyder.Common.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JeremySnyder.Example.Web
{
    [ApiController]
    [Route("api/Home")]
    public class HomeController : JeremySnyderController
    {
        /// <summary>
        /// Used for a test call without security in order to validate a connection to the URL
        /// </summary>
        /// <returns></returns>
        // GET: /api/Home/HelloWorld
        [HttpGet("HelloWorld")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiMessage), StatusCodes.Status200OK)]
        public IActionResult HelloWorld()
        {
            Logging.Debug("HelloWorld initiated");
            return Ok(new ApiMessage { Message = "Hello, World"} );
        }
        
        /// <summary>
        /// Used as a test call with security to ensure that the user is being identified properly
        /// </summary>
        /// <returns></returns>
        // GET: /api/Home/HelloMe
        [HttpGet("HelloMe")]
        [Produces("application/json")]
        [Authorize]
        [ProducesResponseType(typeof(ApiMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status401Unauthorized)]
        public IActionResult HelloMe()
        {
            Logging.Debug("HelloMe initiated");
            var user = GetAuthenticationName();
            return Ok(new ApiMessage { Message = $"Hello, {user}"} );
        }
    }
}
