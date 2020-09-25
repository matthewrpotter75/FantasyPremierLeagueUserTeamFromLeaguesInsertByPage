using System.Data.SqlClient;

namespace FantasyPremierLeagueUserTeams
{
    public interface IUserTeamH2hLeague
    {
        int InsertUserTeamH2hLeague(UserTeamH2hLeagues h2hleagues, SqlConnection db);
        bool UpdateUserTeamH2hLeague(UserTeamH2hLeague h2hleague, SqlConnection db);
        bool DeleteUserTeamH2hLeague(int userTeamid, int h2hLeagueid, SqlConnection db);
    }
}