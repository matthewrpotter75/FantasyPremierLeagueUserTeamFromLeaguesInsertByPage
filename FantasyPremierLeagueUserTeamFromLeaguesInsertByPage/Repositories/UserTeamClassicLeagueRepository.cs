using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using DapperExtensions;
using DataStreams.ETL;

namespace FantasyPremierLeagueUserTeams
{
    public class UserTeamClassicLeagueRepository : IUserTeamClassicLeague
    {
        public int InsertUserTeamClassicLeague(UserTeamClassicLeagues classicleagues, SqlConnection db)
        {
            int rowsAffected = 0;

            try
            {
                using (IDataReader reader = classicleagues.GetDataReader())
                {
                    using (var bulkCopy = new SqlBulkCopy(db))
                    {
                        bulkCopy.BulkCopyTimeout = 1000;
                        bulkCopy.BatchSize = 500;
                        bulkCopy.DestinationTableName = "UserTeamClassicLeagueStaging";
                        bulkCopy.EnableStreaming = true;

                        // Add your column mappings here
                        bulkCopy.ColumnMappings.Add("id", "leagueid");
                        bulkCopy.ColumnMappings.Add("entry_rank", "entry_rank");
                        bulkCopy.ColumnMappings.Add("entry_last_rank", "entry_last_rank");
                        bulkCopy.ColumnMappings.Add("entry_can_leave", "entry_can_leave");
                        bulkCopy.ColumnMappings.Add("entry_can_admin", "entry_can_admin");
                        bulkCopy.ColumnMappings.Add("entry_can_invite", "entry_can_invite");
                        bulkCopy.ColumnMappings.Add("userteamid", "userteamid");

                        bulkCopy.WriteToServer(reader);
                        rowsAffected = SqlBulkCopyExtension.RowsCopiedCount(bulkCopy);
                    }
                }

                return rowsAffected;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamClassicLeague Repository (insert) error: " + ex.Message);
                return rowsAffected;
                //throw ex;
            }
        }

        //public bool InsertUserTeamClassicLeague(UserTeamClassicLeague classicleague, SqlConnection db)
        //{
        //    try
        //    {
        //        long rowsAffected = 0;

        //        //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
        //        //{
        //            rowsAffected = db.Insert(classicleague, commandTimeout: 300);
        //        //}

        //        if (rowsAffected > 0)
        //        {
        //            Logger.Out("ClassicLeague: " + classicleague.name + " - inserted");
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("ClassicLeague Repository (insert) error: " + ex.Message);
        //        throw ex;
        //    }
        //}

        public bool UpdateUserTeamClassicLeague(UserTeamClassicLeague classicleague, SqlConnection db)
        {
            try
            {
                bool rowsUpdated = false;

                rowsUpdated = db.Update(classicleague, commandTimeout: 300);

                if (rowsUpdated == true)
                {
                    Logger.Out("ClassicLeague: " + classicleague.name + " - updated");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamClassicLeague Repository (update) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeamClassicLeague(int userTeamid, int classicleagueid, SqlConnection db)
        {
            try
            {
                bool rowsDeleted = false;
                var classicleague = new UserTeamClassicLeague();

                classicleague = db.Get<UserTeamClassicLeague>(classicleagueid);
                rowsDeleted = db.Delete(new UserTeamClassicLeague() { userteamid = userTeamid, id = classicleagueid });

                if (rowsDeleted == true)
                {
                    Logger.Out("ClassicLeague: " + classicleague.name + " (" + Convert.ToString(classicleagueid) + ") - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamClassicLeague Repository (DeleteClassicLeague) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteClassicLeagueId(int userTeamid, int classicleagueid, SqlConnection db)
        {
            try
            {
                string classicleagueName;
                int rowsDeleted;

                var classicleague = db.Get<UserTeamClassicLeague>(classicleagueid);
                classicleagueName = classicleague.name;

                string deleteQuery = "DELETE FROM dbo.UserTeamClassicLeague WHERE userTeamid = @UserTeamId AND id = @ClassicLeagueId;";
                rowsDeleted = db.Execute(deleteQuery, new { UserTeamId = userTeamid, ClassicLeagueId = classicleagueid });

                if (rowsDeleted > 0)
                {
                    Logger.Out("ClassicLeague: " + classicleagueName + " (" + Convert.ToString(classicleagueid) + ") - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamClassicLeague Repository (DeleteClassicLeagueId) error: " + ex.Message);
                throw ex;
            }
        }

        public List<int> GetAllClassicLeagueIdsForUserTeamId(int userTeamId, SqlConnection db)
        {
            try
            {
                using (IDbCommand cmd = db.CreateCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandTimeout = 300;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetAllClassicLeagueIdsForUserTeamId";

                    IDataParameter param = cmd.CreateParameter();
                    param.ParameterName = "@UserTeamId";
                    param.Value = userTeamId;
                    cmd.Parameters.Add(param);

                    //string selectQuery = @"SELECT leagueid AS id FROM dbo.UserTeamClassicLeague WHERE UserTeamId = @UserTeamId;";

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        List<int> result = ReadList(reader);

                        reader.Close();

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamClassicLeague Repository (GetAllClassicLeagueIds) error: " + ex.Message);
                throw ex;
            }
        }

        public string GetClassicLeagueName(int classicleagueId, SqlConnection db)
        {
            try
            { 
                var classicleague = db.Get<UserTeamClassicLeague>(classicleagueId, commandTimeout: 300);

                string classicleagueName = classicleague.name;

                return classicleagueName;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamClassicLeague Repository (GetClassicLeagueName) error: " + ex.Message);
                throw ex;
            }
        }

        //public List<int> GetCompetedClassicLeagueIds(SqlConnection db)
        //{
        //    try
        //    {
        //        string selectQuery = @"SELECT c.id FROM dbo.UserTeamClassicLeague c INNER JOIN dbo.Gameweeks g ON c.gameweekId = g.id WHERE g.id = (SELECT TOP 1 id FROM dbo.Gameweeks WHERE deadline_time < GETDATE() ORDER BY deadline_time DESC)";

        //        IDataReader reader = db.ExecuteReader(selectQuery);

        //        List<int> result = ReadList(reader);

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("UserTeamClassicLeague Repository (GetCompletedClassicLeagueIds) error: " + ex.Message);
        //        throw ex;
        //    }
        //}

        public DataTable GetClassicLeagueCountFromUserTeamClassicLeagueForUserTeamIds(List<int> userTeamIds, SqlConnection db)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    List<string> sqlParams = new List<string>();

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
                    cmd.CommandText = "GetClassicLeagueCountFromUserTeamClassicLeagueForUserTeamIds";

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
                Logger.Error("UserTeamClassicLeague Repository (GetClassicLeagueCountFromUserTeamClassicLeagueForUserTeamIds) error: " + ex.Message);
                throw ex;
            }
        }

        //public DataTable GetClassicLeagueCountFromUserTeamClassicLeagueForUserTeamIds(List<int> userTeamIds, SqlConnection db)
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

        //            string selectQuery = @"SELECT userteamid,COUNT(*) AS leagueCount FROM dbo.UserTeamClassicLeague WHERE userteamid IN (" + paramNames + ") GROUP BY userteamid;";

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
        //        Logger.Error("UserTeamClassicLeague Repository (GetClassicLeagueCountFromUserTeamClassicLeagueForUserTeamIds) error: " + ex.Message);
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
                Logger.Error("UserTeamClassicLeague Repository (ReadList) error: " + ex.Message);
                throw ex;
            }
        }
    }
}