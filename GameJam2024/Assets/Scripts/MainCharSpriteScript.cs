using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharSpriteScript : MonoBehaviour
{
    public void DamageEnemyTrigger(float time)
    {
        // Player anim damage enemy
        GameManager.Instance.player.DamageEnemyTrigger(time);
    }
}
