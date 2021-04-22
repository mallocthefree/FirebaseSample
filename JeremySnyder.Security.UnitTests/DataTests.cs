/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>April 22, 2021</date>
/////////////////////////////////////////////////////////

using NUnit.Framework;
using JeremySnyder.Security.Data;

namespace JeremySnyder.Security.UnitTests
{
    [TestFixture]
    public class DataTests
    {
        [Test]
        [Category("Integration Test")]
        [Category("Database")]
        [TestCase(1)]
        public void Test_GetUserRoles_ShouldHaveAtLeastOne(long userId)
        {
            var userRoles = SecurityDataModelBoundary.GetUserRoles(userId);
            
            Assert.NotNull(userRoles);
            Assert.IsNotEmpty(userRoles);
        }
    }
}
