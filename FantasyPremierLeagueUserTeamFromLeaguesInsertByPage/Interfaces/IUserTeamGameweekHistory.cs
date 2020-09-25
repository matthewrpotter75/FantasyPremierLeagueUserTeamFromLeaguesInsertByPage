using System.Data.SqlClient;

namespace FantasyPremierLeagueUserTeams
{
    public interface IUserTeamGameweekHistory
    {
        int InsertUserTeamGameweekHistories(UserTeamGameweekHistories userTeamGameweekHistories, SqlConnection db);
        bool UpdateUserTeamGameweekHistory(UserTeamGameweekHistory userTeamGameweekHistory, SqlConnection db);
        bool DeleteUserTeamGameweekHistory(int userTeamGameweekHistoryId, SqlConnection db);
    }
}
