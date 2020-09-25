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
    public class FantasyPremierLeagueAPIClient
    {
        public static void GetUserTeamDataJson(int userTeamId, List<int> userTeamIds, int maxGWFromGameweekHistoryForUserTeamId, int maxGWFromPicksForUserTeamId, int maxGWFromTransferHistoryForUserTeamId, string userTeamUrl, UserTeamGameweekHistories userTeamGameweekHistoriesInsert, UserTeamPicks userTeamPicksInsert, UserTeamPickAutomaticSubs userTeamPickAutomaticSubsInsert, UserTeamChips userTeamChipsInsert, UserTeamTransferHistoryData userTeamTransferHistoriesInsert, UserTeamSeasons userTeamSeasonsInsert, UserTeamClassicLeagues userTeamClassicLeaguesInsert, UserTeamH2hLeagues userTeamH2hLeaguesInsert, int userTeamRetries, SqlConnection db)
        {
            try
            {
                //Process UserTeam, UserTeamClassicLeague, UserTeamH2hLeague, UserTeamCup
                if (Globals.existingUserTeamId == 0 || Globals.leagueCountFromUserTeamClassicLeagueForUserTeamId == 0)
                {
                    FantasyPremierLeagueAPIUserTeamLeagueAndCup.GetUserTeamLeagueAndCupJson(userTeamId, userTeamIds, userTeamUrl, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, db);
                }
                else
                {
                    Logger.Out("UserTeam: " + Convert.ToString(userTeamId) + " - skipped (already exists with leagues)");
                    Logger.Out("");
                }

                //Process UserTeamGameweekHistory, UserTeamChip, UserTeamSeasonHistory
                if (maxGWFromGameweekHistoryForUserTeamId < Globals.latestGameweek || maxGWFromPicksForUserTeamId < Globals.latestGameweek)
                {
                    FantasyPremierLeagueAPIGameweekHistory.GetUserTeamHistoryDataJson(userTeamId, userTeamIds, maxGWFromGameweekHistoryForUserTeamId, userTeamGameweekHistoriesInsert, userTeamPicksInsert, userTeamPickAutomaticSubsInsert, userTeamChipsInsert, userTeamSeasonsInsert, db);
                }

                //Process UserTeamPick, UserTeamPickAutomaticSub
                //Doesn't check UserTeamPickAutomaticSub - which will be missing if run during an active Gameweek - matches are in progress or before have been played)
                if (maxGWFromPicksForUserTeamId < Globals.actualGameweek && Globals.actualGameweek > 0)
                {
                    FantasyPremierLeagueAPIPick.GetUserTeamPickDataJson(userTeamId, maxGWFromPicksForUserTeamId, userTeamPicksInsert, userTeamPickAutomaticSubsInsert, db);
                }

                //Process UserTeamTransferHistory
                if (maxGWFromTransferHistoryForUserTeamId < Globals.actualGameweek && Globals.actualGameweek > 0)
                {
                    FantasyPremierLeagueAPITransferHistory.GetUserTeamTransferHistoryDataJson(userTeamId, maxGWFromTransferHistoryForUserTeamId, userTeamTransferHistoriesInsert, db);
                }

                //Logger.Out("");
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                throw new Exception("GetUserTeamDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                //if (userTeamRetries < 50)
                //{
                //    userTeamRetries += 1;
                //    GetUserTeamDataJson(userTeamId, userTeamIds, userTeamUrl, userTeamRetries);
                //}
                //else
                //{
                //    Logger.Error("GetUserTeamDataJson data exception (UserTeamId: " + userTeamId.ToString() + "):  User Team doesn't exist skipping to next!!!");
                //}
            }
        }
    }
}