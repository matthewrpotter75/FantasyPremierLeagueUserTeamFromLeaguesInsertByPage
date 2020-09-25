using System;

namespace FantasyPremierLeagueUserTeams
{
    public class UserTeamH2hLeagueId : IEquatable<UserTeamH2hLeagueId>
    {
        public int userteamid { get; set; }
        public int id { get; set; }

        public UserTeamH2hLeagueId userTeamH2hLeagueId { get; set; }

        public UserTeamH2hLeagueId(int userteamid, int id)
        {
            this.userteamid = userteamid;
            this.id = id;
        }

        //public static implicit operator UserTeamH2hLeagueId(UserTeamClassicLeague d)
        //{
        //    return new UserTeamH2hLeagueId(d);
        //}

        public static bool operator ==(UserTeamH2hLeagueId e1, UserTeamH2hLeagueId e2)
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
            return e1.userTeamH2hLeagueId == e2.userTeamH2hLeagueId;
        }

        public static bool operator !=(UserTeamH2hLeagueId e1, UserTeamH2hLeagueId e2)
        {
            // Delegate...
            return !(e1 == e2);
        }

        public bool Equals(UserTeamH2hLeagueId other)
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
            return null == userTeamH2hLeagueId ? 0 : userTeamH2hLeagueId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            // Delegate...
            return Equals(obj as UserTeamH2hLeagueId);
        }
    }
}