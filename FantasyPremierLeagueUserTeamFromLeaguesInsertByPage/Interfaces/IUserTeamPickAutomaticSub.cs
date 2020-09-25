using System.Data.SqlClient;

namespace FantasyPremierLeagueUserTeams
{
    public interface IUserTeamPickAutomaticSub
    {
        int InsertUserTeamPickAutomaticSubs(UserTeamPickAutomaticSubs userTeamPickAutomaticSubs, SqlConnection db);
        bool UpdateUserTeamPickAutomaticSub(UserTeamPickAutomaticSub userTeamPickAutomaticSub, SqlConnection db);
        bool DeleteUserTeamPickAutomaticSub(int userTeamPickId, SqlConnection db);
    }
}
