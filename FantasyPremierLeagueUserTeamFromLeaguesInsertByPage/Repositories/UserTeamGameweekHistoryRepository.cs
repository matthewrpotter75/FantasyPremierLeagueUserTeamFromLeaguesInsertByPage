using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Dapper;
using DapperExtensions;
using System.Linq;
using DataStreams.ETL;

namespace FantasyPremierLeagueUserTeams
{
    public class UserTeamGameweekHistoryRepository : IUserTeamGameweekHistory
    {
        public int InsertUserTeamGameweekHistories(UserTeamGameweekHistories userTeamGameweekHistories, SqlConnection db)
        {
            int rowsAffected = 0;

            try
            {
                using (IDataReader reader = userTeamGameweekHistories.GetDataReader())
                {
                    using (var bulkCopy = new SqlBulkCopy(db))
                    {
                        bulkCopy.BulkCopyTimeout = 0;
                        bulkCopy.BatchSize = 1000;
                        bulkCopy.DestinationTableName = "UserTeamGameweekHistoryStaging";
                        bulkCopy.EnableStreaming = true;

                        // Add your column mappings here
                        //bulkCopy.ColumnMappings.Add("id", "id");
                        bulkCopy.ColumnMappings.Add("event", "gameweekid");
                        bulkCopy.ColumnMappings.Add("points", "points");
                        bulkCopy.ColumnMappings.Add("total_points", "total_points");
                        bulkCopy.ColumnMappings.Add("rank", "userteam_rank");
                        bulkCopy.ColumnMappings.Add("rank_sort", "userteam_rank_sort");
                        bulkCopy.ColumnMappings.Add("overall_rank", "userteam_overall_rank");
                        bulkCopy.ColumnMappings.Add("bank", "userteam_bank");
                        bulkCopy.ColumnMappings.Add("value", "userteam_value");
                        bulkCopy.ColumnMappings.Add("event_transfers", "userteam_gameweek_transfers");
                        bulkCopy.ColumnMappings.Add("event_transfers_cost", "userteam_gameweek_transfers_cost");
                        bulkCopy.ColumnMappings.Add("points_on_bench", "points_on_bench");
                        bulkCopy.ColumnMappings.Add("userteamid", "userteamid");

                        bulkCopy.WriteToServer(reader);
                        rowsAffected = SqlBulkCopyExtension.RowsCopiedCount(bulkCopy);
                    }
                }
                return rowsAffected;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamGameweekHistory Repository (insert) error: " + ex.Message);
                return rowsAffected;
                //throw ex;
            }
        }

