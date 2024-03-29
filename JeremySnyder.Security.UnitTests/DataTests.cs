﻿/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>April 22, 2021</date>
/////////////////////////////////////////////////////////

using System.Linq;
using NUnit.Framework;
using JeremySnyder.Security.Data;
using JeremySnyder.Security.Data.Enums;
using JeremySnyder.Security.Data.Models;
using NUnit.Framework.Legacy;

namespace JeremySnyder.Security.UnitTests
{
    [TestFixture]
    public class DataTests
    {
        /// <summary>
        /// Added regularly used test strings here so that they can be changed easily to match a new use case
        /// </summary>

        /* This user must be set up in Firebase */
        private const string UnitTestEmailReal = "jeremysnyder.consulting@gmail.com";
        private const string UnitTestFirstNameReal = "Jeremy";
        private const string UnitTestLastNameReal = "Snyder";
        private const string UnitTestKeyReal = "v2OcfN1HtPVm30JrSpfpnDhN3Tg1";
        
        /* No need to change this one. It's intended for using as a test for "user doesn't exist" scenarios */
        private const string UnitTestEmailBogus = "UnitTest.NotReal@gmail.com";

        [Test]
        [Category("Integration Test")]
        [Category("Database")]
        [TestCase(1)]
        public void Test_GetUserRoles_ShouldHaveAtLeastOne(long userId)
        {
            var userRoles = SecurityDataModelBoundary.GetUserRoles(userId);
            
            ClassicAssert.NotNull(userRoles);
            ClassicAssert.IsNotEmpty(userRoles);
        }
        
        [Test]
        [Category("Integration Test")]
        [Category("Database")]
        [TestCase(1)]
        public void Test_GetUserByID_ShouldExistWithAtLeastOneRole(long userId)
        {
            var user = SecurityDataModelBoundary.FindById(userId);
            
            ClassicAssert.NotNull(user);
            ClassicAssert.NotNull(user.Roles);
            ClassicAssert.IsNotEmpty(user.Roles);
        }
        
        [Test]
        [Category("Integration Test")]
        [Category("Database")]
        [TestCase(0, true, UnitTestEmailBogus, null, "Unit", "Test")]
        [TestCase(1, false, "jeremysnyder.Changed@gmail.com", UnitTestKeyReal, UnitTestFirstNameReal, UnitTestLastNameReal)]
        [TestCase(1, true, UnitTestEmailReal, UnitTestKeyReal, UnitTestFirstNameReal, UnitTestLastNameReal)]
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
            
            ClassicAssert.NotNull(user);
            if (id > 0)
            {
                ClassicAssert.AreEqual(id, user.ID);
            }
            
            ClassicAssert.AreEqual(emailAddress, user.EmailAddress);
            ClassicAssert.AreEqual(identifier, user.SecurityIdentifier);
            ClassicAssert.AreEqual(firstName, user.FirstName);
            ClassicAssert.AreEqual(lastName, user.LastName);
        }

        [Test]
        [Category("Integration Test")]
        [Category("Database")]
        [TestCase(1, UnitTestEmailReal, UnitTestKeyReal, UnitTestFirstNameReal, UnitTestLastNameReal)]
        public void Test_GetUserBySecurityIdentifier_ShouldExist(
            long id,
            string emailAddress,
            string identifier,
            string firstName,
            string lastName)
        {
            var user = SecurityDataModelBoundary.FindBySecurityIdentifier(identifier);
            
            ClassicAssert.NotNull(user);
            ClassicAssert.AreEqual(id, user.ID);
            ClassicAssert.AreEqual(emailAddress, user.EmailAddress);
            ClassicAssert.AreEqual(identifier, user.SecurityIdentifier);
            ClassicAssert.AreEqual(firstName, user.FirstName);
            ClassicAssert.AreEqual(lastName, user.LastName);
            ClassicAssert.NotNull(user.Roles);
            ClassicAssert.IsNotEmpty(user.Roles);
        }
        
