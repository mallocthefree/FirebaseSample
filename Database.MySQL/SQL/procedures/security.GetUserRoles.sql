
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