        public bool UpdateUserTeamGameweekHistory(UserTeamGameweekHistory userTeamGameweekHistory, SqlConnection db)
        {
            try
            {
                bool rowsUpdated = false;

                rowsUpdated = db.Update(userTeamGameweekHistory, commandTimeout: 0);

                if (rowsUpdated == true)
                {
                    Logger.Out("UserTeamGameweekHistory: Gameweek " + Convert.ToString(userTeamGameweekHistory.@event) + " (" + Convert.ToString(userTeamGameweekHistory.id) + ") - updated");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamGameweekHistory Repository (update) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeamGameweekHistory(int userTeamGameweekHistoryId, SqlConnection db)
        {
            try
            {
                bool rowsDeleted = false;
                string userTeamGameweekHistoryGameweek;

                var userTeamGameweekHistory = db.Get<UserTeamGameweekHistory>(userTeamGameweekHistoryId);
                userTeamGameweekHistoryGameweek = Convert.ToString(userTeamGameweekHistory.@event);

                rowsDeleted = db.Delete(new UserTeamGameweekHistory() { id = userTeamGameweekHistoryId });

                if (rowsDeleted == true)
                {
                    Logger.Out("UserTeamGameweekHistory: Gameweek " + userTeamGameweekHistoryGameweek + " (" + Convert.ToString(userTeamGameweekHistoryId) + ") - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamGameweekHistory Repository (DeleteUserTeamGameweekHistory) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeamGameweekHistoryId(int userTeamGameweekHistoryId, SqlConnection db)
        {
            try
            {
                string userTeamGameweekHistoryGameweek;
                int rowsDeleted;

                var userTeamGameweekHistory = db.Get<UserTeamGameweekHistory>(userTeamGameweekHistoryId);
                userTeamGameweekHistoryGameweek = Convert.ToString(userTeamGameweekHistory.@event);

                //rowsDeleted = db.Delete(new UserTeamGameweekHistory() { element = userTeamGameweekHistoryId });
                string deleteQuery = "DELETE FROM dbo.UserTeamGameweekHistory WHERE id = @UserTeamGameweekHistoryId;";
                rowsDeleted = db.Execute(deleteQuery, new { UserTeamGameweekHistoryId = userTeamGameweekHistoryId });

                if (rowsDeleted > 0)
                {
                    Logger.Out("UserTeamGameweekHistory: Gameweek " + userTeamGameweekHistoryGameweek + " (" + Convert.ToString(userTeamGameweekHistoryId) + ") - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamGameweekHistory Repository (DeleteUserTeamGameweekHistoryId) error: " + ex.Message);
                throw ex;
            }
        }

        public List<int> GetAllUserTeamGameweekHistoryIdsForUserTeamId(int userTeamId, SqlConnection db)
        {
            try
            {
                using (IDbCommand cmd = db.CreateCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandTimeout = 300;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetAllUserTeamGameweekHistoryIdsForUserTeamId";

                    IDataParameter param = cmd.CreateParameter();
                    param.ParameterName = "@UserTeamId";
                    param.Value = userTeamId;
                    cmd.Parameters.Add(param);

                    //string selectQuery = @"SELECT gameweekid AS id FROM dbo.UserTeamGameweekHistory WHERE userteamid = @UserTeamId";

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
                Logger.Error("UserTeamGameweekHistory Repository (GetAllUserTeamGameweekHistoryIdsForUserTeamId) error: " + ex.Message);
                throw ex;
            }
        }

        public DataTable GetMaxGameweekIdFromUserTeamGameweekHistoryForUserTeamIds(List<int> userTeamIds, SqlConnection db)
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
                    cmd.CommandTimeout = 300;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetMaxGameweekIdForUserTeamIdsFromUserTeamGameweekHistory";

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
                Logger.Error("UserTeamGameweekHistory Repository (GetMaxGameweekIdFromUserTeamGameweekHistoryForUserTeamIds) error: " + ex.Message);
                throw ex;
            }
        }

        //public DataTable GetMaxGameweekIdFromUserTeamGameweekHistoryForUserTeamIds(List<int> userTeamIds, SqlConnection db)
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

        //            string selectQuery = @"SELECT userteamid,MAX(gameweekid) AS gameweekid FROM dbo.UserTeamGameweekHistory WHERE userteamid IN (" + paramNames + ") GROUP BY userteamid;";

        //            cmd.Connection = db;
        //            cmd.CommandTimeout = 300;
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
        //        Logger.Error("UserTeamGameweekHistory Repository (GetMaxGameweekIdFromUserTeamGameweekHistoryForUserTeamIds) error: " + ex.Message);
        //        throw ex;
        //    }
        //}

        public string UserTeamGameweekHistoryGameweek(int userTeamGameweekHistoryId, SqlConnection db)
        {
            try
            {
                //string selectQuery = @"SELECT team_name FROM dbo.UserTeamGameweekHistorys WHERE id = " + userTeamGameweekHistoryId.ToString();

                var userTeamGameweekHistory = db.Get<UserTeamGameweekHistory>(userTeamGameweekHistoryId, commandTimeout: 300);

                string userTeamGameweekHistoryGameweek = Convert.ToString(userTeamGameweekHistory.@event);

                return userTeamGameweekHistoryGameweek;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamGameweekHistory Repository (userTeamGameweekHistoryGameweek) error: " + ex.Message);
                throw ex;
            }
        }

        public List<int> GetCompletedUserTeamGameweekHistoryIds(SqlConnection db)
        {
            try
            {
                using (IDbCommand cmd = db.CreateCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandTimeout = 300;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetCompletedUserTeamGameweekHistoryIds";

                    //string selectQuery = @"SELECT utgh.id FROM dbo.UserTeamGameweekHistory utgh INNER JOIN dbo.Gameweeks g ON utgh.gameweekId = g.id WHERE g.id = (SELECT TOP 1 id FROM dbo.Gameweeks WHERE deadline_time < GETDATE() ORDER BY deadline_time DESC)";

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
                Logger.Error("UserTeamGameweekHistory Repository (GetCompletedUserTeamGameweekHistoryIds) error: " + ex.Message);
                throw ex;
            }
        }

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
                Logger.Error("UserTeamGameweekHistory Repository (ReadList) error: " + ex.Message);
                throw ex;
            }
        }
    }
}