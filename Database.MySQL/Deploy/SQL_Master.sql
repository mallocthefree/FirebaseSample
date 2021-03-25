
DELIMITER //

CREATE SCHEMA IF NOT EXISTS security;
/*
 Reserved for the start of the master script.
 Can be DB changes or queries to be implemented that have to be done before all
 other object creation scripts
 */


DELIMITER //

SELECT 'Starting 1.Pre_keep.sql';

CREATE TABLE IF NOT EXISTS security.tblDeployment
(
    ID BIGINT PRIMARY KEY AUTO_INCREMENT UNIQUE ,
    Version VARCHAR(100) NOT NULL,
    Type VARCHAR(100) NOT NULL DEFAULT ('Start'),
    DateTimeDeployedUTC DATETIME NOT NULL DEFAULT (UTC_TIMESTAMP())
)
COMMENT = 'Keeps track of SQL deployments'
;

DROP PROCEDURE IF EXISTS security.DeclareDeployment;

CREATE PROCEDURE security.DeclareDeployment(state CHAR(10))
BEGIN

    DECLARE count INT DEFAULT (0);
    DECLARE versionStr VARCHAR(100);
    DECLARE now DATETIME DEFAULT (UTC_TIMESTAMP);

    SELECT ((COUNT(*) / 2) + 1)
    INTO count
    FROM security.tblDeployment
    WHERE DateTimeDeployedUTC > CAST(DATE_FORMAT(now, '%Y.%m.01') AS DATE);

    SELECT count;

    SELECT CONCAT
        (
            CAST(DATE_FORMAT(now, '%Y.%m.') AS CHAR(10)),
            LPAD(count, 3, '0')
        )
    INTO versionStr;

    INSERT INTO security.tblDeployment
        (Version, Type)
    VALUES
        (versionStr, state);
END;

CALL security.DeclareDeployment('Start');

/**************************************************/
SELECT 'Ending 1.Pre_keep.sql';
/*
 Reserved for the start of the master script.
 Can be DB changes or queries to be implemented that have to be done before all
 other object creation scripts
 */


SELECT 'Starting 2.Pre_WipeEveryRelease.sql';


/**************************************************/
/* Remove these lines after initial release       */
/**************************************************/


/**************************************************/
/* End of "wipe code" section                     */
/**************************************************/

/**************************************************/

SELECT 'Ending 2.Pre_WipeEveryRelease.sql';

CREATE TABLE IF NOT EXISTS security.lookup_Integrations
(
    ID INT PRIMARY KEY UNIQUE ,
    IntegrationName VARCHAR(50) NOT NULL,
    Active INT NOT NULL DEFAULT(1),
    DateTimeCreatedUTC DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP(0),
    DateTimeLastUpdatedUTC DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP(0) ON UPDATE CURRENT_TIMESTAMP(0)
)
COMMENT = 'This entity contains all the integrations that the system will use to tie a user to an external system.'
;

CREATE TABLE IF NOT EXISTS security.lookup_Roles
(
    ID INT PRIMARY KEY UNIQUE ,
    RoleName VARCHAR(50) NOT NULL,
    Active INT NOT NULL DEFAULT(1),
    DateTimeCreatedUTC DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP(0),
    DateTimeLastUpdatedUTC DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP(0) ON UPDATE CURRENT_TIMESTAMP(0)
)
COMMENT = 'This entity contains all the roles that the system will need, e.g. User and Admin'
;


SELECT 'Create table script for security.tblUserIntegrations';
CREATE TABLE IF NOT EXISTS security.tblUserIntegrations
(
    UserID            BIGINT       NOT NULL,
    IntegrationID     INT          NOT NULL,
    ExternalID        VARCHAR(200) NOT NULL,
    PRIMARY KEY (UserID, IntegrationID),
    UNIQUE (IntegrationID, ExternalID)
)
COMMENT = 'This table to to contain integration IDs from external sites in conjunction with our internal users.
           Examples of integrations may include: Firebase, HubSpot, SalesForce, or any other external system.'
;

CREATE TABLE IF NOT EXISTS security.tblUserRoles
(
    UserID BIGINT NOT NULL,
    RoleID INT NOT NULL,
    Active INT DEFAULT(1),
    DateTimeCreatedUTC DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP(0),
    DateTimeLastUpdatedUTC DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP(0) ON UPDATE CURRENT_TIMESTAMP(0),
    PRIMARY KEY (UserID, RoleID)
)
COMMENT = 'This table is the where we join users to roles to form the authorization for the system(s)'
;

CREATE TABLE IF NOT EXISTS security.tblUsers
(
    ID BIGINT PRIMARY KEY AUTO_INCREMENT UNIQUE ,
    Name VARCHAR(100) NOT NULL,
    FirstName VARCHAR(100) NULL,
    LastName VARCHAR(100) NULL,
    PhoneNumber VARCHAR(50) NULL,
    Active INTEGER NOT NULL DEFAULT (1),
    DateTimeCreatedUTC DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP(0),
    DateTimeLastUpdatedUTC DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP(0) ON UPDATE CURRENT_TIMESTAMP(0)
)
COMMENT = 'This is the main security identity where combined with security.lookup_Roles and security.tblUserRoles creates the authorization for the system(s).';
;

