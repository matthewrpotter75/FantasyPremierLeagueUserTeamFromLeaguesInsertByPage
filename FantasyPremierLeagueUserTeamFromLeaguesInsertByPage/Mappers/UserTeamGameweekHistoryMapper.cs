using DapperExtensions;
using DapperExtensions.Mapper;

namespace FantasyPremierLeagueUserTeams
{
    public static class UserTeamGameweekHistoryMapping
    {
        public class UserTeamGameweekHistoryMapper : ClassMapper<UserTeamGameweekHistory>
        {
            public UserTeamGameweekHistoryMapper()
            {
                //use different table name
                Table("UserTeamGameweekHistory");

                //Use a different name property from database column
                Map(x => x.userteamid).Column("userteamid");
                Map(x => x.@event).Column("gameweekid");
                Map(x => x.rank).Column("userteam_rank");
                Map(x => x.rank_sort).Column("userteam_rank_sort");
                Map(x => x.overall_rank).Column("userteam_overall_rank");
                Map(x => x.value).Column("userteam_value");
                Map(x => x.bank).Column("userteam_bank");
                Map(x => x.event_transfers).Column("userteam_gameweek_transfers");
                Map(x => x.event_transfers_cost).Column("userteam_gameweek_transfers_cost");
                

                Map(x => x.id).Key(KeyType.Assigned);

                //Ignore this property entirely
                //Map(x => x.userTeamId).Ignore();

                //optional, map all other columns
                AutoMap();
            }
        }
    }

}