using DapperExtensions;
using DapperExtensions.Mapper;

namespace FantasyPremierLeagueUserTeams
{
    public static class UserTeamTransferHistoryMapping
    {
        public class UserTeamTransferHistoryMapper : ClassMapper<UserTeamTransferHistory>
        {
            public UserTeamTransferHistoryMapper()
            {
                //use different table name
                Table("UserTeamTransferHistory");

                //Use a different name property from database column
                Map(x => x.time).Column("transfer_time");
                Map(x => x.element_in_cost).Column("player_in_cost");
                Map(x => x.element_out_cost).Column("player_out_cost");
                Map(x => x.element_in).Column("playerid_in");
                Map(x => x.element_out).Column("playerid_out");
                Map(x => x.entry).Column("uaerTeamid");
                Map(x => x.@event).Column("gameweekid");

                //Map(x => x.id).Key(KeyType.Assigned);

                //Ignore this property entirely
                //Map(x => x.userTeamId).Ignore();

                //optional, map all other columns
                AutoMap();
            }
        }
    }

}