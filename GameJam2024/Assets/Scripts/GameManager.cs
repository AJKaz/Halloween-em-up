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

    public float score = 0;
    public float enemiesKilled = 0;


    void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (score < 0) score = 0;
        if (score > 99) score = 99;
        candyStolenText.text = score.ToString();

        float localHealth = Mathf.Clamp(player.health, 0f, 999f);
        healthText.text = "HP: " + localHealth.ToString();

        livesText.text = "Lives: " + Mathf.Clamp(player.lives, 0, 9).ToString();
    }
}
