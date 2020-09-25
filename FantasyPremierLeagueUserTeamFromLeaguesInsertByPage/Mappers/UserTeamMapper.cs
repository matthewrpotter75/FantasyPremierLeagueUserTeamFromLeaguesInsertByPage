using DapperExtensions;
using DapperExtensions.Mapper;

namespace FantasyPremierLeagueUserTeams
{
    public static class UserTeamMapping
    {
            public class UserTeamMapper : ClassMapper<UserTeam>
            {
                public UserTeamMapper()
                {
                    //use different table name
                    Table("UserTeam");

                    //Use a different name property from database column
                    Map(x => x.name).Column("team_name");
                    Map(x => x.player_region_iso_code_long).Column("player_region_iso_code");
                    Map(x => x.summary_event_points).Column("summary_gameweek_points");
                    Map(x => x.summary_event_rank).Column("summary_gameweek_rank");
                    Map(x => x.current_event).Column("current_gameweekId");
                    Map(x => x.last_deadline_bank).Column("team_bank");
                    Map(x => x.last_deadline_value).Column("team_value");
                    Map(x => x.last_deadline_total_transfers).Column("team_transfers");
                    Map(x => x.favourite_team).Column("favourite_teamId");
                    Map(x => x.started_event).Column("started_gameweekId");

                    Map(x => x.id).Key(KeyType.Assigned);

                    //Ignore this property entirely
                    //Map(x => x.userTeamId).Ignore();
                    Map(x => x.leagues).Ignore();
                    Map(x => x.player_region_iso_code_short).Ignore();

                    //optional, map all other columns
                    AutoMap();
                }
            }
        }

}
