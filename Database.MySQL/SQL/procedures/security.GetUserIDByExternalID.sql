
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
