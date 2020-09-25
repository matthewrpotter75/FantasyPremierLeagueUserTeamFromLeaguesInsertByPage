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
        public static int existingUserTeamId { get; set; }
        public static int userTeamInsertCount { get; set; }

        //private static int _latestGameweek;
        //public static int latestGameweek
        //{
        //    get
        //    {
        //        // Reads are usually simple
        //        return _latestGameweek;
        //    }
        //    set
        //    {
        //        // You can add logic here for race conditions,
        //        // or other measurements
        //        _latestGameweek = value;
        //    }
        //}

        //private static int _actualGameweek;
        //public static int actualGameweek
        //{
        //    get
        //    {
        //        // Reads are usually simple
        //        return _actualGameweek;
        //    }
        //    set
        //    {
        //        // You can add logic here for race conditions,
        //        // or other measurements
        //        _actualGameweek = value;
        //    }
        //}
        //// Perhaps extend this to have Read-Modify-Write static methods
        //// for data integrity during concurrency? Situational.

        //private static int _apiPageCalls;
        //public static int apiPageCalls
        //{
        //    get
        //    {
        //        // Reads are usually simple
        //        return _apiPageCalls;
        //    }
        //    set
        //    {
        //        // You can add logic here for race conditions,
        //        // or other measurements
        //        _apiPageCalls = value;
        //    }
        //}


        //private static int _apiCalls;
        //public static int apiCalls
        //{
        //    get
        //    {
        //        // Reads are usually simple
        //        return _apiCalls;
        //    }
        //    set
        //    {
        //        // You can add logic here for race conditions,
        //        // or other measurements
        //        _apiCalls = value;
        //    }
        //}

        //private static int _apiUserTeamCalls;
        //public static int apiUserTeamCalls
        //{
        //    get
        //    {
        //        // Reads are usually simple
        //        return _apiUserTeamCalls;
        //    }
        //    set
        //    {
        //        // You can add logic here for race conditions,
        //        // or other measurements
        //        _apiUserTeamCalls = value;
        //    }
        //}

        //private static int _apiUserTeamHistoryCalls;
        //public static int apiUserTeamHistoryCalls
        //{
        //    get
        //    {
        //        // Reads are usually simple
        //        return _apiUserTeamHistoryCalls;
        //    }
        //    set
        //    {
        //        // You can add logic here for race conditions,
        //        // or other measurements
        //        _apiUserTeamHistoryCalls = value;
        //    }
        //}

        //private static int _apiUserTeamPickCalls;
        //public static int apiUserTeamPickCalls
        //{
        //    get
        //    {
        //        // Reads are usually simple
        //        return _apiUserTeamPickCalls;
        //    }
        //    set
        //    {
        //        // You can add logic here for race conditions,
        //        // or other measurements
        //        _apiUserTeamPickCalls = value;
        //    }
        //}

        //private static int _apiUserTeamTransferHistoryCalls;
        //public static int apiUserTeamTransferHistoryCalls
        //{
        //    get
        //    {
        //        // Reads are usually simple
        //        return _apiUserTeamTransferHistoryCalls;
        //    }
        //    set
        //    {
        //        // You can add logic here for race conditions,
        //        // or other measurements
        //        _apiUserTeamTransferHistoryCalls = value;
        //    }
        //}

        //private static int _startGameweekId;
        //public static int startGameweekId
        //{
        //    get
        //    {
        //        // Reads are usually simple
        //        return _startGameweekId;
        //    }
        //    set
        //    {
        //        // You can add logic here for race conditions,
        //        // or other measurements
        //        _startGameweekId = value;
        //    }
        //}

        //private static int _userTeamInsertCount;
        //public static int userTeamInsertCount
        //{
        //    get
        //    {
        //        // Reads are usually simple
        //        return _userTeamInsertCount;
        //    }
        //    set
        //    {
        //        // You can add logic here for race conditions,
        //        // or other measurements
        //        _userTeamInsertCount = value;
        //    }
        //}
    }
}
