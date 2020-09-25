using System.Data.SqlClient;

namespace FantasyPremierLeagueUserTeams
{
    public interface IUserTeamClassicLeague
    {
        int InsertUserTeamClassicLeague(UserTeamClassicLeagues classicleagues, SqlConnection db);
        bool UpdateUserTeamClassicLeague(UserTeamClassicLeague classicleague, SqlConnection db);
        bool DeleteUserTeamClassicLeague(int userTeamid, int classicleagueid, SqlConnection db);
    }
}