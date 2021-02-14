using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyPremierLeagueUserTeams
{
    public class Globals
    {
        public static int latestGameweek { get; set; }
        public static int actualGameweek { get; set; }
        public static int apiPageCalls { get; set; }
        public static int apiCalls { get; set; }
        public static int apiUserTeamCalls { get; set; }
        public static int apiUserTeamHistoryCalls { get; set; }
        public static int apiUserTeamPickCalls { get; set; }
        public static int apiUserTeamTransferHistoryCalls { get; set; }
        public static int startGameweekId { get; set; }
        public static int leagueCountFromUserTeamClassicLeagueForUserTeamId { get; set; }
        public static int maxGWFromGameweekHistoryForUserTeamId { get; set; }
        public static int maxGWFromPicksForUserTeamId { get; set; }
        public static int maxGWFromTransferHistoryForUserTeamId { get; set; }
        public static int existingUserTeamId { get; set; }
        public static int userTeamInsertCount { get; set; }
        public static int pageId { get; set; }
        public static int leagueRetries { get; set; }
        public static int userTeamRetries { get; set; }
        public static UserTeamCupMatches userTeamCupInsert { get; set; }
    }
}
