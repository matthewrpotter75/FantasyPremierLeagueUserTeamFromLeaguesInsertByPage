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
    public class UserTeamPickAutomaticSubRepository : IUserTeamPickAutomaticSub
    {
        public int InsertUserTeamPickAutomaticSubs(UserTeamPickAutomaticSubs userTeamPickAutomaticSubs, SqlConnection db)
        {
            try
            {
                int rowsAffected = 0;

                using (IDataReader reader = userTeamPickAutomaticSubs.GetDataReader())
                {
                    using (var bulkCopy = new SqlBulkCopy(db))
                    {
                        bulkCopy.BulkCopyTimeout = 1000;
                        bulkCopy.BatchSize = 500;
                        bulkCopy.DestinationTableName = "UserTeamPickAutomaticSub";
                        bulkCopy.EnableStreaming = true;

                        // Add your column mappings here
                        bulkCopy.ColumnMappings.Add("id", "id");
                        bulkCopy.ColumnMappings.Add("element_in", "playerid_in");
                        bulkCopy.ColumnMappings.Add("element_out", "playerid_out");
                        bulkCopy.ColumnMappings.Add("entry", "userteamid");
                        bulkCopy.ColumnMappings.Add("event", "gameweekid");

                        bulkCopy.WriteToServer(reader);
                        rowsAffected = SqlBulkCopyExtension.RowsCopiedCount(bulkCopy);
                    }
                }

                return rowsAffected;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamPickAutomaticSub Repository (insert) error: " + ex.Message);
                throw ex;
            }
        }

        public bool UpdateUserTeamPickAutomaticSub(UserTeamPickAutomaticSub userTeamPickAutomaticSub, SqlConnection db)
        {
            try
            {
                bool rowsUpdated = false;

                rowsUpdated = db.Update(userTeamPickAutomaticSub, commandTimeout: 300);

                if (rowsUpdated == true)
                {
                    Logger.Out("UserTeamPickAutomaticSubId: " + Convert.ToString(userTeamPickAutomaticSub.id) + " - updated");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamPickAutomaticSub Repository (update) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeamPickAutomaticSub(int userTeamPickAutomaticSubid, SqlConnection db)
        {
            try
            {
                bool rowsDeleted = false;
                var userTeamPickAutomaticSub = new UserTeamPickAutomaticSub();

                userTeamPickAutomaticSub = db.Get<UserTeamPickAutomaticSub>(userTeamPickAutomaticSubid);
                rowsDeleted = db.Delete(new UserTeamPickAutomaticSub() { id = userTeamPickAutomaticSubid });

                if (rowsDeleted == true)
                {
                    Logger.Out("UserTeamPickAutomaticSub: " + Convert.ToString(userTeamPickAutomaticSubid) + " - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamPickAutomaticSub Repository (DeleteUserTeamPickAutomaticSub) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeamPickAutomaticSubId(int userTeamid, int userTeamPickAutomaticSubid, SqlConnection db)
        {
            try
            {
                int rowsDeleted;

                var userTeamPickAutomaticSub = db.Get<UserTeamPickAutomaticSub>(userTeamPickAutomaticSubid);

                string deleteQuery = "DELETE FROM dbo.UserTeamPickAutomaticSub WHERE id = @UserTeamPickAutomaticSubId;";
                rowsDeleted = db.Execute(deleteQuery, new { UserTeamPickAutomaticSubId = userTeamPickAutomaticSubid });

                if (rowsDeleted > 0)
                {
                    Logger.Out("UserTeamPickAutomaticSub: " + Convert.ToString(userTeamPickAutomaticSubid) + " - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamPickAutomaticSub Repository (DeleteUserTeamPickAutomaticSubId) error: " + ex.Message);
                throw ex;
            }
        }

        public List<int> GetAllUserTeamPickAutomaticSubIdsForUserTeamIdAndGameweekId(int userTeamId, int gameweekId, SqlConnection db)
        {
            try
            {
                string selectQuery = @"SELECT playerid_in AS id FROM dbo.UserTeamPickAutomaticSub WHERE userteamid = @UserTeamId AND gameweekid = @GameweekId;";

                IDataReader reader = db.ExecuteReader(selectQuery, new { UserTeamId = userTeamId, GameweekId = gameweekId }, commandTimeout: 300);

                List<int> result = ReadList(reader);

                reader.Close();

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamPickAutomaticSub Repository (GetAllUserTeamPickAutomaticSubIds) error: " + ex.Message);
                throw ex;
            }
        }

        //public string GetUserTeamPickAutomaticSubName(int userTeamPickAutomaticSubId, SqlConnection db)
        //{
        //    //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
        //    //{
        //        var userTeamPickAutomaticSub = db.Get<UserTeamPickAutomaticSub>(userTeamPickAutomaticSubId);

        //        string userTeamPickAutomaticSubName = userTeamPickAutomaticSub.season_name;

        //        return userTeamPickAutomaticSubName;
        //    //}
        //}

        //public List<int> GetCompetedUserTeamPickAutomaticSubIds(SqlConnection db)
        //{
        //    //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
        //    //{
        //        string selectQuery = @"SELECT utc.id FROM dbo.UserTeamPickAutomaticSub utc INNER JOIN dbo.Gameweeks g ON h.gameweekId = g.id WHERE g.id = (SELECT TOP 1 id FROM dbo.Gameweeks WHERE deadline_time < GETDATE() ORDER BY deadline_time DESC)";

        //        IDataReader reader = db.ExecuteReader(selectQuery);

        //        List<int> result = ReadList(reader);

        //        reader.Close();

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
                Logger.Error("UserTeamPickAutomaticSub Repository (ReadList) error: " + ex.Message);
                throw ex;
            }
        }
    }
}