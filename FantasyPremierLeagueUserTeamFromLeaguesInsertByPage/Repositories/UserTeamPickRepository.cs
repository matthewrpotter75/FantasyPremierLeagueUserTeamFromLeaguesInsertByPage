using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Dapper;
using DapperExtensions;
using DataStreams.ETL;

namespace FantasyPremierLeagueUserTeams
{
    public class UserTeamPickRepository : IUserTeamPick
    {
        public int InsertUserTeamPick(UserTeamPicks userTeamPicks, SqlConnection db)
        {
            int rowsAffected = 0;

            try
            {
                using (IDataReader reader = userTeamPicks.GetDataReader())
                {
                    using (var bulkCopy = new SqlBulkCopy(db))
                    {
                        bulkCopy.BulkCopyTimeout = 0;
                        bulkCopy.BatchSize = 1000;
                        bulkCopy.DestinationTableName = "UserTeamPickStaging";
                        bulkCopy.EnableStreaming = true;

                        // Add your column mappings here
                        bulkCopy.ColumnMappings.Add("element", "playerid");
                        bulkCopy.ColumnMappings.Add("position", "position");
                        bulkCopy.ColumnMappings.Add("is_captain", "is_captain");
                        bulkCopy.ColumnMappings.Add("is_vice_captain", "is_vice_captain");
                        bulkCopy.ColumnMappings.Add("multiplier", "multiplier");
                        bulkCopy.ColumnMappings.Add("userteamid", "userteamid");
                        bulkCopy.ColumnMappings.Add("gameweekid", "gameweekid");

                        //using (var dataReader = userTeamPicks.ToDataReader())
                        //{
                        bulkCopy.WriteToServer(reader);
                        rowsAffected = SqlBulkCopyExtension.RowsCopiedCount(bulkCopy);
                        //}
                    }
                }
                return rowsAffected;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamPick Repository (insert) error: " + ex.Message);
                return rowsAffected;
                //throw ex;
            }
        }

