using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    [SerializeField] private float minYSpawn = -4.8f;
    [SerializeField] private float maxYSpawn = 0f;

    [SerializeField] private Enemy[] enemyTypes;

    [SerializeField] private float hSpawnOffset = 20f;

    [SerializeField] private int maxEnemies = 6;

    [SerializeField] private Transform parentTransform;

    

    void Update() {
        int enemiesAlive = GameManager.Instance.enemies.Count;
        if (enemiesAlive < 3 && GameManager.Instance.player.health > 10f) {
            StartCoroutine(SpawnRandomEnemiesNearby(maxEnemies - 1 - enemiesAlive));
        }
        else if (GameManager.Instance.player.health <= 10f && GameManager.Instance.player.health > 0f && enemiesAlive < 2) {
            StartCoroutine(SpawnRandomEnemiesNearby(maxEnemies - 1 - enemiesAlive));
        }
    }

    private IEnumerator SpawnRandomEnemiesNearby(int numToSpawn) {
        for (int i = 0; i < numToSpawn; i++) {
            Enemy enemyToSpawn = enemyTypes[Random.Range(0, enemyTypes.Length)];
            // 50/50 spawn left/right
            Vector2 spawnLocation;
            spawnLocation.x = Random.Range(0, 2) > 0 ? GameManager.Instance.player.transform.position.x - hSpawnOffset : GameManager.Instance.player.transform.position.x + hSpawnOffset;
            spawnLocation.y = Random.Range(minYSpawn, maxYSpawn);

            Enemy newEnemy = Instantiate(enemyToSpawn, spawnLocation, Quaternion.identity, parentTransform);
            GameManager.Instance.enemies.Add(newEnemy);

            yield return new WaitForSeconds(1.5f);
        }
    }
}
