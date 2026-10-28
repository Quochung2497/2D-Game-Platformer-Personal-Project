using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunnerSpawnPoint : SpawnPointBase
{
    private void Start()
    {
        enemyPool = GunnerEnemyPool.Instance.GetComponent<GunnerEnemyPool>();
    }
}
