
CREATE TABLE IF NOT EXISTS security.lookup_Integrations
(
    ID INT PRIMARY KEY UNIQUE ,
    IntegrationName VARCHAR(50) NOT NULL,
    Active INT NOT NULL DEFAULT(1),
    DateTimeCreatedUTC DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP(0),
    DateTimeLastUpdatedUTC DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP(0) ON UPDATE CURRENT_TIMESTAMP(0)
)
COMMENT = 'This entity contains all the integrations that the system will use to tie a user to an external system.'
;
