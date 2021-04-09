
/*******************************************************************************
 *       HEADER [ SECURITY ]
 ******************************************************************************/
 
/*******************************************************************************
 *       TAIL [ SECURITY ]
 ******************************************************************************/
 
/*******************************************************************************
 *       HEADER [ ENVIRONMENT ]
 ******************************************************************************/
 
/*******************************************************************************
 *       TAIL [ ENVIRONMENT ]
 ******************************************************************************/
 
/*******************************************************************************
 *       HEADER [ SCHEMAS ]
 ******************************************************************************/
 
CREATE SCHEMA IF NOT EXISTS rel;

CREATE SCHEMA IF NOT EXISTS security;

/*******************************************************************************
 *       TAIL [ SCHEMAS ]
 ******************************************************************************/
 
/*******************************************************************************
 *       HEADER [ PRE ]
 ******************************************************************************/
 /*
 Reserved for the start of the master script.
 Can be DB changes or queries to be implemented that have to be done before all
 other object creation scripts
 */


DELIMITER //

SELECT 'Starting 1.Pre_keep.sql';

CREATE TABLE IF NOT EXISTS rel.tblDeployment
(
    ID BIGINT PRIMARY KEY AUTO_INCREMENT UNIQUE ,
    Version VARCHAR(100) NOT NULL,
    Type VARCHAR(100) NOT NULL DEFAULT ('Start'),
    DateTimeDeployedUTC DATETIME NOT NULL DEFAULT (UTC_TIMESTAMP())
)
COMMENT = 'Keeps track of SQL deployments'
;

DROP PROCEDURE IF EXISTS rel.DeclareDeployment;

CREATE PROCEDURE rel.DeclareDeployment(state CHAR(10))
BEGIN

    DECLARE count INT DEFAULT (0);
    DECLARE versionStr VARCHAR(100);
    DECLARE now DATETIME DEFAULT (UTC_TIMESTAMP);

    SELECT ((COUNT(*) / 2) + 1)
    INTO count
    FROM rel.tblDeployment
    WHERE DateTimeDeployedUTC > CAST(DATE_FORMAT(now, '%Y.%m.01') AS DATE)
          AND Type = state;

    SELECT count;

    SELECT CONCAT
        (
            CAST(DATE_FORMAT(now, '%Y.%m.') AS CHAR(10)),
            LPAD(count, 3, '0')
        )
    INTO versionStr;

    INSERT INTO rel.tblDeployment
        (Version, Type)
    VALUES
        (versionStr, state);
END;

CALL rel.DeclareDeployment('Start');

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

/*******************************************************************************
 *       TAIL [ PRE ]
 ******************************************************************************/
 
/*******************************************************************************
 *       HEADER [ TABLES ]
 ******************************************************************************/
 
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
    Active INTEGER NOT NULL DEFAULT (1),
    DateTimeCreatedUTC DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP(0),
    DateTimeLastUpdatedUTC DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP(0) ON UPDATE CURRENT_TIMESTAMP(0)
)
COMMENT = 'This is the main security identity where combined with security.lookup_Roles and security.tblUserRoles creates the authorization for the system(s).';
;

/*******************************************************************************
 *       TAIL [ TABLES ]
 ******************************************************************************/
 
/*******************************************************************************
 *       HEADER [ INDEXES ]
 ******************************************************************************/
 
/*******************************************************************************
 *       TAIL [ INDEXES ]
 ******************************************************************************/
 
/*******************************************************************************
 *       HEADER [ FKS ]
 ******************************************************************************/
 
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

/*******************************************************************************
 *       TAIL [ FKS ]
 ******************************************************************************/
 
/*******************************************************************************
 *       HEADER [ CONSTRAINTS ]
 ******************************************************************************/
 
/*******************************************************************************
 *       TAIL [ CONSTRAINTS ]
 ******************************************************************************/
 
/*******************************************************************************
 *       HEADER [ STATIC ]
 ******************************************************************************/
 
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

/*******************************************************************************
 *       TAIL [ STATIC ]
 ******************************************************************************/
 
/*******************************************************************************
 *       HEADER [ FUNCTIONS ]
 ******************************************************************************/
 
/*******************************************************************************
 *       TAIL [ FUNCTIONS ]
 ******************************************************************************/
 
/*******************************************************************************
 *       HEADER [ PROCEDURES ]
 ******************************************************************************/
 

SELECT 'Creating procedure release.fn_GetReleaseInfo';

DROP PROCEDURE IF EXISTS rel.GetReleaseInfo;

