using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeSpawnPoint : SpawnPointBase
{
    private void Start()
    {
        enemyPool = ChargeEnemyPool.Instance.GetComponent<ChargeEnemyPool>();
    }
}
