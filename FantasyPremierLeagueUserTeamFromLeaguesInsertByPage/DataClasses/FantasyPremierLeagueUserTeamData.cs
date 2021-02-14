using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
//using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace FantasyPremierLeagueUserTeams
{
    [DataContract]
    //public class UserTeamEntryData
    //{
    //    [DataMember]
    //    public UserTeam Entry { get; set; }
    //}

    public class UserTeamGameweekHistoryData
    {
        [DataMember]
        public List<UserTeamGameweekHistory> current { get; set; }
        [DataMember]
        public List<UserTeamSeason> past { get; set; }
        [DataMember]
        public List<UserTeamChip> chips { get; set; }
    }

    public class UserTeamPickData
    {
        public string active_chip { get; set; }
        public List<UserTeamPickAutomaticSub> automatic_subs { get; set; }
        public UserTeamGameweekHistory entry_history { get; set; }
        public List<UserTeamPick> picks { get; set; }
    }

    public class UserTeam
    {
        [ExplicitKey]
        public int id { get; set; }
        public DateTime joined_time { get; set; }
        public int started_event { get; set; }
        public int? favourite_team { get; set; }
        public string player_first_name { get; set; }
        public string player_last_name { get; set; }
        public int? player_region_id { get; set; }
        public string player_region_name { get; set; }
        public string player_region_iso_code_short { get; set; }
        public string player_region_iso_code_long { get; set; }
        public int? summary_overall_points { get; set; }
        public int? summary_overall_rank { get; set; }
        public int? summary_event_points { get; set; }
        public int? summary_event_rank { get; set; }
        public int? current_event { get; set; }
        public Leagues leagues { get; set; }
        public string name { get; set; }
        public string kit { get; set; }
        public int? last_deadline_bank { get; set; }
        public int? last_deadline_value { get; set; }
        public int? last_deadline_total_transfers { get; set; }
    }

    public class UserTeams : List<UserTeam> { }

    public class Leagues
    {
        public List<UserTeamClassicLeague> classic { get; set; }
        public List<UserTeamH2hLeague> h2h { get; set; }
        public UserTeamCup cup { get; set; }
        //public List<UserTeamCup> cup { get; set; }
    }

    public class UserTeamClassicLeagues : List<UserTeamLeague> { }
    public class UserTeamH2hLeagues : List<UserTeamLeague> { }
    public class UserTeamCupMatches : List<UserTeamCupMatch> { }

    public class UserTeamClassicLeague : UserTeamLeague { };

    public class UserTeamH2hLeague : UserTeamLeague { };

    public class UserTeamLeague
    {
        [ExplicitKey]
        public int id { get; set; }
        public string name { get; set; }
        public string short_name { get; set; }
        public DateTime created { get; set; }
        public bool closed { get; set; }
        public int? rank { get; set; }
        public int? max_entries { get; set; }
        public string league_type { get; set; }
        public string scoring { get; set; }
        public int? admin_entry { get; set; }
        public int start_event { get; set; }
        public int? entry_rank { get; set; }
        public int? entry_last_rank { get; set; }
        public bool entry_can_leave { get; set; }
        public bool entry_can_admin { get; set; }
        public bool entry_can_invite { get; set; }
        public int userteamid { get; set; }
    }

    public class UserTeamCup
    {
        public List<UserTeamCupMatch> matches { get; set; }
        public UserTeamCupStatus status { get; set; }
        public int? cup_league { get; set; }
    }

    public class UserTeamCupMatch
    {
        [ExplicitKey]
        public int id { get; set; }
        public int entry_1_entry { get; set; }
        public string entry_1_name { get; set; }
        public string entry_1_player_name { get; set; }
        public int entry_1_points { get; set; }
        public int entry_1_win { get; set; }
        public int entry_1_draw { get; set; }
        public int entry_1_loss { get; set; }
        public int entry_1_total { get; set; }
        public int entry_2_entry { get; set; }
        public string entry_2_name { get; set; }
        public string entry_2_player_name { get; set; }
        public int entry_2_points { get; set; }
        public int entry_2_win { get; set; }
        public int entry_2_draw { get; set; }
        public int entry_2_loss { get; set; }
        public int entry_2_total { get; set; }
        public bool is_knockout { get; set; }
        public int? winner { get; set; }
        public int? seed_value { get; set; }
        public int @event { get; set; }
        public string tiebreak { get; set; }
        //[ExplicitKey]
        public int fromuserteamid { get; set; }
    }

    public class UserTeamCupStatus
    {
        public int? qualification_event { get; set; }
        public int? qualification_numbers { get; set; }
        public int? qualification_rank { get; set; }
        public string qualification_state { get; set; }
    }

    public class UserTeamCupTiebreak
    {
        public int userteamcuptiebreakid { get; set; }
        public int userteamcupid { get; set; }
        public int userteamid { get; set; }
        public int gameweekid { get; set; }
        public string name { get; set; }
        public string choice { get; set; }
    }

    public class UserTeamChip
    {
        //[ExplicitKey]
        public int userteamid { get; set; }
        public string name { get; set; }
        public DateTime time { get; set; }
        //[ExplicitKey]
        public int @event { get; set; }
        public int chip { get; set; }
    }

    public class UserTeamChips : List<UserTeamChip> { }

    //public class UserTeamChipId
    //{
    //    public int userteamid { get; set; }
    //    public int gameweekid { get; set; }
    //    public int chipid { get; set; }
    //}

    public class UserTeamSeason
    {
        [ExplicitKey]
        //public int id { get; set; }
        public string season_name { get; set; }
        public int total_points { get; set; }
        public int rank { get; set; }
        //public int season { get; set; }
        public int userteamid { get; set; }
    }

    public class UserTeamSeasons : List<UserTeamSeason> { }

    public class UserTeamGameweekHistory
    {
        [ExplicitKey]
        public int id { get; set; }
        public int userteamid { get; set; }
        public int @event { get; set; }
        public int points { get; set; }
        public int total_points { get; set; }
        public int? rank { get; set; }
        public int? rank_sort { get; set; }
        public int? overall_rank { get; set; }
        public int bank { get; set; }
        public int value { get; set; }
        public int event_transfers { get; set; }
        public int event_transfers_cost { get; set; }
        public int points_on_bench { get; set; }
    }

    public class UserTeamGameweekHistories : List<UserTeamGameweekHistory> { }

    public class UserTeamWildcard
    {
        public string played_time_formatted { get; set; }
        public string status { get; set; }
        public string name { get; set; }
        public DateTime time { get; set; }
        public int chip { get; set; }
        [ExplicitKey]
        public int entry { get; set; }
        [ExplicitKey]
        public int @event { get; set; }
    }

    //public class UserTeamTransferHistoryData
    //{
    //    public List<UserTeamTransferHistory> UserTeamTransferHistory { get; set; }
    //}

    public class UserTeamTransferHistory
    {
        //[ExplicitKey]
        //public int id { get; set; }
        public int element_in { get; set; }
        public int element_in_cost { get; set; }
        public int element_out { get; set; }
        public int element_out_cost { get; set; }
        public int entry { get; set; }
        public int @event { get; set; }
        public DateTime time { get; set; }
        public long userteamtransferhistoryid { get; set; }
    }

    public class UserTeamTransferHistoryData : List<UserTeamTransferHistory> { }

    //public class UserTeamGameweek
    //{
    //    [ExplicitKey]
    //    public int id { get; set; }
    //    public string name { get; set; }
    //    public DateTime deadline_time { get; set; }
    //    public int average_entry_score { get; set; }
    //    public bool finished { get; set; }
    //    public bool data_checked { get; set; }
    //    public int highest_scoring_entry { get; set; }
    //    public int deadline_time_epoch { get; set; }
    //    public int deadline_time_game_offset { get; set; }
    //    public string deadline_time_formatted { get; set; }
    //    public int highest_score { get; set; }
    //    public bool is_previous { get; set; }
    //    public bool is_current { get; set; }
    //    public bool is_next { get; set; }
    //}

    public class UserTeamPick
    {
        public int element { get; set; }
        public int position { get; set; }
        public int multiplier { get; set; }
        public bool is_captain { get; set; }
        public bool is_vice_captain { get; set; }
        public int userteamid { get; set; }
        public int gameweekid { get; set; }
        public UserTeamPickId userteampickid { get; set; }
    }

    public class UserTeamPicks : List<UserTeamPick> { }

    //public class UserTeamPickId
    //{
    //    public int userteamid { get; set; }
    //    public int gameweekid { get; set; }
    //}

    public class UserTeamPickAutomaticSub
    {
        [ExplicitKey]
        public int id { get; set; }
        public int entry { get; set; }
        public int element_in { get; set; }
        public int element_out { get; set; }
        public int @event { get; set; }
    }

    public class UserTeamPickAutomaticSubs : List<UserTeamPickAutomaticSub> { }

}