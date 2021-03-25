
CREATE TABLE IF NOT EXISTS security.tblUserRoles
(
    UserID BIGINT NOT NULL,
    RoleID INT NOT NULL,
    Active INT DEFAULT(1),
    DateTimeCreatedUTC DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP(0),
    DateTimeLastUpdatedUTC DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP(0) ON UPDATE CURRENT_TIMESTAMP(0),
    PRIMARY KEY (UserID, RoleID)
)
COMMENT = 'This table is the where we join users to roles to form the authorization for the system(s)'
;
