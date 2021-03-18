﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyPremierLeagueUserTeams
{
    public class Globals
    {
        public static int LatestGameweek { get; set; }
        public static int ActualGameweek { get; set; }
        public static int ApiPageCalls { get; set; }
        public static int ApiCalls { get; set; }
        public static int ApiUserTeamCalls { get; set; }
        public static int ApiUserTeamHistoryCalls { get; set; }
        public static int ApiUserTeamPickCalls { get; set; }
        public static int ApiUserTeamTransferHistoryCalls { get; set; }
        public static int StartGameweekId { get; set; }
        public static int LeagueCountFromUserTeamClassicLeagueForUserTeamId { get; set; }
        public static int SeasonCountFromUserTeamSeasonForUserTeamId { get; set; }
        public static int MaxGWFromGameweekHistoryForUserTeamId { get; set; }
        public static int MaxGWFromPicksForUserTeamId { get; set; }
        public static int MaxGWFromTransferHistoryForUserTeamId { get; set; }
        public static int ExistingUserTeamId { get; set; }
        public static int UserTeamInsertCount { get; set; }
        public static int PageId { get; set; }
        public static int LeagueRetries { get; set; }
        public static int UserTeamRetries { get; set; }
        public static UserTeamCupMatches UserTeamCupInsert { get; set; }
    }
}
