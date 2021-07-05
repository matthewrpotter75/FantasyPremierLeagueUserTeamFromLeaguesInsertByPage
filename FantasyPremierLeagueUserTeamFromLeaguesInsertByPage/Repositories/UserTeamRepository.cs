using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using DapperExtensions;
using DataStreams.ETL;

namespace FantasyPremierLeagueUserTeams
{
    public class UserTeamRepository : IUserTeam
    {
        public bool InsertUserTeam(UserTeam userTeam, SqlConnection db)
        {
            try
            {
                long rowsAffected = 0;

                rowsAffected = db.Insert(userTeam, commandTimeout: 0);

                if (rowsAffected > 0)
                {
                    Logger.Out("UserTeam: " + userTeam.name + " (" + Convert.ToString(userTeam.id) + ") - inserted");
                    //Logger.Out("");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeam Repository (insert) error: " + ex.Message);
                throw ex;
            }
        }

        public int UpdateUserTeam(UserTeams userTeams, SqlConnection db)
        {
            int rowsAffected = 0;

            try
            {
                using (IDataReader reader = userTeams.GetDataReader())
                {
                    using (var bulkCopy = new SqlBulkCopy(db))
                    {
                        bulkCopy.BulkCopyTimeout = 0;
                        bulkCopy.BatchSize = 1000;
                        bulkCopy.DestinationTableName = "UserTeamUpdateStaging";
                        bulkCopy.EnableStreaming = true;

                        // Add your column mappings here
                        bulkCopy.ColumnMappings.Add("id", "id");
                        bulkCopy.ColumnMappings.Add("player_first_name", "player_first_name");
                        bulkCopy.ColumnMappings.Add("player_last_name", "player_last_name");
                        bulkCopy.ColumnMappings.Add("player_region_id", "player_region_id");
                        bulkCopy.ColumnMappings.Add("player_region_name", "player_region_name");
                        bulkCopy.ColumnMappings.Add("player_region_iso_code_long", "player_region_iso_code");
                        bulkCopy.ColumnMappings.Add("summary_overall_points", "summary_overall_points");
                        bulkCopy.ColumnMappings.Add("summary_overall_rank", "summary_overall_rank");
                        bulkCopy.ColumnMappings.Add("summary_event_points", "summary_gameweek_points");
                        bulkCopy.ColumnMappings.Add("summary_event_rank", "summary_gameweek_rank");
                        bulkCopy.ColumnMappings.Add("current_event", "current_gameweekId");
                        bulkCopy.ColumnMappings.Add("joined_time", "joined_time");
                        bulkCopy.ColumnMappings.Add("name", "team_name");
                        bulkCopy.ColumnMappings.Add("last_deadline_bank", "team_bank");
                        bulkCopy.ColumnMappings.Add("last_deadline_value", "team_value");
                        bulkCopy.ColumnMappings.Add("last_deadline_total_transfers", "team_transfers");
                        bulkCopy.ColumnMappings.Add("kit", "kit");
                        bulkCopy.ColumnMappings.Add("favourite_team", "favourite_teamid");
                        bulkCopy.ColumnMappings.Add("started_event", "started_gameweekid");

                        bulkCopy.WriteToServer(reader);
                        rowsAffected = SqlBulkCopyExtension.RowsCopiedCount(bulkCopy);
                    }
                }
                return rowsAffected;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeam Repository (update) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeam(int userTeamId, SqlConnection db)
        {
            try
            {
                bool rowsDeleted = false;
                string userTeamName;

                var userTeam = db.Get<UserTeam>(userTeamId);
                userTeamName = userTeam.name;

                rowsDeleted = db.Delete(new UserTeam() { id = userTeamId });

                if (rowsDeleted == true)
                {
                    Logger.Out("UserTeam: " + userTeamName + " (" + Convert.ToString(userTeamId) + ") - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeam Repository (DeleteUserTeam) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeamId(int userTeamId, SqlConnection db)
        {
            try
            {
                string userTeamName;
                int rowsDeleted;

                var userTeam = db.Get<UserTeam>(userTeamId);
                userTeamName = userTeam.name;

                //rowsDeleted = db.Delete(new UserTeam() { element = userTeamId });
                string deleteQuery = "DELETE FROM dbo.UserTeam WHERE userTeamId = @UserTeamId;";
                rowsDeleted = db.Execute(deleteQuery, new { UserTeamId = userTeamId });

                if (rowsDeleted > 0)
                {
                    Logger.Out("UserTeam: " + userTeamName + " (" + Convert.ToString(userTeamId) + ") - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeam Repository (DeleteUserTeamId) error: " + ex.Message);
                throw ex;
            }
        }

        public List<int> GetAllUserTeamIds(SqlConnection db)
        {
            try
            {
                using (IDbCommand cmd = db.CreateCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetAllUserTeams";

                    //string selectQuery = @"SELECT id FROM dbo.UserTeam;";

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
                Logger.Error("UserTeam Repository (GetAllUserTeamIds) error: " + ex.Message);
                throw ex;
            }
        }

        public int UpdateUserTeamSQL(SqlConnection db)
        {
            try
            {
                using (IDbCommand cmd = db.CreateCommand())
                {
                    int result = 0;

                    cmd.Connection = db;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "UpdateUserTeam";

                    result = cmd.ExecuteNonQuery();

                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeam Repository (UpdateUserTeamSQL) error: " + ex.Message);
                throw ex;
            }
        }

        public int TruncateUserTeamUpdateStaging(SqlConnection db)
        {
            try
            {
                using (IDbCommand cmd = db.CreateCommand())
                {
                    int result = 0;
                    
                    cmd.Connection = db;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "TruncateUserTeamUpdateStaging";

                    result = cmd.ExecuteNonQuery();

                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeam Repository (TrncateUserTeamUpdateStaging) error: " + ex.Message);
                throw ex;
            }
        }

        public string GetUserTeamName(int userTeamId, SqlConnection db)
        {
            try
            {
                var userTeam = db.Get<UserTeam>(userTeamId, commandTimeout: 0);

                string userTeamName = userTeam.name;

                return userTeamName;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeam Repository (GetAllUserTeamIds) error: " + ex.Message);
                throw ex;
            }
        }

        public DataTable GetUserTeamForUserTeamIds(List<int> userTeamIds, SqlConnection db)
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
                    cmd.CommandText = "GetUserTeamIdForUserTeamIdsFromUserTeam";

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
                Logger.Error("UserTeam Repository (GetUserTeamForUserTeamIds) error: " + ex.Message);
                throw ex;
            }
        }

        //public DataTable GetUserTeamForUserTeamIds(List<int> userTeamIds, SqlConnection db)
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

        //            string selectQuery = @"SELECT id AS userteamid FROM dbo.UserTeam WHERE id IN (" + paramNames + ");";

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
        //        Logger.Error("UserTeam Repository (GetUserTeamForUserTeamIds) error: " + ex.Message);
        //        throw ex;
        //    }
        //}

        public List<int> GetCompetedUserTeamIds(SqlConnection db)
        {
            try
            {
                using (IDbCommand cmd = db.CreateCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetCompetedUserTeamIds";

                    //string selectQuery = @"SELECT ut.id FROM dbo.UserTeam ut INNER JOIN dbo.Gameweeks g ON ut.current_gameweekId = g.id WHERE g.id = (SELECT TOP 1 id FROM dbo.Gameweeks WHERE deadline_time < GETDATE() ORDER BY deadline_time DESC)";

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
                Logger.Error("UserTeam Repository (GetCompetedUserTeamIds) error: " + ex.Message);
                throw ex;
            }
        }

        public int GetLatestGameweekId(SqlConnection db, string entity)
        {
            try
            {
                //string selectQuery = "";

                //if (entity == "UserTeamGameweekHistory")
                //{
                //    selectQuery = @"SELECT MAX(gameweekId) FROM (SELECT gwf.gameweekId, MAX(gwf.kickoff_time) AS MaxKickoffTime FROM dbo.Gameweeks gw INNER JOIN dbo.GameweekFixtures gwf ON gw.id = gwf.gameweekId GROUP BY gwf.gameweekId ) t WHERE DATEADD(hh,9,CAST(CAST(DATEADD(day,1,MaxKickoffTime) AS DATE) AS DATETIME)) < GETDATE();";
                //}
                //else
                //{
                //    selectQuery = @"SELECT MAX(id) AS id FROM dbo.Gameweeks WHERE deadline_time < GETDATE();";
                //}

                using (IDbCommand cmd = db.CreateCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (entity == "UserTeamGameweekHistory")
                    {
                        cmd.CommandText = "GetLatestGameweekId";
                    }
                    else
                    {
                        cmd.CommandText = "GetActualGameweekId";
                    }

                    int result = Convert.ToInt32(cmd.ExecuteScalar());

                    //var result = db.ExecuteScalar<int>(selectQuery, commandTimeout: 0);

                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeam Repository (GetLatestGameweekId) error: " + ex.Message);
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
                Logger.Error("UserTeam Repository (ReadList) error: " + ex.Message);
                throw ex;
            }
        }
    }
}