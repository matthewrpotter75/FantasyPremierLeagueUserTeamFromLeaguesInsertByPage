using DapperExtensions;
using DapperExtensions.Mapper;

namespace FantasyPremierLeagueUserTeams
{
    public class UserTeamCupMapper : ClassMapper<UserTeamCupMatch>
    {
        public UserTeamCupMapper()
        {
            //use different table name
            Table("UserTeamCup");

            //Use a different name property from database column
            Map(x => x.@event).Column("gameweekid");

            Map(x => x.entry_1_entry).Column("homeTeam_userTeamid");
            Map(x => x.entry_1_name).Column("homeTeam_userTeamName");
            Map(x => x.entry_1_player_name).Column("homeTeam_playerName");
            Map(x => x.entry_1_points).Column("homeTeam_points");
            Map(x => x.entry_1_win).Column("homeTeam_win");
            Map(x => x.entry_1_draw).Column("homeTeam_draw");
            Map(x => x.entry_1_loss).Column("homeTeam_loss");
            Map(x => x.entry_1_total).Column("homeTeam_total");

            Map(x => x.entry_2_entry).Column("awayTeam_userTeamid");
            Map(x => x.entry_2_name).Column("awayTeam_userTeamName");
            Map(x => x.entry_2_player_name).Column("awayTeam_playerName");
            Map(x => x.entry_2_points).Column("awayTeam_points");
            Map(x => x.entry_2_win).Column("awayTeam_win");
            Map(x => x.entry_2_draw).Column("awayTeam_draw");
            Map(x => x.entry_2_loss).Column("awayTeam_loss");
            Map(x => x.entry_2_total).Column("awayTeam_total");

            Map(x => x.league).Column("league");

            Map(x => x.id).Key(KeyType.Assigned);

            //Ignore this property entirely
            //Map(x => x.is_bye).Ignore();
            //Map(x => x.knockout_name).Ignore();
            Map(x => x.tiebreak).Ignore();

            //optional, map all other columns
            AutoMap();
        }
    }
}