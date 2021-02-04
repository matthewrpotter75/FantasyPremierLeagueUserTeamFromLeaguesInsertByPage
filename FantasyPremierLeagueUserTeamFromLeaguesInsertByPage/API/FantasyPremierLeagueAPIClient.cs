﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace FantasyPremierLeagueUserTeams
{
    public class FantasyPremierLeagueAPIClient
    {
        public static void GetUserTeamDataJson(int userTeamId, List<int> userTeamIds, int maxGWFromGameweekHistoryForUserTeamId, int maxGWFromPicksForUserTeamId, int maxGWFromTransferHistoryForUserTeamId, string userTeamUrl, UserTeamGameweekHistories userTeamGameweekHistoriesInsert, UserTeamPicks userTeamPicksInsert, UserTeamPickAutomaticSubs userTeamPickAutomaticSubsInsert, UserTeamChips userTeamChipsInsert, UserTeamTransferHistoryData userTeamTransferHistoriesInsert, UserTeamSeasons userTeamSeasonsInsert, UserTeamClassicLeagues userTeamClassicLeaguesInsert, UserTeamH2hLeagues userTeamH2hLeaguesInsert, SqlConnection db)
        {
            try
            {
                //Process UserTeam, UserTeamClassicLeague, UserTeamH2hLeague, UserTeamCup
                ////if (Globals.existingUserTeamId == 0 || Globals.leagueCountFromUserTeamClassicLeagueForUserTeamId == 0)
                //{
                    FantasyPremierLeagueAPIUserTeamLeagueAndCup.GetUserTeamLeagueAndCupJson(userTeamId, userTeamIds, userTeamUrl, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, db);
                //}
                //else
                //{
                //    Logger.Out("UserTeam: " + Convert.ToString(userTeamId) + " - skipped (already exists with leagues)");
                //    Logger.Out("");
                //}

                //Process UserTeamGameweekHistory, UserTeamChip, UserTeamSeasonHistory
                if (maxGWFromGameweekHistoryForUserTeamId < Globals.latestGameweek)
                {
                    FantasyPremierLeagueAPIGameweekHistory.GetUserTeamHistoryDataJson(userTeamId, maxGWFromGameweekHistoryForUserTeamId, userTeamGameweekHistoriesInsert, userTeamChipsInsert, userTeamSeasonsInsert, db);
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
                    FantasyPremierLeagueAPITransferHistory.GetUserTeamTransferHistoryDataJson(userTeamId, userTeamTransferHistoriesInsert, db);
                }

                //Logger.Out("");
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                //throw new Exception("GetUserTeamDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                if (Globals.userTeamRetries < 10)
                {
                    Globals.userTeamRetries += 1;
                    GetUserTeamDataJson(userTeamId, userTeamIds, maxGWFromGameweekHistoryForUserTeamId, maxGWFromPicksForUserTeamId, maxGWFromTransferHistoryForUserTeamId, userTeamUrl, userTeamGameweekHistoriesInsert, userTeamPicksInsert, userTeamPickAutomaticSubsInsert, userTeamChipsInsert, userTeamTransferHistoriesInsert, userTeamSeasonsInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, db);
                }
                else
                {
                    Logger.Error("GetUserTeamDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): Issue processing User Team skipping to next!!!");
                }
            }
        }
    }
}