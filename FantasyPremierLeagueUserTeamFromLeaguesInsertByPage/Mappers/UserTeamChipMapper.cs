using DapperExtensions;
using DapperExtensions.Mapper;
//using Dapper.FluentMap;
//using Dapper.FluentMap.Mapping;
//using Dapper.FluentMap.Dommel.Mapping;

namespace FantasyPremierLeagueUserTeams
{
    public class UserTeamChipMapper : ClassMapper<UserTeamChip>
    {
        public UserTeamChipMapper()
        {
            //use different table name
            Table("UserTeamChip");

            //Use a different name property from database column
            Map(x => x.name).Column("chip_name");
            Map(x => x.time).Column("chip_time");
            Map(x => x.userteamid).Column("userteamid").Key(KeyType.Assigned);
            Map(x => x.@event).Column("gameweekid").Key(KeyType.Assigned);
            Map(x => x.chip).Column("chipid").Key(KeyType.Assigned);

            //Ignore this property entirely
            //Map(x => x.id).Ignore();

            //optional, map all other columns
            AutoMap();
        }
    }

    //public class UserTeamChipMapper : DommelEntityMap<UserTeamChip>
    //{
    //    public UserTeamChipMapper()
    //    {
    //        //use different table name
    //        ToTable("UserTeamChip");

    //        //Use a different name property from database column
    //        Map(x => x.status).ToColumn("chip_status");
    //        Map(x => x.name).ToColumn("chip_name");
    //        Map(x => x.time).ToColumn("chip_time");
    //        Map(x => x.chip).ToColumn("chipid").IsKey();
    //        Map(x => x.entry).ToColumn("userTeamid").IsKey(); ;
    //        Map(x => x.@event).ToColumn("gameweekid").IsKey();

    //        //Map(x => x.entry).IsKey();
    //        //Map(x => x.@event).IsKey();
    //        //Map(x => x.chip).IsKey();

    //        //Map(x => x.entry).Key(KeyType.Assigned);
    //        //Map(x => x.@event).Key(KeyType.Assigned);
    //        //Map(x => x.chip).Key(KeyType.Assigned);

    //        //Ignore this property entirely
    //        //Map(x => x.id).Ignore();

    //        //optional, map all other columns
    //        //AutoMap();
    //    }
    //}
}