using UnityEngine;
using System.Collections;

public class AIUtilities
{
    public static GameObject getClosestPlayer(Vector2 position)
    {
        GameObject player = null;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        float minDistance = Mathf.Infinity;

        for (int i = 0; i < players.Length; i++)
        {
            float distance = Vector2.Distance(position, players[i].transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                player = players[i];
            }
        }

        return player;
    }
}
