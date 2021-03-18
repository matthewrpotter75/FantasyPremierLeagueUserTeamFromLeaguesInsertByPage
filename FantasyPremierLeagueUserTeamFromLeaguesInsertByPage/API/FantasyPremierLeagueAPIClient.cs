using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace FantasyPremierLeagueUserTeams
{
    public class FantasyPremierLeagueAPIClient
    {
        public static void GetUserTeamDataJson(int userTeamId, List<int> userTeamIds, string userTeamUrl, UserTeams userTeamsUpdateInsert, UserTeamCupMatches userTeamCupInsert, UserTeamClassicLeagues userTeamClassicLeaguesInsert, UserTeamH2hLeagues userTeamH2hLeaguesInsert, UserTeamGameweekHistories userTeamGameweekHistoriesInsert, UserTeamPicks userTeamPicksInsert, UserTeamPickAutomaticSubs userTeamPickAutomaticSubsInsert, UserTeamChips userTeamChipsInsert, UserTeamTransferHistoryData userTeamTransferHistoriesInsert, UserTeamSeasons userTeamSeasonsInsert, SqlConnection db)
        {
            try
            {
                //Process UserTeam, UserTeamClassicLeague, UserTeamH2hLeague, UserTeamCup
                ////if (Globals.ExistingUserTeamId == 0 || Globals.LeagueCountFromUserTeamClassicLeagueForUserTeamId == 0)
                //{
                    FantasyPremierLeagueAPIUserTeamLeagueAndCup.GetUserTeamLeagueAndCupJson(userTeamId, userTeamIds, userTeamUrl, userTeamsUpdateInsert, userTeamCupInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, db);
                //}
                //else
                //{
                //    Logger.Out("UserTeam: " + Convert.ToString(userTeamId) + " - skipped (already exists with leagues)");
                //}

                //Process UserTeamGameweekHistory, UserTeamChip, UserTeamSeasonHistory
                if (Globals.MaxGWFromGameweekHistoryForUserTeamId < Globals.LatestGameweek)
                {
                    FantasyPremierLeagueAPIGameweekHistory.GetUserTeamHistoryDataJson(userTeamId, userTeamGameweekHistoriesInsert, userTeamChipsInsert, userTeamSeasonsInsert, db);
                }

                //Process UserTeamPick, UserTeamPickAutomaticSub
                //Doesn't check UserTeamPickAutomaticSub - which will be missing if run during an active Gameweek - matches are in progress or before have been played)
                if (Globals.MaxGWFromPicksForUserTeamId < Globals.ActualGameweek && Globals.ActualGameweek > 0)
                {
                    FantasyPremierLeagueAPIPick.GetUserTeamPickDataJson(userTeamId, userTeamPicksInsert, userTeamPickAutomaticSubsInsert, db);
                }

                //Process UserTeamTransferHistory
                if (Globals.MaxGWFromTransferHistoryForUserTeamId < Globals.ActualGameweek && Globals.ActualGameweek > 0)
                {
                    FantasyPremierLeagueAPITransferHistory.GetUserTeamTransferHistoryDataJson(userTeamId, userTeamTransferHistoriesInsert, db);
                }

                Logger.Out("");
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                //throw new Exception("GetUserTeamDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                if (Globals.UserTeamRetries < 10)
                {
                    Globals.UserTeamRetries += 1;
                    GetUserTeamDataJson(userTeamId, userTeamIds, userTeamUrl, userTeamsUpdateInsert, userTeamCupInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, userTeamGameweekHistoriesInsert, userTeamPicksInsert, userTeamPickAutomaticSubsInsert, userTeamChipsInsert, userTeamTransferHistoriesInsert, userTeamSeasonsInsert, db);
                }
                else
                {
                    Logger.Error("GetUserTeamDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): Issue processing User Team skipping to next!!!");
                }
            }
        }
    }
}