SELECT 'Starting FK scripts for security.tblUserIntegrations';

DROP PROCEDURE IF EXISTS security.AddUserIntegrationKeys;

CREATE PROCEDURE security.AddUserIntegrationKeys()
BEGIN
    IF NOT EXISTS (
        SELECT NULL
        FROM information_schema.TABLE_CONSTRAINTS
        WHERE
            CONSTRAINT_SCHEMA = DATABASE() AND
            CONSTRAINT_NAME   = 'fk_security_tblUserIntegrations_tblUsers_UserID' AND
            CONSTRAINT_TYPE   = 'FOREIGN KEY'
    )
    THEN
        ALTER TABLE security.tblUserIntegrations
        ADD CONSTRAINT fk_security_tblUserIntegrations_tblUsers_UserID
        FOREIGN KEY (UserID)
        REFERENCES security.tblUsers (ID);
    END IF;

    IF NOT EXISTS (
        SELECT NULL
        FROM information_schema.TABLE_CONSTRAINTS
        WHERE
            CONSTRAINT_SCHEMA = DATABASE() AND
            CONSTRAINT_NAME   = 'fk_security_tblUserIntegrations_lookupIntegrations_IntegrationID' AND
            CONSTRAINT_TYPE   = 'FOREIGN KEY'
    )
    THEN
        ALTER TABLE security.tblUserIntegrations
        ADD CONSTRAINT fk_security_tblUserIntegrations_lookupIntegrations_IntegrationID
        FOREIGN KEY (IntegrationID)
        REFERENCES security.lookup_Integrations (ID);
    END IF;
END;

CALL security.AddUserIntegrationKeys();

DROP PROCEDURE IF EXISTS security.AddUserIntegrationKeys;

SELECT 'Starting FK scripts for security.tblUserRoles';

DROP PROCEDURE IF EXISTS security.AddUserRoleKeys;

CREATE PROCEDURE security.AddUserRoleKeys()
BEGIN
    IF NOT EXISTS (
        SELECT NULL
        FROM information_schema.TABLE_CONSTRAINTS
        WHERE
            CONSTRAINT_SCHEMA = DATABASE() AND
            CONSTRAINT_NAME   = 'fk_security_tblUserRoles_tblUsers_UserID' AND
            CONSTRAINT_TYPE   = 'FOREIGN KEY'
    )
    THEN
        ALTER TABLE security.tblUserRoles
        ADD CONSTRAINT fk_security_tblUserRoles_tblUsers_UserID
        FOREIGN KEY (UserID)
        REFERENCES security.tblUsers (ID);
    END IF;

    IF NOT EXISTS (
        SELECT NULL
        FROM information_schema.TABLE_CONSTRAINTS
        WHERE
            CONSTRAINT_SCHEMA = DATABASE() AND
            CONSTRAINT_NAME   = 'fk_security_tblUserRoles_tblRoles_RoleID' AND
            CONSTRAINT_TYPE   = 'FOREIGN KEY'
    )
    THEN
        ALTER TABLE security.tblUserRoles
        ADD CONSTRAINT fk_security_tblUserRoles_tblRoles_RoleID
        FOREIGN KEY (RoleID)
        REFERENCES security.lookup_Roles (ID);
    END IF;
END;

CALL security.AddUserRoleKeys();


DROP PROCEDURE IF EXISTS security.AddUserRoleKeys;

CREATE TEMPORARY TABLE IF NOT EXISTS security_integrations SELECT * FROM security.lookup_Integrations LIMIT 0;

INSERT INTO security_integrations
(ID, IntegrationName, Active)
VALUES
(1, 'Firebase', 1);

INSERT INTO
    security.lookup_Integrations
(ID, IntegrationName, Active)
SELECT
si.ID, si.IntegrationName, si.Active
FROM security_integrations si
LEFT OUTER JOIN security.lookup_Integrations sli on si.ID = sli.ID
WHERE sli.ID IS NULL;

UPDATE security.lookup_Integrations sli
INNER JOIN security_integrations r on r.ID = sli.ID
SET sli.Active = r.Active, sli.IntegrationName = r.IntegrationName
WHERE r.Active <> sli.Active OR r.IntegrationName <> sli.IntegrationName;

DROP TABLE IF EXISTS security_integrations;

CREATE TEMPORARY TABLE IF NOT EXISTS security_roles_insert SELECT * FROM security.lookup_Roles LIMIT 0;

INSERT INTO security_roles_insert
(ID, RoleName, Active)
VALUES
(1, 'Admin', 1),
(2, 'User', 1);

INSERT INTO
    security.lookup_Roles
(ID, RoleName, Active)
SELECT
r.ID, r.RoleName, r.Active
FROM security_roles_insert r
LEFT OUTER JOIN security.lookup_Roles sri on r.ID = sri.ID
WHERE sri.ID IS NULL;

