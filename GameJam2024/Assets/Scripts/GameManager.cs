using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CharacterMovement player;

    public List<Enemy> enemies;

    public Animator playerAnimator;

    [SerializeField] private EnemyManager enemyManager;

    public float score = 0;

    void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (enemyManager != null)
        {

        }
    }
}
