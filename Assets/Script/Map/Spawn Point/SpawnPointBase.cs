using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnPointBase : MonoBehaviour
{
    /*
    [SerializeField] protected Transform[] spawnLocations;
    protected List<Vector3> lastSpawnedPositions = new List<Vector3>();
    protected abstract void SpawnEnemyAtLocation(Transform spawnLocation);

    protected virtual void SpawnAllEnemies()
    {
        foreach (Transform spawnLocation in spawnLocations)
        {
            SpawnEnemyAtLocation(spawnLocation);
        }
    }
    protected abstract void RespawnEnemies();
    protected abstract void SaveAndDespawnAllEnemies();*/
    [SerializeField] protected Transform[] spawnLocations;
    [SerializeField] protected EnemyPoolBase enemyPool;

    protected List<Vector3> lastSpawnedPositions = new List<Vector3>();
    protected List<GameObject> lastSpawnedEnemies = new List<GameObject>();
    private bool isFirstEntry = true;

    protected virtual void SpawnAllEnemies()
    {
        foreach (Transform spawnLocation in spawnLocations)
        {
            SpawnEnemyAtLocation(spawnLocation);
        }
    }

    protected virtual void RespawnEnemies()
    {
        foreach (Vector3 position in lastSpawnedPositions)
        {
            GameObject enemy = enemyPool.GetEnemyFromPool();
            enemy.transform.position = position;
        }
    }

    protected virtual void SaveAndDespawnAllEnemies()
    {
        lastSpawnedPositions.Clear();

        foreach (GameObject enemy in enemyPool.pool)
        {
            if (enemy.activeInHierarchy)
            {
                lastSpawnedPositions.Add(enemy.transform.position);
            }
        }
        enemyPool.ReturnAllEnemiesToPool();
    }

    protected virtual void SpawnEnemyAtLocation(Transform spawnLocation)
    {
        GameObject enemy = enemyPool.GetEnemyFromPool();
        enemy.transform.position = spawnLocation.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (isFirstEntry)
            {
                SpawnAllEnemies();
                isFirstEntry = false;
            }
            else
            {
                RespawnEnemies();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SaveAndDespawnAllEnemies();
        }
    }
}
