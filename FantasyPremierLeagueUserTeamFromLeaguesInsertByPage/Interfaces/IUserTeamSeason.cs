using System.Data.SqlClient;

namespace FantasyPremierLeagueUserTeams
{
    public interface IUserTeamSeason
    {
        int InsertUserTeamSeason(UserTeamSeasons userTeamSeasons, SqlConnection db);
        bool UpdateUserTeamSeason(UserTeamSeason userTeamSeason, SqlConnection db);
        bool DeleteUserTeamSeason(int userTeamSeasonId, SqlConnection db);
    }
}
