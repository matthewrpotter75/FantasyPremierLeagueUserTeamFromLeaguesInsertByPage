using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Dapper;
using DapperExtensions;
using System.Linq;

namespace FantasyPremierLeagueUserTeams
{
    public class UserTeamCupRepository : IUserTeamCup
    {
        public bool InsertUserTeamCup(UserTeamCupMatch cup, SqlConnection db)
        {
            try
            {
                long rowsAffected = 0;

                rowsAffected = db.Insert(cup);

                if (rowsAffected > 0)
                {
                    Logger.Out("Cup: " + cup.entry_1_name + " v " + cup.entry_2_name + " - inserted");
                    return true;
                }
                return false;
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
                string selectQuery = @"SELECT id FROM dbo.UserTeamCup WHERE homeTeam_userteamid = @UserTeamId OR awayTeam_userteamid = @UserTeamId";

                IDataReader reader = db.ExecuteReader(selectQuery, new { UserTeamId = userTeamId });

                List<int> result = ReadList(reader);

                reader.Close();

                return result;
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
            //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
            //{
                string selectQuery = @"SELECT c.id FROM dbo.UserTeamCup c INNER JOIN dbo.Gameweeks g ON c.gameweekId = g.id WHERE g.id = (SELECT TOP 1 id FROM dbo.Gameweeks WHERE deadline_time < GETDATE() ORDER BY deadline_time DESC)";

                IDataReader reader = db.ExecuteReader(selectQuery);

                List<int> result = ReadList(reader);

                reader.Close();

                return result;
            //}
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