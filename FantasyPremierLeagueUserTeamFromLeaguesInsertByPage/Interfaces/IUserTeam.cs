using System.Data.SqlClient;

namespace FantasyPremierLeagueUserTeams
{
    public interface IUserTeam
    {
        bool InsertUserTeam(UserTeam userTeam, SqlConnection db);
        int UpdateUserTeam(UserTeams userTeams, SqlConnection db);
        bool DeleteUserTeam(int userTeamId, SqlConnection db);
    }
}
