using System;

namespace FantasyPremierLeagueUserTeams
{
    public class UserTeamClassicLeagueId : IEquatable<UserTeamClassicLeagueId>
    {
        public int userteamid { get; set; }
        public int id { get; set; }

        public UserTeamClassicLeagueId userTeamClassicLeagueId { get; set; }

        public UserTeamClassicLeagueId(int userteamid, int id)
        {
            this.userteamid = userteamid;
            this.id = id;
        }

        //public static implicit operator UserTeamClassicLeagueId(UserTeamClassicLeague d)
        //{
        //    return new UserTeamClassicLeagueId(d);
        //}

        public static bool operator ==(UserTeamClassicLeagueId e1, UserTeamClassicLeagueId e2)
        {
            if (object.ReferenceEquals(e1, e2))
            {
                return true;
            }
            if (object.ReferenceEquals(e1, null) ||
                object.ReferenceEquals(e2, null))
            {
                return false;
            }
            return e1.userTeamClassicLeagueId == e2.userTeamClassicLeagueId;
        }

        public static bool operator !=(UserTeamClassicLeagueId e1, UserTeamClassicLeagueId e2)
        {
            // Delegate...
            return !(e1 == e2);
        }

        public bool Equals(UserTeamClassicLeagueId other)
        {
            bool result = true;
			if (this.userteamid == other.userteamid && this.id == other.id)
			{
				result = true;			
			}
			else
			{
				result = false;
			}
			return result;
        }

        public override int GetHashCode()
        {
            return null == userTeamClassicLeagueId ? 0 : userTeamClassicLeagueId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            // Delegate...
            return Equals(obj as UserTeamClassicLeagueId);
        }
    }
}