
CREATE TABLE IF NOT EXISTS security.lookup_Roles
(
    ID INT PRIMARY KEY AUTO_INCREMENT UNIQUE ,
    RoleName VARCHAR(50) NOT NULL,
    Active INT NOT NULL DEFAULT(1),
    DateTimeCreatedUTC DATETIME NOT NULL DEFAULT (UTC_TIMESTAMP()),
    DateTimeLastUpdatedUTC DATETIME NOT NULL DEFAULT (UTC_TIMESTAMP())
)
COMMENT = 'This entity contains all the roles that the system will need, e.g. User and Admin'
;
