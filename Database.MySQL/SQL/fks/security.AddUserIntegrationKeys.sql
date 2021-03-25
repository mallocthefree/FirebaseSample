
SELECT 'Starting FK scripts for security.tblUserIntegrations';

DROP PROCEDURE IF EXISTS security.AddUserIntegrationKeys;

CREATE PROCEDURE security.AddUserIntegrationKeys()
BEGIN
    IF NOT EXISTS (
        SELECT NULL
        FROM information_schema.TABLE_CONSTRAINTS
        WHERE
            CONSTRAINT_SCHEMA = DATABASE() AND
            CONSTRAINT_NAME   = 'fk_security_tblUserIntegrations_tblUsers_UserID' AND
            CONSTRAINT_TYPE   = 'FOREIGN KEY'
    )
    THEN
        ALTER TABLE security.tblUserIntegrations
        ADD CONSTRAINT fk_security_tblUserIntegrations_tblUsers_UserID
        FOREIGN KEY (UserID)
        REFERENCES security.tblUsers (ID);
    END IF;

    IF NOT EXISTS (
        SELECT NULL
        FROM information_schema.TABLE_CONSTRAINTS
        WHERE
            CONSTRAINT_SCHEMA = DATABASE() AND
            CONSTRAINT_NAME   = 'fk_security_tblUserIntegrations_lookupIntegrations_IntegrationID' AND
            CONSTRAINT_TYPE   = 'FOREIGN KEY'
    )
    THEN
        ALTER TABLE security.tblUserIntegrations
        ADD CONSTRAINT fk_security_tblUserIntegrations_lookupIntegrations_IntegrationID
        FOREIGN KEY (IntegrationID)
        REFERENCES security.lookup_Integrations (ID);
    END IF;
END;

CALL security.AddUserIntegrationKeys();

DROP PROCEDURE IF EXISTS security.AddUserIntegrationKeys;
