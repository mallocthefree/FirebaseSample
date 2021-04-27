/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>April 22, 2021</date>
/////////////////////////////////////////////////////////

using NUnit.Framework;
using JeremySnyder.Security.Data;
using JeremySnyder.Security.Data.Models;

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
        [TestCase(0, true, "UnitTest.NotReal@gmail.com", null, "Unit", "Test")]
        [TestCase(1, false, "jeremysnyder.Changed@gmail.com", "v2OcfN1HtPVm30JrSpfpnDhN3Tg1", "Jeremy", "Snyder")]
        [TestCase(1, true, "jeremysnyder.consulting@gmail.com", "v2OcfN1HtPVm30JrSpfpnDhN3Tg1", "Jeremy", "Snyder")]
        public void Test_UpsertUser_ShouldCreate(
            long id,
            bool active,
            string emailAddress,
            string identifier,
            string firstName,
            string lastName)
        {
            var userToUpsert = new UserModel
            {
                ID = id,
                EmailAddress = emailAddress,
                FirstName = firstName,
                LastName = lastName,
                SecurityIdentifier = identifier
            };

            var user = SecurityDataModelBoundary.AddUser(userToUpsert);
            
            Assert.NotNull(user);
            if (id > 0)
            {
                Assert.AreEqual(id, user.ID);
            }
            
            Assert.AreEqual(emailAddress, user.EmailAddress);
            Assert.AreEqual(identifier, user.SecurityIdentifier);
            Assert.AreEqual(firstName, user.FirstName);
            Assert.AreEqual(lastName, user.LastName);
        }

        [Test]
        [Category("Integration Test")]
        [Category("Database")]
        [TestCase(1, "jeremysnyder.consulting@gmail.com", "v2OcfN1HtPVm30JrSpfpnDhN3Tg1", "Jeremy", "Snyder")]
        public void Test_GetUserBySecurityIdentifier_ShouldExist(
            long id,
            string emailAddress,
            string identifier,
            string firstName,
            string lastName)
        {
            var user = SecurityDataModelBoundary.FindBySecurityIdentifier(identifier);
            
            Assert.NotNull(user);
            Assert.AreEqual(id, user.ID);
            Assert.AreEqual(emailAddress, user.EmailAddress);
            Assert.AreEqual(identifier, user.SecurityIdentifier);
            Assert.AreEqual(firstName, user.FirstName);
            Assert.AreEqual(lastName, user.LastName);
            Assert.NotNull(user.Roles);
            Assert.IsNotEmpty(user.Roles);
        }
        
        [Test]
        [Category("Integration Test")]
        [Category("Database")]
        [TestCase(1, "jeremysnyder.consulting@gmail.com", "v2OcfN1HtPVm30JrSpfpnDhN3Tg1", "Jeremy", "Snyder")]
        public void Test_GetUserByEmail_ShouldExist(
            long id,
            string emailAddress,
            string securityIdentifier,
            string firstName,
            string lastName)
        {
            var user = SecurityDataModelBoundary.FindByEmail(emailAddress);
            
            Assert.NotNull(user);
            Assert.AreEqual(id, user.ID);
            Assert.AreEqual(emailAddress, user.EmailAddress);
            Assert.AreEqual(securityIdentifier, user.SecurityIdentifier);
            Assert.AreEqual(firstName, user.FirstName);
            Assert.AreEqual(lastName, user.LastName);
            Assert.NotNull(user.Roles);
            Assert.IsNotEmpty(user.Roles);
        }
    }
}
