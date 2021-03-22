/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>March 16, 2021</date>
/////////////////////////////////////////////////////////

namespace JeremySnyder.Security
{
    public interface IAuthentication
    {
        /// <summary>
        /// Build configurations from settings loaded in the <see cref="SecurityFactory"/>
        /// </summary>
        void Configure();
    }
}
