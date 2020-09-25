using DapperExtensions;
using DapperExtensions.Mapper;

namespace FantasyPremierLeagueUserTeams
{
    public class UserTeamSeasonMapper : ClassMapper<UserTeamSeason>
    {
        public UserTeamSeasonMapper()
        {
            //use different table name
            Table("UserTeamSeason");

            //Use a different name property from database column
            //Map(x => x.season).Column("seasonid");
            Map(x => x.rank).Column("userteam_rank");

            Map(x => x.userteamid).Key(KeyType.Assigned);
            //Map(x => x.season_name).Key(KeyType.Assigned);

            //Ignore this property entirely
            //Map(x => x.userTeamId).Ignore();

            //optional, map all other columns
            AutoMap();
        }
    }
}
