using System.Data.SqlClient;

namespace FantasyPremierLeagueUserTeams
{
    public interface IUserTeamCup
    {
        int InsertUserTeamCup(UserTeamCupMatches userTeamCupMatches, SqlConnection db);
        bool UpdateUserTeamCup(UserTeamCupMatch cup, SqlConnection db);
        bool DeleteUserTeamCup(int cupid, SqlConnection db);
    }
}