CREATE PROCEDURE rel.GetReleaseInfo()
BEGIN
/**********************************************************************************************
*   Copyright (c) Jeremy Snyder Consulting, 2021
*
*   Initial Author:     Jeremy Snyder
*   Initial Version:    March 30, 2021
*   Description:        Get SQL release information
*
*   Parameters:
*
*   Results:
*       Get data about the 10 most recent releases
*
*   Revision History:
*
*   Examples:

CALL rel.GetReleaseInfo();

***********************************************************************************************/

    WITH ReleaseVersions AS
        (
            SELECT DISTINCT sd.Version
            FROM rel.tblDeployment sd
            GROUP BY sd.Version
        )
        SELECT rv.Version,
               started.DateTimeDeployedUTC AS DateTimeStarted,
               completed.DateTimeDeployedUTC AS DateTimeCompleted,
               TIME_TO_SEC(TIMEDIFF(completed.DateTimeDeployedUTC, started.DateTimeDeployedUTC)) AS Seconds
        FROM ReleaseVersions rv
        INNER JOIN rel.tblDeployment started
            ON rv.Version = started.Version AND started.Type = 'Start'
        INNER JOIN rel.tblDeployment completed
            ON rv.Version = completed.Version AND completed.Type = 'End'
        ORDER BY started.DateTimeDeployedUTC DESC
        LIMIT 10;

END;


SELECT 'Creating function security.AddUpdateUserRole';

DROP PROCEDURE IF EXISTS security.AddUpdateUserRole;

CREATE PROCEDURE security.AddUpdateUserRole
    (
        IN pUserID BIGINT,
        IN pRoleID INT,
        IN pActive INT
    )
BEGIN
/**********************************************************************************************
*   Copyright (c) Jeremy Snyder Consulting, 2021
*
*   Initial Author:     Jeremy Snyder
*   Initial Version:    March 30, 2021
*   Description:        Add or update a user role
*
*   Parameters:
*               pUserID      The user's internal UserID
*               pRoleID      The ID of the role ( lookup_Roles )
*               pActive      Activate or deactivate a user's role
*
*   Results:
*       Return row added or updated in the tblUserIntegrations
*
*   Revision History:
*
*   Examples:

    CALL security.AddUpdateUserRole ( 1, 1, 1 );

***********************************************************************************************/

    IF EXISTS (SELECT 1
               FROM security.tblUserRoles ur
               WHERE ur.UserID = pUserID AND
                     ur.RoleID = pRoleID)
    THEN
        UPDATE security.tblUserRoles ur
        SET Active = pActive
        WHERE ur.UserID = pUserID AND
              ur.RoleID = pRoleID AND
              ur.Active <> pActive;
    ELSE
        INSERT INTO security.tblUserRoles
        (UserID, RoleID, Active)
        VALUES
        (pUserID, pRoleID, pActive);
    END IF;

    CALL security.GetUserRoles(pUserID);

END;

SELECT 'Creating function security.CreateUpdateUser';

DROP PROCEDURE IF EXISTS security.CreateUpdateUser;

CREATE PROCEDURE security.CreateUpdateUser
    (
        IN pUserID BIGINT,
        IN pName VARCHAR(100),
        IN pFirstName VARCHAR(100),
        IN pLastName VARCHAR(100),
        IN pActive INT,
        IN pIntegrationID INT,
        IN pExternalID VARCHAR(200)
    )
BEGIN
/**********************************************************************************************
*   Copyright (c) Jeremy Snyder Consulting, 2021
*
*   Initial Author:     Jeremy Snyder
*   Initial Version:    March 30, 2021
*   Description:        Create or update a user
*
*   Parameters:
*               pUserID  The user's internal UserID
*
*   Results:
*       Return table of added or updated user information
*
*   Revision History:
*
*   Examples:

    CALL security.CreateUpdateUser ( 1, 'Test User', 'Jeremy', 'Snyder', 1, 1, 'v2OcfN1HtPVm30JrSpfpnDhN3Tg1' );
    CALL security.CreateUpdateUser ( NULL, 'Other Test User', 'Jeremy', 'Snyder', 1, 1, 'testKey' );

***********************************************************************************************/

    IF EXISTS (SELECT 1
               FROM security.tblUserIntegrations
               WHERE IntegrationID = pIntegrationID AND
                     ExternalID = pExternalID)
    THEN
        SELECT UserID
        FROM security.tblUserIntegrations
        WHERE IntegrationID = pIntegrationID AND
              ExternalID = pExternalID
        LIMIT 1
        INTO pUserID;
    END IF;

    IF pUserID > 0 AND
       pUserID IS NOT NULL
    THEN
        UPDATE security.tblUsers u
        SET Name = pName,
            FirstName = pFirstName,
            LastName = pLastName,
            Active = pActive
        WHERE u.ID = pUserID AND
              (
                  Name <> pName OR
                  FirstName <> pFirstName OR
                  LastName <> pLastName OR
                  Active <> pActive
              );
    ELSE
        INSERT INTO security.tblUsers
        (Name, FirstName, LastName, Active)
        VALUES
        (pName, pFirstName, pLastName, pActive);

        SELECT LAST_INSERT_ID() INTO pUserID;
    END IF;

    CALL security.CreateUpdateUserIntegration(pUserID, pIntegrationID, pExternalID);

    CALL security.GetUserByID ( pUserID );

