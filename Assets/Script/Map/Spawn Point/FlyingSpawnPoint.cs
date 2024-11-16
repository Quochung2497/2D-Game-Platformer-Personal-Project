using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingSpawnPoint : SpawnPointBase
{
    private void Awake()
    {
        enemyPool = FlyingEnemyPool.Instance.GetComponent<FlyingEnemyPool>();
    }
}
