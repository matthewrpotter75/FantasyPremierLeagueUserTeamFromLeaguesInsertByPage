using DapperExtensions;
using DapperExtensions.Mapper;

namespace FantasyPremierLeagueUserTeams
{
    public class UserTeamH2hLeagueMapper : ClassMapper<UserTeamH2hLeague>
    {
        public UserTeamH2hLeagueMapper()
        {
            //use different table name
            Table("UserTeamH2hLeague");

            //Use a different name property from database column
            //Map(x => x.name).Column("league_name");
            //Map(x => x.scoring).Column("scoring");
            //Map(x => x.rank).Column("league_rank");
            //Map(x => x.max_entries).Column("league_size");
            //Map(x => x.admin_entry).Column("admin_userTeamid");
            //Map(x => x.start_event).Column("start_gameweekid");

            Map(x => x.id).Column("leagueid").Key(KeyType.Assigned);
            //Map(x => x.userTeamId).Key(KeyType.Assigned);

            //Ignore this property entirely
            Map(x => x.name).Ignore();
            Map(x => x.short_name).Ignore();
            Map(x => x.created).Ignore();
            Map(x => x.closed).Ignore();
            Map(x => x.rank).Ignore();
            Map(x => x.max_entries).Ignore();
            Map(x => x.league_type).Ignore();
            Map(x => x.scoring).Ignore();
            Map(x => x.start_event).Ignore();
            Map(x => x.admin_entry).Ignore();

            //optional, map all other columns
            AutoMap();
        }
    }
}