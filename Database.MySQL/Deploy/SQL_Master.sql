
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
            CAST(count AS CHAR(2))
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
    IntegrationTypeID INT          NOT NULL,
    ExternalID        VARCHAR(200) NOT NULL,
    PRIMARY KEY (UserID, IntegrationTypeID),
    UNIQUE (IntegrationTypeID, ExternalID)
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

SELECT 'Starting FK scripts for security.tblUserRoles';

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
        FOREIGN KEY (IntegrationTypeID)
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
SET sri.Active = r.Active, sri.RoleName = r.Active
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
