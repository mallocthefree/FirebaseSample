/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>April 9, 2021</date>
/////////////////////////////////////////////////////////

using JeremySnyder.Shared.Data.Base.DTO;

namespace JeremySnyder.Security.Data.DTO
{
    internal class UserDTO : BaseDTOWithID
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string SecurityIdentifier { get; set; }
        public bool Active { get; set; }
    }
}