END;

SELECT 'Creating function security.CreateUpdateUserIntegration';

DROP PROCEDURE IF EXISTS security.CreateUpdateUserIntegration;

CREATE PROCEDURE security.CreateUpdateUserIntegration
    (
        IN pUserID BIGINT,
        IN pIntegrationID INT,
        IN pExternalID VARCHAR(200)
    )
BEGIN
/**********************************************************************************************
*   Copyright (c) Jeremy Snyder Consulting, 2021
*
*   Initial Author:     Jeremy Snyder
*   Initial Version:    March 30, 2021
*   Description:        Create or update a user integration externalID
*
*   Parameters:
*               pUserID          The user's internal UserID
*               pIntegrationID   The ID of the integration type ( lookup_Integrations )
*               pExternalID      The user's ID in the integrated system
*
*   Results:
*       Return row added or updated in the tblUserIntegrations
*
*   Revision History:
*
*   Examples:

   CALL security.CreateUpdateUserIntegration ( 1, 1, 'v2OcfN1HtPVm30JrSpfpnDhN3Tg1' );

***********************************************************************************************/

    IF EXISTS (SELECT 1
               FROM security.tblUserIntegrations ui
               WHERE UserID = pUserID AND
                     IntegrationID = pIntegrationID)
    THEN
        UPDATE security.tblUserIntegrations ui
        SET ExternalID = pExternalID
        WHERE ui.UserID = pUserID AND
              ui.IntegrationID = pIntegrationID;
    ELSE
        INSERT INTO security.tblUserIntegrations
        (UserID, IntegrationID, ExternalID)
        VALUES
        (pUserID, pIntegrationID, pExternalID);
    END IF;

    SELECT ui.UserID,
           ui.IntegrationID,
           ui.ExternalID
    FROM security.tblUserIntegrations ui
    WHERE ui.UserID = pUserID AND
          ui.IntegrationID = pIntegrationID;

END;

SELECT 'Creating function security.GetRoles';

DROP PROCEDURE IF EXISTS security.GetRoles;

CREATE PROCEDURE security.GetRoles()
BEGIN
/**********************************************************************************************
*   Copyright (c) Jeremy Snyder Consulting, 2021
*
*   Initial Author:     Jeremy Snyder
*   Initial Version:    March 30, 2021
*   Description:        Get all Roles
*
*   Parameters:
*
*   Results:
*       Return table of active RoleID and RoleNames
*
*   Revision History:
*
*   Examples:

    CALL security.GetRoles();

***********************************************************************************************/

    SELECT
           lr.ID,
           lr.RoleName
    FROM security.lookup_Roles lr
    WHERE lr.Active= 1
    ORDER BY lr.RoleName;

END;

SELECT 'Creating function security.GetUserByID';

DROP PROCEDURE IF EXISTS security.GetUserByID;

CREATE PROCEDURE security.GetUserByID( IN userID BIGINT )
BEGIN
/**********************************************************************************************
*   Copyright (c) Jeremy Snyder Consulting, 2021
*
*   Initial Author:     Jeremy Snyder
*   Initial Version:    March 30, 2021
*   Description:        Get all user information from the given UserID
*
*   Parameters:
*               userID  The user's internal UserID
*
*   Results:
*       Return table of user information
*
*   Revision History:
*
*   Examples:

    CALL security.GetUserByID ( 1 );

***********************************************************************************************/

/*
 In most SQL queries I would represent the full extent of the data using a cross apply. However,
 since we are only interested in data related to security as this is a 'security' schema procedure, I
 will then apply a direct query to include only the IntegrationID = 1 which we know to be Firebase.
 */
    SELECT
           u.ID,
           u.Name,
           u.FirstName,
           u.LastName,
           ui.ExternalID,
           u.Active
    FROM security.tblUsers u
        LEFT OUTER JOIN security.tblUserIntegrations ui
            ON u.ID = ui.UserID AND ui.IntegrationID = 1 /* Currently enforce only Firebase in response */
    WHERE u.ID = userID;
