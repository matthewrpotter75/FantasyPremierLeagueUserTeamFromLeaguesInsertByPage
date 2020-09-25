using System.Data.SqlClient;

namespace FantasyPremierLeagueUserTeams
{
    public interface IUserTeamTransferHistory
    {
        int InsertUserTeamTransferHistories(UserTeamTransferHistoryData userTeamTransferData, SqlConnection db);
        bool UpdateUserTeamTransferHistory(UserTeamTransferHistory userTeamTransferHistory, SqlConnection db);
        bool DeleteUserTeamTransferHistory(int userTeamTransferHistoryId, SqlConnection db);
    }
}
