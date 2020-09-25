using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyPremierLeagueUserTeams
{

    public class LeagueStandingsData
    {
        public League league { get; set; }
        public NewEntries new_entries { get; set; }
        public Standings standings { get; set; }
    }

    public class NewEntries
    {
        public bool has_next { get; set; }
        public int number { get; set; }
        public List<object> results { get; set; }
    }

    public class League
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime created { get; set; }
        public object max_entries { get; set; }
        public string league_type { get; set; }
        public string _scoring { get; set; }
        public object admin_entry { get; set; }
        public int start_event { get; set; }
        public string code_privacy { get; set; }
        public int? rank { get; set; }
    }

    public class Standings
    {
        public bool has_next { get; set; }
        public int number { get; set; }
        public List<TeamLeaguePosition> results { get; set; }
    }

    public class TeamLeaguePosition
    {
        public int id { get; set; }
        public string entry_name { get; set; }
        public int event_total { get; set; }
        public string player_name { get; set; }
        public string movement { get; set; }
        public bool own_entry { get; set; }
        public int rank { get; set; }
        public int last_rank { get; set; }
        public int rank_sort { get; set; }
        public int total { get; set; }
        public int entry { get; set; }
        public int league { get; set; }
        public int start_event { get; set; }
        public int stop_event { get; set; }
    }
}