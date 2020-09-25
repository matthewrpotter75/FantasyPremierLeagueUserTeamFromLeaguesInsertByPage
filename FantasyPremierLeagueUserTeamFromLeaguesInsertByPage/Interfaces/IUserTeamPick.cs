using System.Data.SqlClient;

namespace FantasyPremierLeagueUserTeams
{
    public interface IUserTeamPick
    {
        int InsertUserTeamPick(UserTeamPicks userTeamPicks, SqlConnection db);
        bool UpdateUserTeamPick(UserTeamPick userTeamPick, SqlConnection db);
        bool DeleteUserTeamPick(int userTeamPicksId, SqlConnection db);
    }
}
