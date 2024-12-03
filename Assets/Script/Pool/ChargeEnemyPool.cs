using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeEnemyPool : EnemyPoolBase
{
    [SerializeField] private GameObject ChargeEnemyPrefab;

    public static ChargeEnemyPool Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        GrowPool();
        DontDestroyOnLoad(gameObject);
    }

    protected override GameObject CreateEnemy()
    {
        return Instantiate(ChargeEnemyPrefab);
    }
}
