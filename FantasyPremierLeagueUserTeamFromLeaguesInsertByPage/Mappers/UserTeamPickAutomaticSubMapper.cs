using DapperExtensions;
using DapperExtensions.Mapper;

namespace FantasyPremierLeagueUserTeams
{
    public class UserTeamPickAutomaticSubMapper : ClassMapper<UserTeamPickAutomaticSub>
    {
        public UserTeamPickAutomaticSubMapper()
        {
            //use different table name
            Table("UserTeamPickAutomaticSub");

            //Use a different name property from database column
            Map(x => x.element_in).Column("playerid_in");
            Map(x => x.element_out).Column("playerid_out");
            Map(x => x.entry).Column("userteamid");
            Map(x => x.@event).Column("gameweekid");

            Map(x => x.id).Key(KeyType.Assigned);

            //Ignore this property entirely
            //Map(x => x.id).Ignore();

            //optional, map all other columns
            AutoMap();
        }
    }
}
