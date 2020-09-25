using DapperExtensions;
using DapperExtensions.Mapper;

namespace FantasyPremierLeagueUserTeams
{
    public static class UserTeamWildcardMapping
    {
        public class UserTeamWildcardMapper : ClassMapper<UserTeamWildcard>
        {
            public UserTeamWildcardMapper()
            {
                //use different table name
                Table("UserTeamWildcard");

                //Use a different name property from database column
                Map(x => x.played_time_formatted).Column("wildcard_played_time_formatted");
                Map(x => x.name).Column("chip_name");
                Map(x => x.status).Column("chip_status");
                Map(x => x.time).Column("chip_time");
                Map(x => x.chip).Column("chipid");
                Map(x => x.entry).Column("userTeamId");
                Map(x => x.@event).Column("gameweekId");

                //Map(x => x.id).Key(KeyType.Assigned);

                //Ignore this property entirely
                //Map(x => x.userTeamId).Ignore();

                //optional, map all other columns
                AutoMap();
            }
        }
    }

}