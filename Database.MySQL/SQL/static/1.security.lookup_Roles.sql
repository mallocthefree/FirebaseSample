
CREATE TEMPORARY TABLE IF NOT EXISTS security_roles_insert SELECT * FROM security.lookup_Roles LIMIT 0;

INSERT INTO security_roles_insert
(ID, RoleName, Active)
VALUES
(1, 'Admin', 1),
(2, 'User', 1);

INSERT INTO
    security.lookup_Roles
(ID, RoleName, Active)
SELECT
r.ID, r.RoleName, r.Active
FROM security_roles_insert r
LEFT OUTER JOIN security.lookup_Roles sri on r.ID = sri.ID
WHERE sri.ID IS NULL;

UPDATE security.lookup_Roles sri
INNER JOIN security_roles_insert r on r.ID = sri.ID
SET sri.Active = r.Active, sri.RoleName = r.RoleName
WHERE r.Active <> sri.Active OR r.RoleName <> sri.RoleName;

DROP TABLE security_roles_insert;
