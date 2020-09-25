using System.Data.SqlClient;

namespace FantasyPremierLeagueUserTeams
{
    public interface IUserTeamCup
    {
        bool InsertUserTeamCup(UserTeamCupMatch cup, SqlConnection db);
        bool UpdateUserTeamCup(UserTeamCupMatch cup, SqlConnection db);
        bool DeleteUserTeamCup(int cupid, SqlConnection db);
    }
}