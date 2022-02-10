using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using DapperExtensions;
using DataStreams.ETL;

namespace FantasyPremierLeagueUserTeams
{
    public class UserTeamTransferHistoryRepository : IUserTeamTransferHistory
    {
        public int InsertUserTeamTransferHistories(UserTeamTransferHistoryData userTeamTransferData, SqlConnection db)
        {
            int rowsAffected = 0;

            try
            {
                using (IDataReader reader = userTeamTransferData.GetDataReader())
                {
                    using (var bulkCopy = new SqlBulkCopy(db))
                    {
                        bulkCopy.BulkCopyTimeout = 0;
                        bulkCopy.BatchSize = 1000;
                        bulkCopy.DestinationTableName = "UserTeamTransferHistoryStaging";
                        bulkCopy.EnableStreaming = true;

                        // Add your column mappings here
                        bulkCopy.ColumnMappings.Add("time", "transfer_time");
                        bulkCopy.ColumnMappings.Add("element_in", "playerid_in");
                        bulkCopy.ColumnMappings.Add("element_in_cost", "player_in_cost");
                        bulkCopy.ColumnMappings.Add("element_out", "playerid_out");
                        bulkCopy.ColumnMappings.Add("element_out_cost", "player_out_cost");
                        bulkCopy.ColumnMappings.Add("entry", "userteamid");
                        bulkCopy.ColumnMappings.Add("event", "gameweekid");
                        bulkCopy.ColumnMappings.Add("userteamtransferhistoryid", "userteamtransferhistoryid");

                        bulkCopy.WriteToServer(reader);
                        rowsAffected = SqlBulkCopyExtension.RowsCopiedCount(bulkCopy);
                    }
                }
                return rowsAffected;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamTransferHistory Repository (insert) error: " + ex.Message);
                return rowsAffected;
                //throw ex;
            }
        }

