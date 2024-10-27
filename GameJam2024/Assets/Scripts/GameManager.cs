using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CharacterMovement player;

    public List<Enemy> enemies;

    public Animator playerAnimator;

    [SerializeField] private EnemyManager enemyManager;

    [SerializeField] private TMP_Text candyStolenText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text livesText;

    public int score = 0;
    public float enemiesKilled = 0;


    void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        candyStolenText.text = score.ToString("D7");

        float localHealth = Mathf.Clamp(player.health, 0f, 999f);
        healthText.text = "HP: " + localHealth.ToString();
    }
}
