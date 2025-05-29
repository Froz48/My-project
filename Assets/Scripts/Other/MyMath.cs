
using Unity.Netcode;
using UnityEngine;

public static class MyMath{
    public static float GetDistanceToNearestPlayer(Vector2 pos){
        return Vector2.Distance(pos, GetNearestPlayer(pos).transform.position);    
    }

    public static Player GetNearestPlayer(Vector2 pos){
        float minDistance = 500000;
        Player nearestPlayer = null;
        foreach (var player in NetworkManager.Singleton.ConnectedClientsList){
            float distance = Vector2.Distance(player.PlayerObject.transform.position, pos);
            if (distance < minDistance){
                minDistance = distance;
                nearestPlayer = player.PlayerObject.GetComponent<Player>();
            }
        }
        return nearestPlayer;
    }
}