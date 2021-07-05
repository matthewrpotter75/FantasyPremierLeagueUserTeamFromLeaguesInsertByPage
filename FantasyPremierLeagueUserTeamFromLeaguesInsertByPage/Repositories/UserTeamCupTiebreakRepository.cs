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
    public class UserTeamCupTiebreakRepository : IUserTeamCupTiebreak
    {
        public bool InsertUserTeamCupTiebreak(UserTeamCupTiebreak cupTiebreak)
        {
            try
            {
                long rowsAffected = 0;

                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                {
                    rowsAffected = db.Insert(cupTiebreak, commandTimeout: 0);
                }

                if (rowsAffected > 0)
                {
                    Logger.Out("CupTiebreak: " + cupTiebreak.name + " - inserted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("CupTiebreak Repository (insert) error: " + ex.Message);
                throw ex;
            }
        }

        public bool UpdateUserTeamCupTiebreak(UserTeamCupTiebreak cupTiebreak)
        {
            try
            {
                bool rowsUpdated = false;

                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                {
                    rowsUpdated = db.Update(cupTiebreak, commandTimeout: 0);
                }

                if (rowsUpdated == true)
                {
                    Logger.Out("CupTiebreak: " + cupTiebreak.name + " - updated");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("CupTiebreak Repository (update) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeamCupTiebreak(int userteamcupid)
        {
            try
            {
                bool rowsDeleted = false;
                var cupTiebreak = new UserTeamCupTiebreak();

                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                {
                    cupTiebreak = db.Get<UserTeamCupTiebreak>(userteamcupid, commandTimeout: 0);
                    rowsDeleted = db.Delete(new UserTeamCupTiebreak() { userteamcupid = userteamcupid });
                }

                if (rowsDeleted == true)
                {
                    Logger.Out("CupTiebreak: " + cupTiebreak.name + " (" + Convert.ToString(userteamcupid) + ") - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("CupTiebreak Repository (DeleteCup) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeamCupTiebreakId(int userteamcupid)
        {
            try
            {
                string cupTiebreakName;
                int rowsDeleted;

                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                {
                    var cupTiebreak = db.Get<UserTeamCupTiebreak>(userteamcupid);
                    cupTiebreakName = cupTiebreak.name;

                    string deleteQuery = "DELETE FROM dbo.UserTeamCupTiebreak WHERE userteamcupid = @UserTeamCupId;";
                    rowsDeleted = db.Execute(deleteQuery, new { UserTeamCupId = userteamcupid });

                    if (rowsDeleted > 0)
                    {
                        Logger.Out("CupTiebreak: " + cupTiebreak.name + " (" + Convert.ToString(userteamcupid) + ") - deleted");
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("CupTiebreak Repository (DeleteCupId) error: " + ex.Message);
                throw ex;
            }
        }

        public List<int> GetAllCupIdsWithTiebreak()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                {
                    //string selectQuery = @"SELECT id FROM dbo.UserTeamCup WHERE homeTeam_points = awayTeam_points;";
                    string selectQuery = @"SELECT homeTeam_userTeamid AS id FROM dbo.UserTeamCup utc WHERE homeTeam_points = awayTeam_points AND NOT EXISTS (SELECT 1 FROM dbo.UserTeamCupTiebreak WHERE userteamcupid = utc.id) ORDER BY id;";

                    IDataReader reader = db.ExecuteReader(selectQuery, commandTimeout: 0);

                    List<int> result = ReadList(reader);

                    reader.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CupTiebreak Repository (GetAllCupIdsWithTiebreak) error: " + ex.Message);
                throw ex;
            }
        }

        public List<string> GetAllCupNamesForUserId(int userTeamId)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                {
                    string selectQuery = @"SELECT utct.tiebreak_name FROM dbo.UserTeamCupTiebreak utct INNER JOIN dbo.UserTeamCup utc ON utct.userteamcupid = utc.id WHERE utc.homeTeam_userTeamid = @UserTeamId";

                    IDataReader reader = db.ExecuteReader(selectQuery, new { UserTeamId = userTeamId }, commandTimeout: 0);

                    List<string> result = ReadStringList(reader);

                    reader.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CupTiebreak Repository (GetAllCupNamesForUserId) error: " + ex.Message);
                throw ex;
            }
        }

        public string GetCupTiebreakName(int userteamcupId)
        {
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
            {
                var cupTiebreak = db.Get<UserTeamCupTiebreak>(userteamcupId, commandTimeout: 0);

                string cupTiebreakName = cupTiebreak.name;

                return cupTiebreakName;
            }
        }

        List<int> ReadList(IDataReader reader)
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

        List<string> ReadStringList(IDataReader reader)
        {
            List<string> list = new List<string>();
            int column = reader.GetOrdinal("tiebreak_name");

            while (reader.Read())
            {
                //check for the null value and than add 
                if (!reader.IsDBNull(column))
                    list.Add(reader.GetString(column));
            }

            return list;
        }

    }
}