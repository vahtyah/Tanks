using ExitGames.Client.Photon;

public static class CharacterExtensions
{
    public static void SetPlayerDied(this Character player, bool dead)
    {
        player.PhotonView.Owner.SetCustomProperties(new Hashtable { { GlobalString.PLAYER_DIED, dead } });
    }
    
    public static bool IsDead(this Character player)
    {
        if (!player.PhotonView.Owner.CustomProperties.TryGetValue(GlobalString.PLAYER_DIED, out var isDead))
        {
            return false;
        }

        return (bool)isDead;
    }
    public static TeamType GetTeamType(this Character player)
    {
        var teamType = Team.TryGetCacheTeam(player);
        if (teamType != TeamType.None) return teamType;

        var team = player.PhotonView.Owner.GetTeam();
        teamType = team?.TeamType ?? TeamType.None;
        Team.CacheTeam(player, teamType);

        return teamType;
    }
    
    public static Team GetTeam(this Character player)
    {
        return Team.GetTeamByPlayer(player.PhotonView.Owner);
    }

    public static void AddScore(this Character player, int score)
    {
        player.PhotonView.Owner.SetCustomProperties(new Hashtable
        {
            {
                GlobalString.PLAYER_SCORE,
                (int)player.PhotonView.Owner.CustomProperties[GlobalString.PLAYER_SCORE] + score
            }
        });
    }

    public static int GetScore(this Character player)
    {
        return player.PhotonView.Owner.CustomProperties.TryGetValue(GlobalString.PLAYER_SCORE, out var score)
            ? (int)score
            : 0;
    }

    public static void AddKill(this Character player, int skill)
    {
        player.PhotonView.Owner.SetCustomProperties(new Hashtable
        {
            {
                GlobalString.PLAYER_KILLS,
                (int)player.PhotonView.Owner.CustomProperties[GlobalString.PLAYER_KILLS] + skill
            }
        });
    }

    public static int GetKills(this Character player)
    {
        return player.PhotonView.Owner.CustomProperties.TryGetValue(GlobalString.PLAYER_KILLS, out var score)
            ? (int)score
            : 0;
    }

    public static void AddDeath(this Character player, int death)
    {
        player.PhotonView.Owner.SetCustomProperties(new Hashtable
        {
            {
                GlobalString.PLAYER_DEATHS,
                (int)player.PhotonView.Owner.CustomProperties[GlobalString.PLAYER_DEATHS] + death
            }
        });
    }

    public static int GetDeaths(this Character player)
    {
        return player.PhotonView.Owner.CustomProperties.TryGetValue(GlobalString.PLAYER_DEATHS, out var score)
            ? (int)score
            : 0;
    }
}