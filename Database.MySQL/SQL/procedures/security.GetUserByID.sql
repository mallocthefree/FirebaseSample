
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
