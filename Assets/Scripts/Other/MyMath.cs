
using Unity.Netcode;
using UnityEngine;
using System.Linq;

public static class MyMath
{
    public static float GetDistanceToNearestPlayer(Vector2 pos)
    {
        Player nearestPlayer = GetNearestPlayer(pos);
        
        if (nearestPlayer == null)
        {
            return float.MaxValue;
        }
        return Vector2.Distance(pos, nearestPlayer.transform.position);
    }

    public static Player GetNearestPlayer(Vector2 pos)
    {
        float minDistance = float.MaxValue;
        Player nearestPlayer = null;

        var alivePlayers = NetworkManager.Singleton.ConnectedClientsList;
            

        foreach (var client in alivePlayers)
        {
            // if (!client.PlayerObject.GetComponent<Player>().IsAlive()) continue;
            float distance = Vector2.Distance(client.PlayerObject.transform.position, pos);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPlayer = client.PlayerObject.GetComponent<Player>();
            }
        }
        return nearestPlayer;
    }
}