/* Additional note:
   ui.IntegrationID = 1 could also have been part of the WHERE clause. I add it to the join because it
   will filter out at that layer before the WHERE is provided. Most modern databases will optimize this
   for us now. However, I will usually assume nothing.
 */

END;

SELECT 'Creating function security.GetUserIDByExternalID';

DROP PROCEDURE IF EXISTS GetUserIDByExternalID;

CREATE PROCEDURE security.GetUserIDByExternalID
    (
        IN integrationID BIGINT,
        IN externalID VARCHAR(200)
    )
BEGIN
/**********************************************************************************************
*   Copyright (c) Jeremy Snyder Consulting, 2021
*
*   Initial Author:     Jeremy Snyder
*   Initial Version:    March 30, 2021
*   Description:        Get UserID from security.tblUserIntegrations
*
*   Parameters:
*               integrationID
*               externalID
*   Results:
*               UserID of the user if successful. Else 0
*
*   Revision History:
*
*   Examples:

CALL security.GetUserIDByExternalID( 1, 'v2OcfN1HtPVm30JrSpfpnDhN3Tg1' );

***********************************************************************************************/

        SELECT  0 AS UserID
        UNION
        SELECT  ui.UserID
        FROM    security.tblUserIntegrations  ui
        WHERE   ui.IntegrationID = integrationID AND
                ui.ExternalID  = externalID
        ORDER BY UserID DESC
        LIMIT 1;

END;

SELECT 'Creating function security.GetUserRoles';

DROP PROCEDURE IF EXISTS security.GetUserRoles;

CREATE PROCEDURE security.GetUserRoles( IN pUserID BIGINT )
BEGIN
/**********************************************************************************************
*   Copyright (c) Jeremy Snyder Consulting, 2021
*
*   Initial Author:     Jeremy Snyder
*   Initial Version:    March 30, 2021
*   Description:        Get Roles by userID
*
*   Parameters:
*               userID         ID of the User ( security.tblUsers )
*                              Using NULL will return all of the users.
*   Results:
*       Return a row(s) with roles by userID
*
*   Revision History:
*
*   Examples:

    CALL security.GetUserRoles(null);
    CALL security.GetUserRoles(1);
    CALL security.GetUserRoles(2);

***********************************************************************************************/

    SELECT
           ur.UserID,
           ur.RoleID,
           lr.RoleName
    FROM security.tblUserRoles ur
        INNER JOIN security.tblUsers u
            ON ur.UserID = u.ID
        INNER JOIN security.lookup_Roles lr
            ON ur.roleid = lr.ID
    WHERE ur.Active = 1 AND
          u.Active = 1 AND
          lr.Active = 1 AND
          ur.UserID = COALESCE( pUserID,  ur.UserID )
    ORDER BY ur.UserID;

END;

/*******************************************************************************
 *       TAIL [ PROCEDURES ]
 ******************************************************************************/
 
/*******************************************************************************
 *       HEADER [ TESTDATA ]
 ******************************************************************************/
 SELECT 'Adding test user data';

/*************************************************************
  Users
*************************************************************/

DROP PROCEDURE IF EXISTS AddTestUsers;

CREATE PROCEDURE AddTestUsers()
BEGIN
    CREATE TEMPORARY TABLE IF NOT EXISTS security_users SELECT * FROM security.tblUsers LIMIT 0;

    INSERT INTO security_users
    (ID, Name, FirstName, LastName, Active)
    VALUES
    (1, 'Test User', 'Jeremy' , 'Snyder', 1);

    ALTER TABLE security.tblUsers AUTO_INCREMENT=1;

    INSERT INTO
        security.tblUsers
    (Name, FirstName, LastName, Active)
    SELECT
           u.Name,
           u.FirstName,
           u.LastName,
           u.Active
    FROM security_users u
    LEFT OUTER JOIN security.tblUsers su ON
        u.ID = su.ID
    WHERE su.ID IS NULL;

    UPDATE security.tblUsers su
    INNER JOIN security_users u ON u.ID = su.ID
    SET su.Active = u.Active
    WHERE u.Active <> su.Active OR
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

/*******************************************************************************
 *       TAIL [ TESTDATA ]
 ******************************************************************************/
 
/*******************************************************************************
 *       HEADER [ POST ]
 ******************************************************************************/
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

CALL rel.DeclareDeployment('End');

SELECT 'Ending ZZ.post_Keep.sql';

CALL rel.GetReleaseInfo();

/*******************************************************************************
 *       TAIL [ POST ]
 ******************************************************************************/
 