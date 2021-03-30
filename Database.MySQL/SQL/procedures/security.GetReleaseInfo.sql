

SELECT 'Creating procedure security.fn_GetReleaseInfo';

DROP PROCEDURE IF EXISTS security.GetReleaseInfo;

CREATE PROCEDURE security.GetReleaseInfo()
BEGIN
/**********************************************************************************************
*   Copyright (c) Jeremy Snyder Consulting, 2021
*
*   Initial Author:     Jeremy Snyder
*   Initial Version:    March 30, 2021
*   Description:        Get SQL release information
*
*   Parameters:
*
*   Results:
*       Get data about the 10 most recent releases
*
*   Revision History:
*
*   Examples:

CALL security.GetReleaseInfo();

***********************************************************************************************/

    WITH ReleaseVersions AS
        (
            SELECT DISTINCT sd.Version
            FROM security.tblDeployment sd
            GROUP BY sd.Version
        )
        SELECT rv.Version,
               started.DateTimeDeployedUTC AS DateTimeStarted,
               completed.DateTimeDeployedUTC AS DateTimeCompleted,
               TIME_TO_SEC(TIMEDIFF(completed.DateTimeDeployedUTC, started.DateTimeDeployedUTC)) AS Seconds
        FROM ReleaseVersions rv
        INNER JOIN security.tblDeployment started
            ON rv.Version = started.Version AND started.Type = 'Start'
        INNER JOIN security.tblDeployment completed
            ON rv.Version = completed.Version AND completed.Type = 'End'
        ORDER BY started.DateTimeDeployedUTC DESC
        LIMIT 10;

END;

