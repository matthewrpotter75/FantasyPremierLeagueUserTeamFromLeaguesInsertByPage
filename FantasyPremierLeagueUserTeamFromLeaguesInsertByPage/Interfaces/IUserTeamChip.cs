using System.Data.SqlClient;

namespace FantasyPremierLeagueUserTeams
{
    public interface IUserTeamChip
    {
        //bool InsertUserTeamChip(UserTeamChip userTeamChip);
        int InsertUserTeamChip(UserTeamChips userTeamChips, SqlConnection db);
        bool UpdateUserTeamChip(UserTeamChip userTeamChip, SqlConnection db);
        bool DeleteUserTeamChip(int userTeamid, int gameweekid, int chipid, SqlConnection db);
    }
}
