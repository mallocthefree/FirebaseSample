
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
    DECLARE now DATETIME DEFAULT (UTC_TIMESTAMP());

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

CREATE TABLE IF NOT EXISTS security.lookup_Roles
(
    ID INT PRIMARY KEY AUTO_INCREMENT UNIQUE ,
    RoleName VARCHAR(50) NOT NULL,
    Active INT NOT NULL DEFAULT(1),
    DateTimeCreatedUTC DATETIME NOT NULL DEFAULT (UTC_TIMESTAMP()),
    DateTimeLastUpdatedUTC DATETIME NOT NULL DEFAULT (UTC_TIMESTAMP())
)
COMMENT = 'This entity contains all the roles that the system will need, e.g. User and Admin'
;

CREATE TABLE IF NOT EXISTS security.tblUserRoles
(
    UserID BIGINT NOT NULL,
    RoleID INT NOT NULL,
    Active INT DEFAULT(1),
    DateTimeCreatedUTC DATETIME NOT NULL DEFAULT (UTC_TIMESTAMP()),
    DateTimeLastUpdatedUTC DATETIME NOT NULL DEFAULT (UTC_TIMESTAMP()),
    UNIQUE (UserID, RoleID)
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
    DateTimeCreatedUTC DATETIME NOT NULL DEFAULT (UTC_TIMESTAMP()),
    DateTimeLastUpdatedUTC DATETIME NOT NULL DEFAULT (UTC_TIMESTAMP())
)
COMMENT = 'This is the main security identity where combined with security.lookup_Roles and security.tblUserRoles creates the authorization for the system(s).';
;
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
