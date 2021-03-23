
SELECT 'Starting FK scripts for security.tblUserRoles';

DROP PROCEDURE IF EXISTS security.AddUserRoleKeys;

CREATE PROCEDURE security.AddUserRoleKeys()
BEGIN
    IF NOT EXISTS (
        SELECT NULL
        FROM information_schema.TABLE_CONSTRAINTS
        WHERE
            CONSTRAINT_SCHEMA = DATABASE() AND
            CONSTRAINT_NAME   = 'fk_security_tblUserRoles_tblUsers_UserID' AND
            CONSTRAINT_TYPE   = 'FOREIGN KEY'
    )
    THEN
        ALTER TABLE security.tblUserRoles
        ADD CONSTRAINT fk_security_tblUserRoles_tblUsers_UserID
        FOREIGN KEY (UserID)
        REFERENCES security.tblUsers (ID);
    END IF;

    IF NOT EXISTS (
        SELECT NULL
        FROM information_schema.TABLE_CONSTRAINTS
        WHERE
            CONSTRAINT_SCHEMA = DATABASE() AND
            CONSTRAINT_NAME   = 'fk_security_tblUserRoles_tblRoles_RoleID' AND
            CONSTRAINT_TYPE   = 'FOREIGN KEY'
    )
    THEN
        ALTER TABLE security.tblUserRoles
        ADD CONSTRAINT fk_security_tblUserRoles_tblRoles_RoleID
        FOREIGN KEY (RoleID)
        REFERENCES security.lookup_Roles (ID);
    END IF;
END;

CALL security.AddUserRoleKeys();


DROP PROCEDURE IF EXISTS security.AddUserRoleKeys;
