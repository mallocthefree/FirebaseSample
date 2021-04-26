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
        
        [Test]
        [Category("Integration Test")]
        [Category("Database")]
        [TestCase("v2OcfN1HtPVm30JrSpfpnDhN3Tg1", "Jeremy", "Snyder")]
        public void Test_GetUserBySecurityIdentifier_ShouldExist(
            string identifier,
            string firstName,
            string lastName)
        {
            var user = SecurityDataModelBoundary.FindBySecurityIdentifier(identifier);
            
            Assert.NotNull(user);
            Assert.AreEqual(1, user.ID);
            Assert.AreEqual(identifier, user.Identifier);
            Assert.AreEqual(firstName, user.FirstName);
            Assert.AreEqual(lastName, user.LastName);
            Assert.NotNull(user.Roles);
            Assert.IsNotEmpty(user.Roles);
        }
        
        [Test]
        [Category("Integration Test")]
        [Category("Database")]
        [TestCase("jeremysnyder.consulting@gmail.com", "Jeremy", "Snyder")]
        public void Test_GetUserByEmail_ShouldExist(
            string emailAddress,
            string firstName,
            string lastName)
        {
            var user = SecurityDataModelBoundary.FindByEmail(emailAddress);
            
            Assert.NotNull(user);
            Assert.AreEqual(1, user.ID);
            Assert.AreEqual(emailAddress, user.Identifier);
            Assert.AreEqual(firstName, user.FirstName);
            Assert.AreEqual(lastName, user.LastName);
            Assert.NotNull(user.Roles);
            Assert.IsNotEmpty(user.Roles);
        }
    }
}
