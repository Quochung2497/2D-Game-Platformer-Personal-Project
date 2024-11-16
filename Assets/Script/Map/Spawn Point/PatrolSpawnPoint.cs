using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolSpawnPoint : SpawnPointBase
{
    /*
    private bool isFirstEntry = true;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player") && PlayerController.Instance.pState.alive)
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
        if (other.CompareTag("Player") && PlayerController.Instance.pState.alive)
        {
            SaveAndDespawnAllEnemies();
        }
    }

    protected override void SpawnEnemyAtLocation(Transform transform)
    {
        GameObject enemy = PatrolEnemyPool.Instance.GetEnemyFromPool();
        enemy.transform.position = transform.position;
    }
    protected override void RespawnEnemies()
    {
        foreach (Vector3 position in lastSpawnedPositions)
        {
            GameObject enemy = PatrolEnemyPool.Instance.GetEnemyFromPool();
            enemy.transform.position = position;
        }
    }
    protected override void SaveAndDespawnAllEnemies()
    {
        lastSpawnedPositions.Clear(); 

        foreach (GameObject enemy in PatrolEnemyPool.Instance.pool)
        {
            if (enemy.activeInHierarchy)
            {
                lastSpawnedPositions.Add(enemy.transform.position);
            }
        }
        PatrolEnemyPool.Instance.ReturnAllEnemiesToPool();
    }*/
    private void Start()
    {
        enemyPool = PatrolEnemyPool.Instance.GetComponent<PatrolEnemyPool>();
    }
}