UPDATE security.lookup_Roles sri
INNER JOIN security_roles_insert r on r.ID = sri.ID
SET sri.Active = r.Active, sri.RoleName = r.RoleName
WHERE r.Active <> sri.Active OR r.RoleName <> sri.RoleName;

DROP TABLE security_roles_insert;
/*
 Reserved for the end of the master script.
 Can be DB changes or queries to be implemented that have to be done after all
 other object creation scripts
 */

SELECT 'Starting YY.post_WipedEveryRelease.sql';

/**************************************************/
/*          DO NOT EDIT ABOVE THIS LINE           */
/**************************************************/

/**************************************************/
/*          DO NOT EDIT BELOW THIS LINE           */
/**************************************************/

SELECT 'Ending YY.post_WipedEveryRelease.sql';
/*
 Reserved for the end of the master script.
 Can be DB changes or queries to be implemented that have to be done after all
 other object creation scripts
 */

SELECT 'Starting ZZ.post_Keep.sql';

/**************************************************/
/*          DO NOT EDIT ABOVE THIS LINE           */
/**************************************************/



/**************************************************/
/*          DO NOT EDIT BELOW THIS LINE           */
/**************************************************/

CALL security.DeclareDeployment('End');

SELECT 'Ending ZZ.post_Keep.sql';
SELECT 'Adding test user data';

/*************************************************************
  Users
*************************************************************/

DROP PROCEDURE IF EXISTS AddTestUsers;

CREATE PROCEDURE AddTestUsers()
BEGIN
    CREATE TEMPORARY TABLE IF NOT EXISTS security_users SELECT * FROM security.tblUsers LIMIT 0;

    INSERT INTO security_users
    (ID, Name, FirstName, LastName, PhoneNumber, Active)
    VALUES
    (1, 'Test User', 'Jeremy' , 'Snyder', '+1-952-555-1212', 1);

    ALTER TABLE security.tblUsers AUTO_INCREMENT=1;

    INSERT INTO
        security.tblUsers
    (Name, FirstName, LastName, PhoneNumber, Active)
    SELECT
           u.Name,
           u.FirstName,
           u.LastName,
           u.PhoneNumber,
           u.Active
    FROM security_users u
    LEFT OUTER JOIN security.tblUsers su ON
        u.ID = su.ID
    WHERE su.ID IS NULL;

    UPDATE security.tblUsers su
    INNER JOIN security_users u ON u.ID = su.ID
    SET su.Active = u.Active
    WHERE u.Active <> su.Active OR
          u.PhoneNumber <> su.PhoneNumber OR
          u.FirstName <> su.FirstName OR
          u.LastName <> su.LastName OR
          u.Name <> su.Name;

    ALTER TABLE security.tblUsers AUTO_INCREMENT=1000;

    DROP TABLE IF EXISTS security_users;
END;


CALL AddTestUsers();

DROP PROCEDURE IF EXISTS AddTestUsers;

/*************************************************************
  User Roles
*************************************************************/

DROP TABLE IF EXISTS security_userRoles;

CREATE TEMPORARY TABLE IF NOT EXISTS security_userRoles SELECT * FROM security.tblUserRoles LIMIT 0;

INSERT INTO security_userRoles
(UserID, RoleID, Active)
VALUES
(1, 1, 1),
(1, 2, 1);

INSERT INTO
    security.tblUserRoles
(UserID, RoleID, Active)
SELECT
si.UserID, si.RoleID, si.Active
FROM security_userRoles si
LEFT OUTER JOIN security.tblUserRoles sli ON
    si.UserID = sli.UserID AND
    si.RoleID = sli.RoleID
WHERE sli.UserID IS NULL;

UPDATE security.tblUserRoles sur
INNER JOIN security_userRoles ur ON
    ur.UserID = sur.UserID AND
    ur.RoleID = sur.RoleID
SET sur.Active = ur.Active
WHERE ur.Active <> sur.Active;

DROP TABLE IF EXISTS security_userRoles;

/*************************************************************
  User Integrations
*************************************************************/

DROP TABLE IF EXISTS security_userIntegrations;

CREATE TEMPORARY TABLE IF NOT EXISTS security_userIntegrations SELECT * FROM security.tblUserIntegrations LIMIT 0;

INSERT INTO security_userIntegrations
(UserID, IntegrationID, ExternalID)
VALUES
(1, 1, 'v2OcfN1HtPVm30JrSpfpnDhN3Tg1');

INSERT INTO
    security.tblUserIntegrations
(UserID, IntegrationID, ExternalID)
SELECT
ui.UserID, ui.IntegrationID, ui.ExternalID
FROM security_userIntegrations ui
LEFT OUTER JOIN security.tblUserIntegrations sui ON
    sui.UserID = ui.UserID AND
    sui.IntegrationID = ui.IntegrationID
WHERE sui.UserID IS NULL;

UPDATE security.tblUserIntegrations sui
INNER JOIN security.tblUserIntegrations ui ON
    ui.UserID = sui.UserID AND
    ui.IntegrationID = sui.IntegrationID
SET sui.ExternalID = ui.ExternalID
WHERE ui.ExternalID <> sui.ExternalID;


DROP TABLE IF EXISTS security_userIntegrations;
