
SELECT 'Creating function security.GetRoles';

DROP PROCEDURE IF EXISTS security.GetRoles;

CREATE PROCEDURE security.GetRoles()
BEGIN
/**********************************************************************************************
*   Copyright (c) Oso Vega, 2021
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