        public bool UpdateUserTeamTransferHistory(UserTeamTransferHistory userTeamTransferHistory, SqlConnection db)
        {
            try
            {
                bool rowsUpdated = false;

                rowsUpdated = db.Update(userTeamTransferHistory, commandTimeout: 0);

                if (rowsUpdated == true)
                {
                    Logger.Out("UserTeamTransferHistory: Gameweek " + Convert.ToString(userTeamTransferHistory.@event) + " (Out:" + Convert.ToString(userTeamTransferHistory.element_out) + " In:" + Convert.ToString(userTeamTransferHistory.element_in) + ") - updated");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamTransferHistory Repository (update) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeamTransferHistory(int userTeamTransferHistoryId, SqlConnection db)
        {
            try
            {
                bool rowsDeleted = false;
                string userTeamTransferHistoryGameweek;

                var userTeamTransferHistory = db.Get<UserTeamTransferHistory>(userTeamTransferHistoryId, commandTimeout: 0);
                userTeamTransferHistoryGameweek = Convert.ToString(userTeamTransferHistory.@event);

                rowsDeleted = db.Delete(new UserTeamTransferHistory() { userteamtransferhistoryid = userTeamTransferHistoryId });

                if (rowsDeleted == true)
                {
                    Logger.Out("UserTeamTransferHistory: Gameweek " + userTeamTransferHistoryGameweek + " (" + Convert.ToString(userTeamTransferHistoryId) + ") - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamTransferHistory Repository (DeleteUserTeamTransferHistory) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeamTransferHistoryId(int userTeamTransferHistoryId, SqlConnection db)
        {
            try
            {
                string userTeamTransferHistoryGameweek;
                int rowsDeleted;

                var userTeamTransferHistory = db.Get<UserTeamTransferHistory>(userTeamTransferHistoryId, commandTimeout: 0);
                userTeamTransferHistoryGameweek = Convert.ToString(userTeamTransferHistory.@event);

                //rowsDeleted = db.Delete(new UserTeamTransferHistory() { element = userTeamTransferHistoryId });
                string deleteQuery = "DELETE FROM dbo.UserTeamTransferHistory WHERE id = @UserTeamTransferHistoryId;";
                rowsDeleted = db.Execute(deleteQuery, new { UserTeamTransferHistoryId = userTeamTransferHistoryId });

                if (rowsDeleted > 0)
                {
                    Logger.Out("UserTeamTransferHistory: Gameweek " + userTeamTransferHistoryGameweek + " (" + Convert.ToString(userTeamTransferHistoryId) + ") - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamTransferHistory Repository (DeleteUserTeamTransferHistoryId) error: " + ex.Message);
                throw ex;
            }
        }

        public List<long> GetAllUserTeamTransferHistoryIdsForUserTeamId(int userTeamId, SqlConnection db)
        {
            try
            {
                using (IDbCommand cmd = db.CreateCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetAllUserTeamTransferHistoryIdsForUserTeamId";

                    IDataParameter param = cmd.CreateParameter();
                    param.ParameterName = "@UserTeamId";
                    param.Value = userTeamId;
                    cmd.Parameters.Add(param);

                    //string selectQuery = @"SELECT userteamtransferhistoryid AS id FROM dbo.UserTeamTransferHistory WHERE UserTeamId = @UserTeamId";

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        List<long> result = ReadListLong(reader);

                        reader.Close();
                        reader.Dispose();

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamTransferHistory Repository (GetAllUserTeamTransferHistoryIds) error: " + ex.Message);
                throw ex;
            }
        }

        public List<int> GetAllUserTeamIdsWithTransferHistory(SqlConnection db)
        {
            try
            {
                using (IDbCommand cmd = db.CreateCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetAllUserTeamIdsWithTransferHistory";

                    //string selectQuery = @"SELECT DISTINCT userteamid AS id FROM dbo.UserTeamTransferHistory";

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
                Logger.Error("UserTeamTransferHistory Repository (GetAllUserTeamTransferHistoryIds) error: " + ex.Message);
                throw ex;
            }
        }

        public string userTeamTransferHistoryGameweek(int userTeamTransferHistoryId, SqlConnection db)
        {
            try
            {
                //string selectQuery = @"SELECT team_name FROM dbo.UserTeamTransferHistorys WHERE id = " + userTeamTransferHistoryId.ToString();

                var userTeamTransferHistory = db.Get<UserTeamTransferHistory>(userTeamTransferHistoryId, commandTimeout: 0);

                string userTeamTransferHistoryGameweek = Convert.ToString(userTeamTransferHistory.@event);

                return userTeamTransferHistoryGameweek;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamTransferHistory Repository (userTeamTransferHistoryGameweek) error: " + ex.Message);
                throw ex;
            }
        }

        public List<int> GetCompetedUserTeamTransferHistoryIds(SqlConnection db)
        {
            try
            {
                using (IDbCommand cmd = db.CreateCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetCompetedUserTeamTransferHistoryIds";

                    //string selectQuery = @"SELECT utth.id FROM dbo.UserTeamTransferHistory utth INNER JOIN dbo.Gameweeks g ON utth.gameweekId = g.id WHERE g.id = (SELECT TOP 1 id FROM dbo.Gameweeks WHERE deadline_time < GETDATE() ORDER BY deadline_time DESC)";

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
                Logger.Error("UserTeamTransferHistory Repository (GetCompetedUserTeamTransferHistoryIds) error: " + ex.Message);
                throw ex;
            }
        }

        public DataTable GetMaxGameweekIdFromUserTeamTransferHistoryForUserTeamIds(List<int> userTeamIds, SqlConnection db)
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
                    cmd.CommandText = "GetMaxGameweekIdForUserTeamIdsFromUserTeamTransferHistory";

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
                Logger.Error("UserTeamTransferHistory Repository (GetMaxGameweekIdFromUserTeamTransferHistoryForUserTeamIds) error: " + ex.Message);
                throw ex;
            }
        }

        //public DataTable GetMaxGameweekIdFromUserTeamTransferHistoryForUserTeamIds(List<int> userTeamIds, SqlConnection db)
        //{
        //    try
        //    {
        //        using (SqlCommand cmd = new SqlCommand())
        //        {
        //            List<string> sqlParams = new List<string>();

        //            int i = 0;
        //            foreach (var value in userTeamIds)
        //            {
        //                var name = "@p" + i++;
        //                cmd.Parameters.AddWithValue(name, value);
        //                sqlParams.Add(name);
        //            }

        //            string paramNames = string.Join(",", sqlParams);

        //            string selectQuery = @"SELECT userteamid,MAX(gameweekid) AS gameweekid FROM dbo.UserTeamTransferHistory WHERE userteamid IN (" + paramNames + ") GROUP BY userteamid;";

        //            cmd.Connection = db;
        //            cmd.CommandTimeout = 0;
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
        //        Logger.Error("UserTeamTransferHistory Repository (GetMaxGameweekIdFromUserTeamTransferHistoryForUserTeamIds) error: " + ex.Message);
        //        throw ex;
        //    }
        //}

        List<int> ReadList(IDataReader reader)
        {
            try
            {
                List<int> list = new List<int>();
                int column = reader.GetOrdinal("id");

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
                Logger.Error("UserTeamTransferHistory Repository (ReadList) error: " + ex.Message);
                throw ex;
            }
        }

        List<long> ReadListLong(IDataReader reader)
        {
            try
            {
                List<long> list = new List<long>();
                int column = reader.GetOrdinal("id");

                while (reader.Read())
                {
                    //check for the null value and than add 
                    if (!reader.IsDBNull(column))
                        list.Add(reader.GetInt64(column));
                }
                return list;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamTransferHistory Repository (ReadListLong) error: " + ex.Message);
                throw ex;
            }
        }
    }
}