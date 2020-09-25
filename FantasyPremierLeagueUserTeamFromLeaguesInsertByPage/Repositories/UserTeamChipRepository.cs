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
    public class UserTeamChipRepository : IUserTeamChip
    {
        public int InsertUserTeamChip(UserTeamChips userTeamChips, SqlConnection db)
        {
            try
            {
                int rowsAffected = 0;

                using (IDataReader reader = userTeamChips.GetDataReader())
                {
                    using (var bulkCopy = new SqlBulkCopy(db))
                    {
                        bulkCopy.BulkCopyTimeout = 1000;
                        bulkCopy.BatchSize = 500;
                        bulkCopy.DestinationTableName = "UserTeamChip";
                        bulkCopy.EnableStreaming = true;

                        // Add your column mappings here
                        //bulkCopy.ColumnMappings.Add("name", "chip_name");
                        bulkCopy.ColumnMappings.Add("time", "chip_time");
                        bulkCopy.ColumnMappings.Add("userteamid", "userteamid");
                        bulkCopy.ColumnMappings.Add("event", "gameweekid");
                        bulkCopy.ColumnMappings.Add("chip", "chipid");

                        bulkCopy.WriteToServer(reader);
                        rowsAffected = SqlBulkCopyExtension.RowsCopiedCount(bulkCopy);
                    }
                }
                return rowsAffected;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamChip Repository (insert) error: " + ex.Message);
                throw ex;
            }
        }

        //public class UserTeamChipRepository : IUserTeamChip
        //{
        //    public bool InsertUserTeamChip(UserTeamChip userTeamChip)
        //    {
        //        try
        //        {
        //            int rowsAffected = 0;

        //            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
        //            {
        //                rowsAffected = (int)db.Insert(userTeamChip);
        //            }

        //            if (rowsAffected > 0)
        //            {
        //                Logger.Out("UserTeamChip: " + userTeamChip.name + " - inserted");
        //                return true;
        //            }
        //            return false;
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.Error("UserTeamChip Repository (insert) error: " + ex.Message);
        //            Logger.Error("UserTeamChip Repository (insert) error: " + ex.InnerException);
        //            throw ex;
        //        }
        //    }

        public bool UpdateUserTeamChip(UserTeamChip userTeamChip, SqlConnection db)
        {
            try
            {
                bool rowsUpdated = false;

                rowsUpdated = db.Update(userTeamChip, commandTimeout: 300);

                if (rowsUpdated == true)
                {
                    Logger.Out("UserTeamChip: " + userTeamChip.name + " - updated");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamChip Repository (update) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeamChip(int userTeamid, int gamweekid, int chipid, SqlConnection db)
        {
            try
            {
                bool rowsDeleted = false;
                var userTeamChip = new UserTeamChip();

                userTeamChip = db.Get<UserTeamChip>(chipid);
                rowsDeleted = db.Delete(new UserTeamChip() { userteamid = userTeamid });

                if (rowsDeleted == true)
                {
                    Logger.Out("UserTeamChip: " + userTeamChip.name + " (" + Convert.ToString(chipid) + ") - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamChip Repository (DeleteUserTeamChip) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeamChipId(int userTeamid, int gameweekid, int chipid, SqlConnection db)
        {
            try
            {
                string userTeamChipName;
                int rowsDeleted;

                var userTeamChip = db.Get<UserTeamChip>(chipid);
                userTeamChipName = userTeamChip.name;

                string deleteQuery = "DELETE FROM dbo.UserTeamChip WHERE userTeamid = @UserTeamId AND gameweekid = @GameweekId AND chipid = @ChipId;";
                rowsDeleted = db.Execute(deleteQuery, new { UserTeamId = userTeamid, GameweekId = gameweekid, ChipId = chipid });

                if (rowsDeleted > 0)
                {
                    Logger.Out("UserTeamChip: " + userTeamChipName + " (" + Convert.ToString(chipid) + ") - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamChip Repository (DeleteUserTeamChipId) error: " + ex.Message);
                throw ex;
            }
        }

        //public List<int> GetAllUserTeamChipIdsForUserTeamId(int userTeamId, SqlConnection db)
        //{
        //    try
        //    {
        //        string selectQuery = @"SELECT chipid FROM dbo.UserTeamChip WHERE UserTeamId = @UserTeamId;";

        //        IDataReader reader = db.ExecuteReader(selectQuery, new { UserTeamId = userTeamId }, commandTimeout: 300);

        //        List<int> result = ReadList(reader);

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("UserTeamChip Repository (GetAllUserTeamChipIdsForUserTeamId) error: " + ex.Message);
        //        throw ex;
        //    }
        //}

        public List<UserTeamChipId> GetAllUserTeamChipIdsForUserTeamIdAndGameweekIdAndChipId(UserTeamChipId userTeamChipId, SqlConnection db)
        {
            try
            {
                int userTeamId, gameweekId, chipId;

                userTeamId = userTeamChipId.userteamid;
                gameweekId = userTeamChipId.gameweekid;
                chipId = userTeamChipId.chipid;

                string selectQuery = @"SELECT DISTINCT userteamid, gameweekid, chipid FROM dbo.UserTeamChip WHERE userteamid = @UserTeamId AND gameweekid = @GameweekId AND chipid = @ChipId;";

                IDataReader reader = db.ExecuteReader(selectQuery, new { UserTeamId = userTeamId, GameweekId = gameweekId, ChipId = chipId });

                List<UserTeamChipId> result = ReadUserTeamChipIdList(reader);

                reader.Close();

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamChip Repository (GetAllUserTeamChipIdsForUserTeamIdAndGameweekIdAndChipId) error: " + ex.Message);
                throw ex;
            }
        }

        public List<UserTeamChipId> GetAllUserTeamChipIdsForUserTeamId(int userTeamId, SqlConnection db)
        {
            try
            {
                //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                //{
                    string selectQuery = @"SELECT DISTINCT userteamid, gameweekid, chipid FROM dbo.UserTeamChip WHERE userteamid = @UserTeamId;";

                    IDataReader reader = db.ExecuteReader(selectQuery, new { UserTeamId = userTeamId });

                    List<UserTeamChipId> result = ReadUserTeamChipIdList(reader);

                    reader.Close();

                    return result;
                //}
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamChip Repository (GetAllUserTeamChipIdsForUserTeamId) error: " + ex.Message);
                throw ex;
            }
        }

        public List<int> GetAllUserTeamIdsWithChips(SqlConnection db)
        {
            try
            {
                //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                //{
                    //Need to change query to include all userteamid up until max so don't reprocess blanks
                    string selectQuery = @"SELECT DISTINCT userteamid AS id FROM dbo.UserTeamChip";

                    IDataReader reader = db.ExecuteReader(selectQuery, commandTimeout: 300);

                    List<int> result = ReadIdList(reader);

                    reader.Close();

                    return result;
                //}
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamChip Repository (GetAllUserTeamIdsWithChips) error: " + ex.Message);
                throw ex;
            }
        }

        public int GetLatestUserTeamChipUserTeamId(SqlConnection db)
        {
            try
            {
                //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                //{
                    string selectQuery = @"SELECT MAX(userteamid) AS id FROM dbo.UserTeamChip utc WHERE NOT EXISTS (SELECT 1 FROM dbo.UserTeam_ManualInserts WHERE userteamid = utc.userteamid);";

                    var result = db.ExecuteScalar<int>(selectQuery, commandTimeout: 300);

                    return result;
                //}
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamChip Repository (GetLatestUserTeamChipUserTeamIds) error: " + ex.Message);
                throw ex;
            }
        }

        public string GetUserTeamChipName(int chipid, SqlConnection db)
        {
            try
            {
                var userTeamChip = db.Get<UserTeamChip>(chipid);

                string userTeamChipName = userTeamChip.name;

                return userTeamChipName;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamChip Repository (GetUserTeamChipName) error: " + ex.Message);
                throw ex;
            }
        }

        public List<int> GetCompetedUserTeamChipIds(SqlConnection db)
        {
            try
            { 
                string selectQuery = @"SELECT c.id FROM dbo.UserTeamChip c INNER JOIN dbo.Gameweeks g ON c.gameweekId = g.id WHERE g.id = (SELECT TOP 1 id FROM dbo.Gameweeks WHERE deadline_time < GETDATE() ORDER BY deadline_time DESC)";

                IDataReader reader = db.ExecuteReader(selectQuery);

                List<int> result = ReadList(reader);

                reader.Close();

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamChip Repository (GetCompetedUserTeamChipIds) error: " + ex.Message);
                throw ex;
            }
        }

        public int GetChipIdFromName(string chipName, SqlConnection db)
        {
            try
            {
                string selectQuery = @"SELECT chipid AS id FROM dbo.Chip utc WHERE chipname = @ChipName;";

                var result = db.ExecuteScalar<int>(selectQuery, new { ChipName = chipName }, commandTimeout: 300);

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamChip Repository (GetChipIdFromName) error: " + ex.Message);
                throw ex;
            }
        }

        List<int> ReadList(IDataReader reader)
        {
            try
            { 
                List<int> list = new List<int>();
                int column = reader.GetOrdinal("chipid");

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
                Logger.Error("UserTeamChip Repository (ReadList) error: " + ex.Message);
                throw ex;
            }
        }

        List<int> ReadIdList(IDataReader reader)
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
                Logger.Error("UserTeamChip Repository (ReadIdList) error: " + ex.Message);
                throw ex;
            }
        }

        List<UserTeamChipId> ReadUserTeamChipIdList(IDataReader reader)
        {
            try
            {
                List<UserTeamChipId> list = new List<UserTeamChipId>();
                int column1 = reader.GetOrdinal("userteamid");
                int column2 = reader.GetOrdinal("gameweekid");
                int column3 = reader.GetOrdinal("chipid");

                //UserTeamChipId userTeamChipId = new UserTeamChipId(0, 0, 0);
                //UserTeamChipId userTeamChipId = new UserTeamChipId();

                while (reader.Read())
                {
                    //check for the null value and than add 
                    if (!reader.IsDBNull(column1))
                    {
                        UserTeamChipId userTeamChipId = new UserTeamChipId(0, 0, 0);

                        userTeamChipId.userteamid = reader.GetInt32(column1);
                        userTeamChipId.gameweekid = reader.GetInt32(column2);
                        userTeamChipId.chipid = reader.GetInt32(column3);
                        list.Add(userTeamChipId);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamChip Repository (GetChipIdFromName) error: " + ex.Message);
                throw ex;
            }
        }
    }
}