
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

    CALL security.CreateUpdateUser ( 1, 'Test User', 'Jeremy', 'Snyder', 1, 2, 'v2OcfN1HtPVm30JrSpfpnDhN3Tg1' );
    CALL security.CreateUpdateUser ( NULL, 'Other Test User', 'Jeremy', 'Snyder', 1, 2, 'testKey' );

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
