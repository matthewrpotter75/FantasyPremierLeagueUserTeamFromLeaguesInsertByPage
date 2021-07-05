using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace FantasyPremierLeagueUserTeams
{
    public class FantasyPremierLeagueLeaguesAPIClient
    {
        #region methods

        public static bool GetLeagueDataJson(int leagueId, List<int> userTeamIds, string userTeamLeaguesUrl, string userTeamUrl, UserTeams userTeamsUpdateInsert, UserTeamCupMatches userTeamCupInsert, UserTeamClassicLeagues userTeamClassicLeaguesInsert, UserTeamH2hLeagues userTeamH2hLeaguesInsert, UserTeamGameweekHistories userTeamGameweekHistoriesInsert, UserTeamPicks userTeamPicksInsert, UserTeamPickAutomaticSubs userTeamPickAutomaticSubsInsert, UserTeamChips userTeamChipsInsert, UserTeamTransferHistoryData userTeamTransferHistoriesInsert, UserTeamSeasons userTeamSeasonsInsert, SqlConnection db)
        {
            UserTeamRepository userTeamRepository = new UserTeamRepository();
            UserTeamPickRepository userTeamPickRepository = new UserTeamPickRepository();
            UserTeamGameweekHistoryRepository userTeamGameweekHistoryRepository = new UserTeamGameweekHistoryRepository();
            UserTeamTransferHistoryRepository userTeamTransferHistoryRepository = new UserTeamTransferHistoryRepository();
            UserTeamClassicLeagueRepository userTeamClassicLeagueRepository = new UserTeamClassicLeagueRepository();
            UserTeamSeasonRepository userTeamSeasonRepository = new UserTeamSeasonRepository();

            try
            {
				int userTeamId = 0;
                bool has_next = false;
                List<int> leagueUserTeamIds = new List<int>();

                string url = string.Format(userTeamLeaguesUrl, leagueId, Globals.PageId);

                JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.None };

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (HttpClient client = new HttpClient())
                using (Stream s = client.GetStreamAsync(url).Result)
                using (StreamReader sr = new StreamReader(s))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    Globals.ApiCalls += 1;
                    Globals.ApiPageCalls += 1;

                    // read the json from a stream
                    // json size doesn't matter because only a small piece is read at a time from the HTTP request
                    var leagueStandingsData = serializer.Deserialize<LeagueStandingsData>(reader);

                    if (leagueStandingsData != null)
                    {
                        League league = leagueStandingsData.league;
                        Standings standings = leagueStandingsData.standings;

                        string leagueName = league.name;
                        has_next = standings.has_next;

                        Logger.Out(leagueName + " (" + Convert.ToString(leagueId) + "): Page " + Convert.ToString(Globals.PageId));
                        Logger.Out("");

                        if (db.State == ConnectionState.Closed)
                        {
                            db.ConnectionString = ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString;
                            db.Open();
                            Logger.Error("GetLeagueDataJson Info (LeagueId: " + leagueId.ToString() + ", PageId:" + Globals.PageId.ToString() + "): Reopening closed db connection");
                        }

                        List<int> pageUserTeamIds = new List<int>();

                        foreach (TeamLeaguePosition teamLeaguePosition in standings.results)
                        {
                            userTeamId = teamLeaguePosition.entry;

                            //Add each UserTeamId to a list for checking if GameweekHistory, TransferHistory, and Picks have already been processed for that GW by UserTeamId
                            pageUserTeamIds.Add(userTeamId);
                        }

                        using (DataTable dtUserMaxGWForUserTeamForGameweekHistory = userTeamGameweekHistoryRepository.GetMaxGameweekIdFromUserTeamGameweekHistoryForUserTeamIds(pageUserTeamIds, db))
                        using (DataTable dtUserMaxGWForUserTeamForPicks = userTeamPickRepository.GetMaxGameweekIdFromUserTeamPickForUserTeamIds(pageUserTeamIds, db))
                        using (DataTable dtUserMaxGWForUserTeamForTransferHistory = userTeamTransferHistoryRepository.GetMaxGameweekIdFromUserTeamTransferHistoryForUserTeamIds(pageUserTeamIds, db))
                        using (DataTable dtClassicLeagueCountForUserTeam = userTeamClassicLeagueRepository.GetClassicLeagueCountFromUserTeamClassicLeagueForUserTeamIds(pageUserTeamIds, db))
                        using (DataTable dtSeasonCountForUserTeam = userTeamSeasonRepository.GetSeasonCountFromUserTeamSeasonForUserTeamIds(pageUserTeamIds, db))
                        using (DataTable dtUserTeam = userTeamRepository.GetUserTeamForUserTeamIds(pageUserTeamIds, db))
                        {
                            foreach (TeamLeaguePosition teamLeaguePosition in standings.results)
                            {
                                userTeamId = teamLeaguePosition.entry;

                                Globals.MaxGWFromGameweekHistoryForUserTeamId = GetColumnNameValue(userTeamId, dtUserMaxGWForUserTeamForGameweekHistory, "gameweekid");
                                Globals.MaxGWFromPicksForUserTeamId = GetColumnNameValue(userTeamId, dtUserMaxGWForUserTeamForPicks, "gameweekid");
                                Globals.MaxGWFromTransferHistoryForUserTeamId = GetColumnNameValue(userTeamId, dtUserMaxGWForUserTeamForTransferHistory, "gameweekid");
                                Globals.LeagueCountFromUserTeamClassicLeagueForUserTeamId = GetColumnNameValue(userTeamId, dtClassicLeagueCountForUserTeam, "leagueCount");
                                Globals.SeasonCountFromUserTeamSeasonForUserTeamId = GetColumnNameValue(userTeamId, dtSeasonCountForUserTeam, "seasonCount");
                                Globals.ExistingUserTeamId = GetColumnNameValue(userTeamId, dtUserTeam, "userteamid");

                                FantasyPremierLeagueAPIClient.GetUserTeamDataJson(userTeamId, userTeamIds, userTeamUrl, userTeamsUpdateInsert, userTeamCupInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, userTeamGameweekHistoriesInsert, userTeamPicksInsert, userTeamPickAutomaticSubsInsert, userTeamChipsInsert, userTeamTransferHistoriesInsert, userTeamSeasonsInsert, db);
                            }
                        }
                        pageUserTeamIds.Clear();
                    }
                    return has_next;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetLeagueDataJson data exception (LeagueId: " + leagueId.ToString() + ", PageId:" + Globals.PageId.ToString() + "): " + ex.Message);

                bool has_next = true;

                if (Globals.LeagueRetries < 10)
                {
                    has_next = GetLeagueDataJson(leagueId, userTeamIds, userTeamLeaguesUrl, userTeamUrl, userTeamsUpdateInsert, userTeamCupInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, userTeamGameweekHistoriesInsert, userTeamPicksInsert, userTeamPickAutomaticSubsInsert, userTeamChipsInsert, userTeamTransferHistoriesInsert, userTeamSeasonsInsert, db);
                    Globals.LeagueRetries += 1;
                }
                else
                {
                    Logger.Error("GetLeagueDataJson data exception (LeagueId: " + leagueId.ToString() + "):  League/Page doesn't exist skipping to next!!!");
                    //Program.WriteToDB(pageId, userTeamGameweekHistoriesInsert, userTeamPicksInsert, userTeamPickAutomaticSubsInsert, userTeamChipsInsert, userTeamTransferHistoriesInsert, userTeamSeasonsInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, db);
                    Globals.LeagueRetries = 0;
                    //throw new Exception("GetLeagueDataJson data exception (LeagueId: " + leagueId.ToString() + ", PageId:" + Globals.PageId.ToString() + "): " + ex.Message);
                }

                return has_next;
            }
        }

        //private static int GetMaxGameweekId(int userTeamId, SqlDataReader dr)
        private static int GetColumnNameValue(int userTeamId, DataTable dt, string columnName)
        {
            try
            {
                int result = 0;

                foreach (DataRow row in dt.Rows)
                {
                    if (Convert.ToInt32(row["userteamid"]) == userTeamId)
                    {
                        //result = Convert.ToInt32(row["gameweekid"]);
                        result = Convert.ToInt32(row[columnName]);
                        return result;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("FantasyPremierLeagueLeaguesAPI (GetColumnNameValue) error: " + ex.Message);
                throw ex;
            }
        }
        #endregion
    }
}