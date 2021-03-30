
CREATE TABLE IF NOT EXISTS security.tblUsers
(
    ID BIGINT PRIMARY KEY AUTO_INCREMENT UNIQUE ,
    Name VARCHAR(100) NOT NULL,
    FirstName VARCHAR(100) NULL,
    LastName VARCHAR(100) NULL,
    Active INTEGER NOT NULL DEFAULT (1),
    DateTimeCreatedUTC DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP(0),
    DateTimeLastUpdatedUTC DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP(0) ON UPDATE CURRENT_TIMESTAMP(0)
)
COMMENT = 'This is the main security identity where combined with security.lookup_Roles and security.tblUserRoles creates the authorization for the system(s).';
;
