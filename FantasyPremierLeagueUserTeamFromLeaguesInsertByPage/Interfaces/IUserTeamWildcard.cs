using System.Data.SqlClient;

namespace FantasyPremierLeagueUserTeams
{
    public interface IUserTeamWildcard
    {
        bool InsertUserTeamWildcard(UserTeamWildcard userTeamChip);
        bool UpdateUserTeamWildcard(UserTeamWildcard userTeamChip);
        bool DeleteUserTeamWildcard(int userTeamId, int chipId);
    }
}
