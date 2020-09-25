using System;

namespace FantasyPremierLeagueUserTeams
{
    public class UserTeamChipId : IEquatable<UserTeamChipId>
    {
        public int userteamid { get; set; }
        public int gameweekid { get; set; }
        public int chipid { get; set; }

        public UserTeamChipId userTeamChipId { get; set; }

        public UserTeamChipId(int userteamid, int gameweekid, int chipid)
        {
            this.userteamid = userteamid;
            this.gameweekid = gameweekid;
            this.chipid = chipid;
        }

        //public static implicit operator UserTeamChipId(UserTeamChip d)
        //{
        //    return new UserTeamChipId(d);
        //}

        public static bool operator ==(UserTeamChipId e1, UserTeamChipId e2)
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
            return e1.userTeamChipId == e2.userTeamChipId;
        }

        public static bool operator !=(UserTeamChipId e1, UserTeamChipId e2)
        {
            // Delegate...
            return !(e1 == e2);
        }

        public bool Equals(UserTeamChipId other)
        {
            bool result = true;
			if (this.userteamid == other.userteamid && this.gameweekid == other.gameweekid && this.chipid == other.chipid)
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
            return null == userTeamChipId ? 0 : userTeamChipId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            // Delegate...
            return Equals(obj as UserTeamChipId);
        }
    }
}