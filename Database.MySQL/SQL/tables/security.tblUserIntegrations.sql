

SELECT 'Create table script for security.tblUserIntegrations';
CREATE TABLE IF NOT EXISTS security.tblUserIntegrations
(
    UserID            BIGINT       NOT NULL,
    IntegrationTypeID INT          NOT NULL,
    ExternalID        VARCHAR(200) NOT NULL,
    PRIMARY KEY (UserID, IntegrationTypeID),
    UNIQUE (IntegrationTypeID, ExternalID)
)
COMMENT = 'This table to to contain integration IDs from external sites in conjunction with our internal users.
           Examples of integrations may include: Firebase, HubSpot, SalesForce, or any other external system.'
;
