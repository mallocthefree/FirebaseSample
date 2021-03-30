/*
 Reserved for the end of the master script.
 Can be DB changes or queries to be implemented that have to be done after all
 other object creation scripts
 */

SELECT 'Starting ZZ.post_Keep.sql';

/**************************************************/
/*          DO NOT EDIT ABOVE THIS LINE           */
/**************************************************/



/**************************************************/
/*          DO NOT EDIT BELOW THIS LINE           */
/**************************************************/

CALL rel.DeclareDeployment('End');

SELECT 'Ending ZZ.post_Keep.sql';

CALL rel.GetReleaseInfo();
