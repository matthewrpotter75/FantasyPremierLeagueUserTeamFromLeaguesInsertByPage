namespace FantasyPremierLeagueUserTeams
{
    public interface IUserTeamCupTiebreak
    {
        bool InsertUserTeamCupTiebreak(UserTeamCupTiebreak cupTiebreak);
        bool UpdateUserTeamCupTiebreak(UserTeamCupTiebreak cupTiebreak);
        bool DeleteUserTeamCupTiebreak(int cupid);
    }
}