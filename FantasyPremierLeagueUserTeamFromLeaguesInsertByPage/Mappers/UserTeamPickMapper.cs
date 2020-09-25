using DapperExtensions;
using DapperExtensions.Mapper;

namespace FantasyPremierLeagueUserTeams
{
    public class UserTeamPickMapper : ClassMapper<UserTeamPick>
    {
        public UserTeamPickMapper()
        {
            //use different table name
            Table("UserTeamPick");

            //Use a different name property from database column
            Map(x => x.element).Column("playerid");

            Map(x => x.userteamid).Key(KeyType.Assigned);
            Map(x => x.gameweekid).Key(KeyType.Assigned);
            Map(x => x.position).Key(KeyType.Assigned);

            //Ignore this property entirely
            Map(x => x.userteampickid).Ignore();

            //optional, map all other columns
            AutoMap();
        }
    }
}
