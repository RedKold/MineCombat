using System.Collections;
using System.Collections.Generic;
using MineCombat;
using UnityEngine;

public class SinglePlayerSystem : Singleton<SinglePlayerSystem>
{
    private Player player;

    private Player enemy;

    public bool isSingleGame => true;

    public void initSinglePlayer()
    {
        if (player != null)
        {
            Debug.LogWarning("Player has been inited");
        }
        Debug.Log("Init the player");
        player = new Player("RedKold", 20);
    }

    public void initEnemy()
    {
       if(enemy != null)
        {
            Debug.LogWarning("Enemy hsa been inited");    
        }
        Debug.Log("Init the enemy");
        enemy = new Player("Yukina", 20);
    }

    public Player getPlayer()
    {
        return player;
    }

    public Player getEnemy()
    {
        return enemy;
    }

    protected override void Awake()
    {
        initSinglePlayer();
        initEnemy();
    }

}
