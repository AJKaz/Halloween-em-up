using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    [SerializeField] private float minYSpawn = -4.8f;
    [SerializeField] private float maxYSpawn = 0f;

    [SerializeField] private Enemy[] enemyTypes;

    [SerializeField] private float hSpawnOffset = 20f;

    [SerializeField] private int idealLivingEnemies = 6;
    [SerializeField] private float spawnDelay = 1.0f;

    [SerializeField] private Transform parentTransform;

    public void TrySpawnEnemies() {
        int enemiesAlive = GameManager.Instance.enemies.Count;

        int ranNum = Random.Range(0, idealLivingEnemies + 1);

        if (ranNum <= idealLivingEnemies - enemiesAlive) {
            StartCoroutine(SpawnRandomEnemiesNearby(idealLivingEnemies - enemiesAlive));
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

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
