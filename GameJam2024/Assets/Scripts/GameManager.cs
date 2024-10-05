using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Player player;

    public List<Enemy> enemies;

    void Awake()
    {
        Instance = this;
    }
}