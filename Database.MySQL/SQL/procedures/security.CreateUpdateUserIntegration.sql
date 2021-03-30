
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
*   Copyright (c) Oso Vega, 2021
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
