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
    public class UserTeamSeasonRepository : IUserTeamSeason
    {
        public int InsertUserTeamSeason(UserTeamSeasons userTeamSeasons, SqlConnection db)
        {
            try
            {
                int rowsAffected = 0;

                using (IDataReader reader = userTeamSeasons.GetDataReader())
                {
                    using (var bulkCopy = new SqlBulkCopy(db))
                    {
                        bulkCopy.BulkCopyTimeout = 1000;
                        bulkCopy.BatchSize = 500;
                        bulkCopy.DestinationTableName = "UserTeamSeason";
                        bulkCopy.EnableStreaming = true;

                        // Add your column mappings here
                        bulkCopy.ColumnMappings.Add("season_name", "season_name");
                        bulkCopy.ColumnMappings.Add("total_points", "total_points");
                        bulkCopy.ColumnMappings.Add("rank", "userteam_rank");
                        bulkCopy.ColumnMappings.Add("userteamid", "userteamid");

                        //using (var dataReader = userTeamSeasons.ToDataReader())
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
                Logger.Error("UserTeamSeason Repository (insert) error: " + ex.Message);
                throw ex;
            }
        }

        //public bool InsertUserTeamSeason(UserTeamSeasons userTeamSeasonsInsert, SqlConnection db)
        //{
        //    try
        //    {
        //        long rowsAffected = 0;

        //        //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
        //        //{
        //            rowsAffected = db.Insert(userTeamSeasonsInsert, commandTimeout: 300);
        //        //}

        //        //if (rowsAffected > 0)
        //        //{
        //        //    Logger.Out("UserTeamSeason: " + userTeamSeason.season_name + " - inserted");
        //        //    return true;
        //        //}
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("UserTeamSeason Repository (insert) error: " + ex.Message);
        //        throw ex;
        //    }
        //}

        public bool UpdateUserTeamSeason(UserTeamSeason userTeamSeason, SqlConnection db)
        {
            try
            {
                bool rowsUpdated = false;

                rowsUpdated = db.Update(userTeamSeason, commandTimeout: 300);

                if (rowsUpdated == true)
                {
                    Logger.Out("UserTeamSeason: " + userTeamSeason.season_name + " - updated");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamSeason Repository (update) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeamSeason(int userTeamId, SqlConnection db)
        {
            try
            {
                bool rowsDeleted = false;
                var userTeamSeason = new UserTeamSeason();

                userTeamSeason = db.Get<UserTeamSeason>(userTeamId);
                rowsDeleted = db.Delete(new UserTeamSeason() { userteamid = userTeamId });

                if (rowsDeleted == true)
                {
                    Logger.Out("UserTeamSeason: " + userTeamSeason.season_name + " (" + Convert.ToString(userTeamId) + ") - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamSeason Repository (DeleteUserTeamSeason) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeamSeasonId(int userTeamid, int userTeamSeasonid, SqlConnection db)
        {
            try
            {
                string userTeamSeasonName;
                int rowsDeleted;

                var userTeamSeason = db.Get<UserTeamSeason>(userTeamSeasonid);
                userTeamSeasonName = userTeamSeason.season_name;

                string deleteQuery = "DELETE FROM dbo.UserTeamSeason WHERE id = @UserTeamSeasonId;";
                rowsDeleted = db.Execute(deleteQuery, new { UserTeamSeasonId = userTeamSeasonid });

                if (rowsDeleted > 0)
                {
                    Logger.Out("UserTeamSeason: " + userTeamSeasonName + " (" + Convert.ToString(userTeamSeasonid) + ") - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamSeason Repository (DeleteUserTeamSeasonId) error: " + ex.Message);
                throw ex;
            }
        }

        public List<int> GetAllUserTeamSeasonIdsForUserTeamId(int userTeamId, SqlConnection db)
        {
            try
            {
                string selectQuery = @"SELECT uts.id FROM dbo.UserTeamSeason uts INNER JOIN dbo.UserTeam ut ON uts.userteamid = ut.id WHERE ut.id = @UserTeamId;";

                IDataReader reader = db.ExecuteReader(selectQuery, new { UserTeamId = userTeamId }, commandTimeout: 300);

                List<int> result = ReadList(reader);

                reader.Close();

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamSeason Repository (GetAllUserTeamSeasonIds) error: " + ex.Message);
                throw ex;
            }
        }

        public List<string> GetAllUserTeamSeasonNamesForUserTeamId(int userTeamId, SqlConnection db)
        {
            try
            {
                string selectQuery = @"SELECT uts.season_name AS name FROM dbo.UserTeamSeason uts INNER JOIN dbo.UserTeam ut ON uts.userteamid = ut.id WHERE ut.id = @UserTeamId;";

                IDataReader reader = db.ExecuteReader(selectQuery, new { UserTeamId = userTeamId }, commandTimeout: 300);

                List<string> result = ReadListString(reader);

                reader.Close();

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamSeason Repository (GetAllUserTeamSeasonIds) error: " + ex.Message);
                throw ex;
            }
        }

        public string GetUserTeamSeasonName(int userTeamSeasonId, SqlConnection db)
        {
            try
            {
                var userTeamSeason = db.Get<UserTeamSeason>(userTeamSeasonId, commandTimeout: 300);

                string userTeamSeasonName = userTeamSeason.season_name;

                return userTeamSeasonName;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamSeason Repository (GetUserTeamSeasonName) error: " + ex.Message);
                throw ex;
            }
        }

        //public List<int> GetCompetedUserTeamSeasonIds(SqlConnection db)
        //{
        //    //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
        //    //{
        //        string selectQuery = @"SELECT utc.id FROM dbo.UserTeamSeason utc INNER JOIN dbo.Gameweeks g ON h.gameweekId = g.id WHERE g.id = (SELECT TOP 1 id FROM dbo.Gameweeks WHERE deadline_time < GETDATE() ORDER BY deadline_time DESC)";

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
                Logger.Error("UserTeamSeason Repository (ReadList) error: " + ex.Message);
                throw ex;
            }
        }

        List<string> ReadListString(IDataReader reader)
        {
            try
            {
                List<string> list = new List<string>();
                int column = reader.GetOrdinal("name");

                while (reader.Read())
                {
                    //check for the null value and than add 
                    if (!reader.IsDBNull(column))
                        list.Add(reader.GetString(column));
                }
                return list;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamSeason Repository (ReadListString) error: " + ex.Message);
                throw ex;
            }
        }
    }
}