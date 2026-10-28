using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingSpawnPoint : SpawnPointBase
{
    private void Start()
    {
        enemyPool = FlyingEnemyPool.Instance.GetComponent<FlyingEnemyPool>();
    }
}
