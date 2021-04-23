/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>April 22, 2021</date>
/////////////////////////////////////////////////////////


using System.Collections.Generic;
using System.Data;
using Dapper;

namespace JeremySnyder.Shared.Data
{
    public abstract class RepositoryBase
    {
        public static IEnumerable<T> Query<T>(string connectionName, string schema, string function, string parameterList = "", object parameters = null)
        {
            using (var connection = new SQLConnection(connectionName))
            {
                using (var dbConnection = connection.Connection)
                {
                    dbConnection.Open();
                    string query = $"CALL {schema}.{function}({parameterList});";
                    var result = dbConnection.Query<T>
                    (
                        query,
                        parameters,
                        commandType: CommandType.Text
                    );

                    return result ?? new List<T>();
                }
            }
        }
    }
}
