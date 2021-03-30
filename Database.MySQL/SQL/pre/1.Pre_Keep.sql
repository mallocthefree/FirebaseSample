/*
 Reserved for the start of the master script.
 Can be DB changes or queries to be implemented that have to be done before all
 other object creation scripts
 */


DELIMITER //

SELECT 'Starting 1.Pre_keep.sql';

CREATE TABLE IF NOT EXISTS security.tblDeployment
(
    ID BIGINT PRIMARY KEY AUTO_INCREMENT UNIQUE ,
    Version VARCHAR(100) NOT NULL,
    Type VARCHAR(100) NOT NULL DEFAULT ('Start'),
    DateTimeDeployedUTC DATETIME NOT NULL DEFAULT (UTC_TIMESTAMP())
)
COMMENT = 'Keeps track of SQL deployments'
;

DROP PROCEDURE IF EXISTS security.DeclareDeployment;

CREATE PROCEDURE security.DeclareDeployment(state CHAR(10))
BEGIN

    DECLARE count INT DEFAULT (0);
    DECLARE versionStr VARCHAR(100);
    DECLARE now DATETIME DEFAULT (UTC_TIMESTAMP);

    SELECT ((COUNT(*) / 2) + 1)
    INTO count
    FROM security.tblDeployment
    WHERE DateTimeDeployedUTC > CAST(DATE_FORMAT(now, '%Y.%m.01') AS DATE)
          AND Type = state;

    SELECT count;

    SELECT CONCAT
        (
            CAST(DATE_FORMAT(now, '%Y.%m.') AS CHAR(10)),
            LPAD(count, 3, '0')
        )
    INTO versionStr;

    INSERT INTO security.tblDeployment
        (Version, Type)
    VALUES
        (versionStr, state);
END;

CALL security.DeclareDeployment('Start');

/**************************************************/
SELECT 'Ending 1.Pre_keep.sql';
