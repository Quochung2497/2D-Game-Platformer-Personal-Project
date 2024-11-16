using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunnerSpawnPoint : SpawnPointBase
{
    private void Awake()
    {
        enemyPool = GunnerEnemyPool.Instance.GetComponent<GunnerEnemyPool>();
    }
}