        [Test]
        [Category("Integration Test")]
        [Category("Database")]
        [TestCase(1, UnitTestEmailReal, UnitTestKeyReal, UnitTestFirstNameReal, UnitTestLastNameReal)]
        public void Test_GetUserByEmail_ShouldExist(
            long id,
            string emailAddress,
            string securityIdentifier,
            string firstName,
            string lastName)
        {
            var user = SecurityDataModelBoundary.FindByEmail(emailAddress);
            
            ClassicAssert.NotNull(user);
            ClassicAssert.AreEqual(id, user.ID);
            ClassicAssert.AreEqual(emailAddress, user.EmailAddress);
            ClassicAssert.AreEqual(securityIdentifier, user.SecurityIdentifier);
            ClassicAssert.AreEqual(firstName, user.FirstName);
            ClassicAssert.AreEqual(lastName, user.LastName);
            ClassicAssert.NotNull(user.Roles);
            ClassicAssert.IsNotEmpty(user.Roles);
        }
        
        [Test]
        [Category("Integration Test")]
        [Category("Database")]
        [TestCase(1, UnitTestEmailBogus)]
        [TestCase(2, UnitTestEmailBogus )]
        public void Test_AddUserRoles_ShouldAdd(long roleId, string emailAddress)
        {
            var userModel = SecurityDataModelBoundary.FindByEmail(emailAddress);

            if (userModel?.ID == 0)
            {
                Assert.Inconclusive("Invalid test email");
            }
            
            var alreadyHas = userModel.Roles.Any(r => r.RoleID == roleId);

            if (alreadyHas)
            {
                // It's worth noting that because we made it here, the test WILL update the updated timestamp
                SecurityDataModelBoundary.RemoveUserRole(userModel.ID, (SecurityRoles)roleId);
            }
            
            var roles = SecurityDataModelBoundary.GetUserRoles(userModel.ID);
            alreadyHas = roles.Any(r => r.RoleID == roleId);

            if (alreadyHas)
            {
                Assert.Inconclusive("Cannot test because the role already exists and removal has failed");
            }

            SecurityDataModelBoundary.AddUserRole(userModel.ID, (SecurityRoles)roleId);
            roles = SecurityDataModelBoundary.GetUserRoles(userModel.ID);
            
            ClassicAssert.NotNull(roles);
            ClassicAssert.AreEqual(emailAddress, userModel.EmailAddress);
            ClassicAssert.IsTrue(roles.Any(r => r.RoleID == roleId));
        }
        
        [Test]
        [Category("Integration Test")]
        [Category("Database")]
        [TestCase(1, UnitTestEmailBogus )]
        public void Test_RemoveUserRole_ShouldRemove(long roleId, string emailAddress)
        {
            var userModel = SecurityDataModelBoundary.FindByEmail(emailAddress);

            if (userModel?.ID == 0)
            {
                Assert.Inconclusive("Invalid test email");
            }
            
            var alreadyHas = userModel.Roles.Any(r => r.RoleID == roleId);

            if (!alreadyHas)
            {
                // It's worth noting that because we made it here, the test WILL update the updated timestamp
                SecurityDataModelBoundary.AddUserRole(userModel.ID, (SecurityRoles)roleId);
            }
            
            var roles = SecurityDataModelBoundary.GetUserRoles(userModel.ID);
            alreadyHas = roles.Any(r => r.RoleID == roleId);

            if (!alreadyHas)
            {
                Assert.Inconclusive("Cannot test because the role didn't exist already and adding failed");
            }

            SecurityDataModelBoundary.RemoveUserRole(userModel.ID, (SecurityRoles)roleId);
            roles = SecurityDataModelBoundary.GetUserRoles(userModel.ID);
            
            ClassicAssert.NotNull(roles);
            ClassicAssert.AreEqual(emailAddress, userModel.EmailAddress);
            ClassicAssert.IsTrue(roles.All(r => r.RoleID != roleId));
        }
    }
}
