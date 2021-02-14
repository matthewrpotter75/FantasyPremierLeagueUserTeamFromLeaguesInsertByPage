using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using DapperExtensions;
using DataStreams.ETL;

namespace FantasyPremierLeagueUserTeams
{
    public class UserTeamCupRepository : IUserTeamCup
    {
        public int InsertUserTeamCup(UserTeamCupMatches userTeamCupMatches, SqlConnection db)
        {
            int rowsAffected = 0;

            try
            {
                using (IDataReader reader = userTeamCupMatches.GetDataReader())
                {
                    using (var bulkCopy = new SqlBulkCopy(db))
                    {
                        bulkCopy.BulkCopyTimeout = 1000;
                        bulkCopy.BatchSize = 1000;
                        bulkCopy.DestinationTableName = "UserTeamCupStaging";
                        bulkCopy.EnableStreaming = true;

                        // Add your column mappings here
                        bulkCopy.ColumnMappings.Add("id", "id");
                        bulkCopy.ColumnMappings.Add("entry_1_entry", "homeTeam_userTeamid");
                        bulkCopy.ColumnMappings.Add("entry_1_name", "homeTeam_userTeamName");
                        bulkCopy.ColumnMappings.Add("entry_1_player_name", "homeTeam_playerName");
                        bulkCopy.ColumnMappings.Add("entry_1_points", "homeTeam_points");
                        bulkCopy.ColumnMappings.Add("entry_1_win", "homeTeam_win");
                        bulkCopy.ColumnMappings.Add("entry_1_draw", "homeTeam_draw");
                        bulkCopy.ColumnMappings.Add("entry_1_loss", "homeTeam_loss");
                        bulkCopy.ColumnMappings.Add("entry_1_total", "homeTeam_total");
                        bulkCopy.ColumnMappings.Add("entry_2_entry", "awayTeam_userTeamid");
                        bulkCopy.ColumnMappings.Add("entry_2_name", "awayTeam_userTeamName");
                        bulkCopy.ColumnMappings.Add("entry_2_player_name", "awayTeam_playerName");
                        bulkCopy.ColumnMappings.Add("entry_2_points", "awayTeam_points");
                        bulkCopy.ColumnMappings.Add("entry_2_win", "awayTeam_win");
                        bulkCopy.ColumnMappings.Add("entry_2_draw", "awayTeam_draw");
                        bulkCopy.ColumnMappings.Add("entry_2_loss", "awayTeam_loss");
                        bulkCopy.ColumnMappings.Add("entry_2_total", "awayTeam_total");
                        bulkCopy.ColumnMappings.Add("event", "gameweekid");

                        bulkCopy.ColumnMappings.Add("is_knockout", "is_knockout");
                        bulkCopy.ColumnMappings.Add("winner", "winner");
                        bulkCopy.ColumnMappings.Add("seed_value", "seed_value");
                        bulkCopy.ColumnMappings.Add("fromuserteamid", "fromuserteamid");
                        bulkCopy.ColumnMappings.Add("tiebreak", "tiebreak");

                        bulkCopy.WriteToServer(reader);
                        rowsAffected = SqlBulkCopyExtension.RowsCopiedCount(bulkCopy);
                    }
                }
                return rowsAffected;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamCup Repository (insert) error: " + ex.Message);
                throw ex;
            }
        }

        public bool UpdateUserTeamCup(UserTeamCupMatch cup, SqlConnection db)
        {
            try
            {
                bool rowsUpdated = false;

                rowsUpdated = db.Update(cup);

                if (rowsUpdated == true)
                {
                    Logger.Out("Cup: " + cup.entry_1_name + " v " + cup.entry_2_name + " - updated");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamCup Repository (update) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeamCup(int cupid, SqlConnection db)
        {
            try
            {
                bool rowsDeleted = false;
                var cup = new UserTeamCupMatch();

                cup = db.Get<UserTeamCupMatch>(cupid);
                rowsDeleted = db.Delete(new UserTeamCupMatch() { id = cupid });                    

                if (rowsDeleted == true)
                {
                    Logger.Out("Cup: " + cup.entry_1_name + " v " + cup.entry_2_name + " (" + Convert.ToString(cupid) + ") - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamCup Repository (DeleteCup) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeamCupId(int cupid, SqlConnection db)
        {
            try
            {
                string cupName;
                int rowsDeleted;

                var cup = db.Get<UserTeamCupMatch>(cupid);
                cupName = cup.entry_1_name + " v " + cup.entry_2_name;

                string deleteQuery = "DELETE FROM dbo.UserTeamCup WHERE id = @CupId;";
                rowsDeleted = db.Execute(deleteQuery, new { CupId = cupid });

                if (rowsDeleted > 0)
                {
                    Logger.Out("UserTeamCup: " + cupName + " (" + Convert.ToString(cupid) + ") - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("Cup Repository (DeleteCupId) error: " + ex.Message);
                throw ex;
            }
        }

        public List<int> GetAllCupIdsForUserId(int userTeamId, SqlConnection db)
        {
            try
            {
                using (IDbCommand cmd = db.CreateCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandTimeout = 300;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetAllCupIdsForUserId";

                    IDataParameter param = cmd.CreateParameter();
                    param.ParameterName = "@UserTeamId";
                    param.Value = userTeamId;
                    cmd.Parameters.Add(param);

                    //string selectQuery = @"SELECT id FROM dbo.UserTeamCup WHERE homeTeam_userteamid = @UserTeamId OR awayTeam_userteamid = @UserTeamId";

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
                Logger.Error("UserTeamCup Repository (GetAllCupIds) error: " + ex.Message);
                throw ex;
            }
        }

        public string GetCupName(int cupId, SqlConnection db)
        {
            try
            {
                var cup = db.Get<UserTeamCupMatch>(cupId);

                string cupName = cup.entry_1_name + " v " + cup.entry_2_name;

                return cupName;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamCup Repository (GetCupName) error: " + ex.Message);
                throw ex;
            }
        }

        public List<int> GetCompetedCupIds(SqlConnection db)
        {
            try
            {
                using (IDbCommand cmd = db.CreateCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandTimeout = 300;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetCompetedCupIds";

                    //string selectQuery = @"SELECT c.id FROM dbo.UserTeamCup c INNER JOIN dbo.Gameweeks g ON c.gameweekId = g.id WHERE g.id = (SELECT TOP 1 id FROM dbo.Gameweeks WHERE deadline_time < GETDATE() ORDER BY deadline_time DESC)";

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
                Logger.Error("UserTeamCup Repository (GetCompetedCupIds) error: " + ex.Message);
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
                Logger.Error("UserTeamCup Repository (ReadList) error: " + ex.Message);
                throw ex;
            }
        }
    }
}