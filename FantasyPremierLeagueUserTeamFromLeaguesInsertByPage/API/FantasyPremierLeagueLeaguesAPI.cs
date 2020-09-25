using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace FantasyPremierLeagueUserTeams
{
    public class FantasyPremierLeagueLeaguesAPIClient
    {
        #region methods

        public static bool GetLeagueDataJson(int leagueId, int pageId, List<int> userTeamIds, string userTeamLeaguesUrl, string userTeamUrl, UserTeamGameweekHistories userTeamGameweekHistoriesInsert, UserTeamPicks userTeamPicksInsert, UserTeamPickAutomaticSubs userTeamPickAutomaticSubsInsert, UserTeamChips userTeamChipsInsert, UserTeamTransferHistoryData userTeamTransferHistoriesInsert, UserTeamSeasons userTeamSeasonsInsert, UserTeamClassicLeagues userTeamClassicLeaguesInsert, UserTeamH2hLeagues userTeamH2hLeaguesInsert, int userTeamRetries, SqlConnection db)
        {
            int leagueRetries = 0;

            UserTeamRepository userTeamRepository = new UserTeamRepository();
            UserTeamPickRepository userTeamPickRepository = new UserTeamPickRepository();
            UserTeamPickAutomaticSubRepository userTeamPickAutomaticSubRepository = new UserTeamPickAutomaticSubRepository();
            UserTeamGameweekHistoryRepository userTeamGameweekHistoryRepository = new UserTeamGameweekHistoryRepository();
            UserTeamChipRepository userTeamChipRepository = new UserTeamChipRepository();
            UserTeamTransferHistoryRepository userTeamTransferHistoryRepository = new UserTeamTransferHistoryRepository();
            UserTeamSeasonRepository userTeamSeasonRepository = new UserTeamSeasonRepository();
            UserTeamClassicLeagueRepository userTeamClassicLeagueRepository = new UserTeamClassicLeagueRepository();
            UserTeamH2hLeagueRepository userTeamH2hLeagueRepository = new UserTeamH2hLeagueRepository();

            try
            {
				int userTeamId = 0;
                bool has_next = false;
                List<int> leagueUserTeamIds = new List<int>();

                string url = string.Format(userTeamLeaguesUrl, leagueId, pageId);

                HttpClient client = new HttpClient();
                JsonSerializer serializer = new JsonSerializer();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (Stream s = client.GetStreamAsync(url).Result)
                using (StreamReader sr = new StreamReader(s))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    Globals.apiCalls += 1;
                    Globals.apiPageCalls += 1;

                    // read the json from a stream
                    // json size doesn't matter because only a small piece is read at a time from the HTTP request
                    var leagueStandingsData = serializer.Deserialize<LeagueStandingsData>(reader);

                    if (leagueStandingsData != null)
                    {
                        League league = leagueStandingsData.league;
                        Standings standings = leagueStandingsData.standings;

                        string leagueName = league.name;
                        has_next = standings.has_next;

                        Logger.Out(leagueName + " (" + Convert.ToString(leagueId) + "): Page " + Convert.ToString(pageId));
                        Logger.Out("");

                        List<int> pageUserTeamIds = new List<int>();

                        foreach (TeamLeaguePosition teamLeaguePosition in standings.results)
                        {
                            userTeamId = teamLeaguePosition.entry;

                            //Add each UserTeamId to a list for checking if GameweekHistory, TransferHistory, and Picks have already been processed for that GW by UserTeamId
                            pageUserTeamIds.Add(userTeamId);
                        }

                        int maxGWFromGameweekHistoryForUserTeamId;
                        int maxGWFromPicksForUserTeamId;
                        int maxGWFromTransferHistoryForUserTeamId;

                        using (DataTable dtUserMaxGWForUserTeamForGameweekHistory = userTeamGameweekHistoryRepository.GetMaxGameweekIdFromUserTeamGameweekHistoryForUserTeamIds(pageUserTeamIds, db))
                        using (DataTable dtUserMaxGWForUserTeamForPicks = userTeamPickRepository.GetMaxGameweekIdFromUserTeamPickForUserTeamIds(pageUserTeamIds, db))
                        using (DataTable dtUserMaxGWForUserTeamForTransferHistory = userTeamTransferHistoryRepository.GetMaxGameweekIdFromUserTeamTransferHistoryForUserTeamIds(pageUserTeamIds, db))
                        using (DataTable dtClassicLeagueCountForUserTeam = userTeamClassicLeagueRepository.GetClassicLeagueCountFromUserTeamClassicLeagueForUserTeamIds(pageUserTeamIds, db))
                        using (DataTable dtUserTeam = userTeamRepository.GetUserTeamForUserTeamIds(pageUserTeamIds, db))
                        {
                            foreach (TeamLeaguePosition teamLeaguePosition in standings.results)
                            {
                                userTeamId = teamLeaguePosition.entry;

                                maxGWFromGameweekHistoryForUserTeamId = GetColumnNameValue(userTeamId, dtUserMaxGWForUserTeamForGameweekHistory, "gameweekid");
                                maxGWFromPicksForUserTeamId = GetColumnNameValue(userTeamId, dtUserMaxGWForUserTeamForPicks, "gameweekid");
                                maxGWFromTransferHistoryForUserTeamId = GetColumnNameValue(userTeamId, dtUserMaxGWForUserTeamForTransferHistory, "gameweekid");
                                Globals.leagueCountFromUserTeamClassicLeagueForUserTeamId = GetColumnNameValue(userTeamId, dtClassicLeagueCountForUserTeam, "leagueCount");
                                Globals.existingUserTeamId = GetColumnNameValue(userTeamId, dtUserTeam, "userteamid");

                                FantasyPremierLeagueAPIClient.GetUserTeamDataJson(userTeamId, userTeamIds, maxGWFromGameweekHistoryForUserTeamId, maxGWFromPicksForUserTeamId, maxGWFromTransferHistoryForUserTeamId, userTeamUrl, userTeamGameweekHistoriesInsert, userTeamPicksInsert, userTeamPickAutomaticSubsInsert, userTeamChipsInsert, userTeamTransferHistoriesInsert, userTeamSeasonsInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, userTeamRetries, db);
                            }
                        }

                        pageUserTeamIds.Clear();
                    }
                    return has_next;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetLeagueDataJson data exception (LeagueId: " + leagueId.ToString() + ", PageId:" + pageId.ToString() + "): " + ex.Message);
                //throw new Exception("GetLeagueDataJson data exception (LeagueId: " + leagueId.ToString() + ", PageId:" + pageId.ToString() + "): " + ex.Message);

                bool has_next = true;

                if (leagueRetries < 50)
                {
                    //int userTeamPickRowsInserted = 0;
                    //int userTeamPickAutomaticSubsRowsInserted = 0;
                    //int userTeamGameweekHistoryRowsInserted = 0;
                    //int userTeamChipRowsInserted = 0;
                    //int userTeamTransferHistoryRowsInserted = 0;
                    //int userTeamSeasonRowsInserted = 0;
                    //int userTeamClassicLeagueRowsInserted = 0;
                    //int userTeamH2hLeagueRowsInserted = 0;

                    //userTeamChipRowsInserted = userTeamChipRepository.InsertUserTeamChip(userTeamChipsInsert, db);
                    //Logger.Out("UserTeamChip bulk insert complete");

                    ////Write UserTeamGameweekHistory to the db
                    //userTeamGameweekHistoryRowsInserted = userTeamGameweekHistoryRepository.InsertUserTeamGameweekHistories(userTeamGameweekHistoriesInsert, db);
                    //Logger.Out("UserTeamGameweekHistory bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                    ////Write UserTeamPick to the db
                    //userTeamPickRowsInserted = userTeamPickRepository.InsertUserTeamPick(userTeamPicksInsert, db);
                    //Logger.Out("UserTeamPick bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                    ////Write UserTeamPickAutomaticSub to the db
                    //userTeamPickAutomaticSubsRowsInserted = userTeamPickAutomaticSubRepository.InsertUserTeamPickAutomaticSubs(userTeamPickAutomaticSubsInsert, db);
                    //Logger.Out("UserTeamPickAutomaticSub bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                    ////Write UserTeamTransferHistory to the db
                    //userTeamTransferHistoryRowsInserted = userTeamTransferHistoryRepository.InsertUserTeamTransferHistories(userTeamTransferHistoriesInsert, db);
                    //Logger.Out("UserTeamTransferHistory bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                    ////Write UserTeamSeason to the db
                    //userTeamSeasonRowsInserted = userTeamSeasonRepository.InsertUserTeamSeason(userTeamSeasonsInsert, db);
                    //Logger.Out("UserTeamSeason bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                    ////Write UserTeamClassicLeague to the db
                    //userTeamClassicLeagueRowsInserted = userTeamClassicLeagueRepository.InsertUserTeamClassicLeague(userTeamClassicLeaguesInsert, db);
                    //Logger.Out("UserTeamClassicLeague bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                    ////Write UserTeamH2hLeague to the db
                    //userTeamH2hLeagueRowsInserted = userTeamH2hLeagueRepository.InsertUserTeamH2hLeague(userTeamH2hLeaguesInsert, db);
                    //Logger.Out("UserTeamH2hLeague bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                    //Logger.Out("");
                    //Logger.Out("UserTeamChip: " + Convert.ToString(userTeamChipRowsInserted) + " rows inserted");
                    //Logger.Out("UserTeamGameweekHistory: " + Convert.ToString(userTeamGameweekHistoryRowsInserted) + " rows inserted");
                    //Logger.Out("UserTeamPick: " + Convert.ToString(userTeamPickRowsInserted) + " rows inserted");
                    //Logger.Out("UserTeamPickAutomaticSub: " + Convert.ToString(userTeamPickAutomaticSubsRowsInserted) + " rows inserted");
                    //Logger.Out("UserTeamTransferHistory: " + Convert.ToString(userTeamTransferHistoryRowsInserted) + " rows inserted");
                    //Logger.Out("UserTeamSeason: " + Convert.ToString(userTeamSeasonRowsInserted) + " rows inserted");
                    //Logger.Out("UserTeamClassicLeague: " + Convert.ToString(userTeamClassicLeagueRowsInserted) + " rows inserted");
                    //Logger.Out("UserTeamH2hLeague: " + Convert.ToString(userTeamH2hLeagueRowsInserted) + " rows inserted");
                    //Logger.Out("");

                    //leagueRetries += 1;
                    //userTeamPicksInsert.Clear();
                    //userTeamPickAutomaticSubsInsert.Clear();
                    //userTeamGameweekHistoriesInsert.Clear();
                    //userTeamChipsInsert.Clear();
                    //userTeamTransferHistoriesInsert.Clear();
                    //userTeamSeasonsInsert.Clear();
                    //userTeamClassicLeaguesInsert.Clear();
                    //userTeamH2hLeaguesInsert.Clear();

                    has_next = FantasyPremierLeagueLeaguesAPIClient.GetLeagueDataJson(leagueId, pageId, userTeamIds, userTeamLeaguesUrl, userTeamUrl, userTeamGameweekHistoriesInsert, userTeamPicksInsert, userTeamPickAutomaticSubsInsert, userTeamChipsInsert, userTeamTransferHistoriesInsert, userTeamSeasonsInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, userTeamRetries, db);
                }
                else
                {
                    Logger.Error("GetLeagueDataJson data exception (LeagueId: " + leagueId.ToString() + "):  League/Page doesn't exist skipping to next!!!");
                    leagueRetries = 0;
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