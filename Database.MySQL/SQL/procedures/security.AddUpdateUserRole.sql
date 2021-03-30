
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
*   Copyright (c) Oso Vega, 2021
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