        public bool UpdateUserTeamPick(UserTeamPick userTeamPick, SqlConnection db)
        {
            try
            {
                bool rowsUpdated = false;

                rowsUpdated = db.Update(userTeamPick, commandTimeout: 0);

                if (rowsUpdated == true)
                {
                    Logger.Out("UserTeamPick: Position:" + Convert.ToString(userTeamPick.position) + " PlayerId: " + Convert.ToString(userTeamPick.element) + " - updated");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamPick Repository (update) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeamPick(int userTeamid, SqlConnection db)
        {
            try
            {
                bool rowsDeleted = false;
                var userTeamPick = new UserTeamPick();

                userTeamPick = db.Get<UserTeamPick>(userTeamid);
                rowsDeleted = db.Delete(new UserTeamPick() { userteamid = userTeamid });

                if (rowsDeleted == true)
                {
                    Logger.Out("UserTeamPick: Position:" + Convert.ToString(userTeamPick.position) + " PlayerId: " + Convert.ToString(userTeamPick.element) + " - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamPick Repository (DeleteUserTeamPick) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeamPickId(int userTeamid, int userTeamPickid, SqlConnection db)
        {
            try
            {
                string userTeamPickPosition;
                string userTeamPickPlayerId;
                int rowsDeleted;

                var userTeamPick = db.Get<UserTeamPick>(userTeamPickid);
                userTeamPickPosition = Convert.ToString(userTeamPick.position);
                userTeamPickPlayerId = Convert.ToString(userTeamPick.element);

                string deleteQuery = "DELETE FROM dbo.UserTeamPick WHERE id = @UserTeamPickId;";
                rowsDeleted = db.Execute(deleteQuery, new { UserTeamPickId = userTeamPickid });

                if (rowsDeleted > 0)
                {
                    Logger.Out("UserTeamPick: Position:" + userTeamPickPosition + " PlayerId: " + userTeamPickPlayerId + " - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamPick Repository (DeleteUserTeamPickId) error: " + ex.Message);
                throw ex;
            }
        }

        //public List<UserTeamPickId> GetAllUserTeamPickIdsForUserTeamIdAndGameweekId(UserTeamPickId userTeamPickId, SqlConnection db)
        //{
        //    try
        //    {
        //        int userTeamId, gameweekId;

        //        userTeamId = userTeamPickId.userteamid;
        //        gameweekId = userTeamPickId.gameweekid;

        //        //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
        //        //{
        //            string selectQuery = @"SELECT DISTINCT userteamid, gameweekid FROM dbo.UserTeamPick WHERE userteamid = @UserTeamId AND gameweekid = @GameweekId;";

        //            IDataReader reader = db.ExecuteReader(selectQuery, new { UserTeamId = userTeamId, GameweekId = gameweekId }, commandTimeout: 300);

        //            List<UserTeamPickId> result = ReadList(reader);

        //            return result;
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("UserTeamPick Repository (GetAllUserTeamPickIdsForUserTeamIdAndGameweekId) error: " + ex.Message);
        //        throw ex;
        //    }
        //}

        public List<UserTeamPickId> GetAllUserTeamPickIdsForUserTeamIdAndGameweekId(int userTeamId, int gameweekId, SqlConnection db)
        {
            try
            {
                using (IDbCommand cmd = db.CreateCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetAllUserTeamPickIdsForUserTeamIdAndGameweekId";

                    IDataParameter param1 = cmd.CreateParameter();
                    param1.ParameterName = "@UserTeamId";
                    param1.Value = userTeamId;
                    cmd.Parameters.Add(param1);

                    IDataParameter param2 = cmd.CreateParameter();
                    param2.ParameterName = "@GameweekId";
                    param2.Value = gameweekId;
                    cmd.Parameters.Add(param2);

                    //string selectQuery = @"SELECT userteamid, gameweekid, position FROM dbo.UserTeamPick WHERE userteamid = @UserTeamId AND gameweekid = @GameweekId;";

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        List<UserTeamPickId> result = ReadUserTeamPickIdList(reader);

                        reader.Close();
                        reader.Dispose();

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamPick Repository (GetAllUserTeamPickIdsForUserTeamIdAndGameweekId) error: " + ex.Message);
                throw ex;
            }
        }

        public DataTable GetMaxGameweekIdFromUserTeamPickForUserTeamIds(List<int> userTeamIds, SqlConnection db)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    int i = 1;
                    var name = "";
                    foreach (var userTeamId in userTeamIds)
                    {
                        name = "@UserTeamId" + i++;
                        cmd.Parameters.AddWithValue(name, userTeamId);
                    }

                    int parameterCount = cmd.Parameters.Count;

                    if (parameterCount < 50)
                    {
                        for (i = parameterCount + 1; i <= 50; i++)
                        {
                            name = "@UserTeamId" + i++;
                            cmd.Parameters.AddWithValue(name, 0);
                        }
                    }

                    cmd.Connection = db;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetMaxGameweekIdForUserTeamIdsFromUserTeamPick";

                    //Create SqlDataAdapter and DataTable
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable result = new DataTable();

                    // this will query your database and return the result to your datatable
                    da.Fill(result);
                    da.Dispose();

                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamPick Repository (GetAllUserTeamPickIdsForUserTeamIdAndGameweekId) error: " + ex.Message);
                throw ex;
            }
        }

        //public DataTable GetMaxGameweekIdFromUserTeamPickForUserTeamIds(List<int> userTeamIds, SqlConnection db)
        //{
        //    try
        //    {
        //        using (SqlCommand cmd = new SqlCommand())
        //        {
        //            List<string> sqlParams = new List<string>();

        //            int i = 0;
        //            foreach (var value in userTeamIds)
        //            {
        //                var name = "@UserTeamId" + i++;
        //                cmd.Parameters.AddWithValue(name, value);
        //                sqlParams.Add(name);
        //            }

        //            string paramNames = string.Join(",", sqlParams);

        //            string selectQuery = @"SELECT userteamid,MAX(gameweekid) AS gameweekid FROM dbo.UserTeamPick WHERE userteamid IN (" + paramNames + ") GROUP BY userteamid;";

        //            cmd.Connection = db;
        //            cmd.CommandTimeout = 100;
        //            cmd.CommandType = CommandType.Text;
        //            cmd.CommandText = selectQuery;

        //            //Create SqlDataAdapter and DataTable
        //            SqlDataAdapter da = new SqlDataAdapter(cmd);
        //            DataTable result = new DataTable();

        //            // this will query your database and return the result to your datatable
        //            da.Fill(result);
        //            da.Dispose();

        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("UserTeamPick Repository (GetAllUserTeamPickIdsForUserTeamIdAndGameweekId) error: " + ex.Message);
        //        throw ex;
        //    }
        //}

        public List<int> GetAllUserTeamPickIds(SqlConnection db)
        {
            try
            {
                using (IDbCommand cmd = db.CreateCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetAllUserTeamPickIds";

                    //string selectQuery = @"SELECT DISTINCT userteamid AS id FROM dbo.UserTeamPick;";

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        List<int> result = ReadList(reader);

                        reader.Close();
                        reader.Dispose();

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamPick Repository (GetAllUserTeamPickIds) error: " + ex.Message);
                throw ex;
            }
        }

        public int GetMaxGameweekId(int userTeamId, SqlConnection db)
        {
            try
            {
                using (IDbCommand cmd = db.CreateCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetMaxGameweekIdForUserTeamIdFromUserTeamPick";

                    IDataParameter param = cmd.CreateParameter();
                    param.ParameterName = "@UserTeamId";
                    param.Value = userTeamId;
                    cmd.Parameters.Add(param);

                    //string selectQuery = @"SELECT ISNULL(MAX(gameweekid),0) FROM dbo.UserTeamPick WHERE userteamid = @UserTeamId;";

                    int result = Convert.ToInt32(cmd.ExecuteScalar());

                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamPick Repository (GetMaxGameweekId) error: " + ex.Message);
                throw ex;
            }
        }

        //public string GetUserTeamPickName(int userTeamPickId, SqlConnection db)
        //{
        //    //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
        //    //{
        //        var userTeamPick = db.Get<UserTeamPick>(userTeamPickId);

        //        string userTeamPickName = userTeamPick.season_name;

        //        return userTeamPickName;
        //    //}
        //}

        //public List<int> GetCompetedUserTeamPickIds(SqlConnection db)
        //{
        //    //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
        //    //{
        //        string selectQuery = @"SELECT utc.id FROM dbo.UserTeamPick utc INNER JOIN dbo.Gameweeks g ON h.gameweekId = g.id WHERE g.id = (SELECT TOP 1 id FROM dbo.Gameweeks WHERE deadline_time < GETDATE() ORDER BY deadline_time DESC)";

        //        IDataReader reader = db.ExecuteReader(selectQuery);

        //        List<int> result = ReadList(reader);

        //        return result;
        //    //}
        //}

        List<int> ReadList(IDataReader reader)
        {
            try
            {
                List<int> list = new List<int>();
                int column = reader.GetOrdinal("userteamid");

                while (reader.Read())
                {
                    //check for the null value and than add 
                    if (!reader.IsDBNull(column))
                        list.Add(reader.GetInt32(column));
                }
                return list;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamPick Repository (ReadUserTeamIdList) error: " + ex.Message);
                throw ex;
            }
        }

        List<UserTeamPickId> ReadUserTeamPickIdList(IDataReader reader)
        {
            try
            {
                List<UserTeamPickId> list = new List<UserTeamPickId>();
                int column1 = reader.GetOrdinal("userteamid");
                int column2 = reader.GetOrdinal("gameweekid");
                int column3 = reader.GetOrdinal("position");

                //UserTeamPickId userTeamPickId = new UserTeamPickId(0, 0, 0);
                //UserTeamPickId userTeamChipId = new UserTeamPickId();

                while (reader.Read())
                {
                    //check for the null value and than add 
                    if (!reader.IsDBNull(column1))
                    {
                        UserTeamPickId userTeamPickId = new UserTeamPickId(0, 0, 0);

                        userTeamPickId.userteamid = reader.GetInt32(column1);
                        userTeamPickId.gameweekid = reader.GetInt32(column2);
                        userTeamPickId.position = reader.GetInt32(column3);
                        list.Add(userTeamPickId);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamPick Repository (ReadUserTeamPickIdList) error: " + ex.Message);
                throw ex;
            }
        }
    }
}