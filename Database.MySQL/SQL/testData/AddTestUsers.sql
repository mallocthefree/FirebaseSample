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
(1, 1, 'jeremysnyder.consulting@gmail.com'),
(1, 2, 'v2OcfN1HtPVm30JrSpfpnDhN3Tg1');

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
