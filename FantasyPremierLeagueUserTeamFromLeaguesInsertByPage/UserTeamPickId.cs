using System;

namespace FantasyPremierLeagueUserTeams
{
    public class UserTeamPickId : IEquatable<UserTeamPickId>
    {
        public int userteamid { get; set; }
        public int gameweekid { get; set; }
        public int position { get; set; }

        public UserTeamPickId userTeamPickId { get; set; }

        public UserTeamPickId(int userteamid, int gameweekid, int position)
        {
            this.userteamid = userteamid;
            this.gameweekid = gameweekid;
            this.position = position;
        }

        //public static implicit operator UserTeamPickId(UserTeamPick d)
        //{
        //    return new UserTeamPickId(d);
        //}

        public static bool operator ==(UserTeamPickId e1, UserTeamPickId e2)
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
            return e1.userTeamPickId == e2.userTeamPickId;
        }

        public static bool operator !=(UserTeamPickId e1, UserTeamPickId e2)
        {
            // Delegate...
            return !(e1 == e2);
        }

        public bool Equals(UserTeamPickId other)
        {
            bool result = true;
			if (this.userteamid == other.userteamid && this.gameweekid == other.gameweekid && this.position == other.position)
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
            return null == userTeamPickId ? 0 : userTeamPickId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            // Delegate...
            return Equals(obj as UserTeamPickId);
        }
    }
}