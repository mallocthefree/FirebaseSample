
CREATE TEMPORARY TABLE IF NOT EXISTS security_integrations SELECT * FROM security.lookup_Integrations LIMIT 0;

INSERT INTO security_integrations
(ID, IntegrationName, Active)
VALUES
(1, 'Email', 1),
(2, 'Firebase', 1);

INSERT INTO
    security.lookup_Integrations
(ID, IntegrationName, Active)
SELECT
si.ID, si.IntegrationName, si.Active
FROM security_integrations si
LEFT OUTER JOIN security.lookup_Integrations sli on si.ID = sli.ID
WHERE sli.ID IS NULL;

UPDATE security.lookup_Integrations sli
INNER JOIN security_integrations r on r.ID = sli.ID
SET sli.Active = r.Active, sli.IntegrationName = r.IntegrationName
WHERE r.Active <> sli.Active OR r.IntegrationName <> sli.IntegrationName;

DROP TABLE IF EXISTS security_integrations;
