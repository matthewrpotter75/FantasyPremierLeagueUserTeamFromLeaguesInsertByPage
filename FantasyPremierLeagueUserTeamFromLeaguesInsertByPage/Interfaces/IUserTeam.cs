using System.Data.SqlClient;

namespace FantasyPremierLeagueUserTeams
{
    public interface IUserTeam
    {
        bool InsertUserTeam(UserTeam userTeam, SqlConnection db);
        bool UpdateUserTeam(UserTeam userTeam, SqlConnection db);
        bool DeleteUserTeam(int userTeamId, SqlConnection db);
    }
}
