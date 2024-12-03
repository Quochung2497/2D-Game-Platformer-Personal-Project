using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyPool : EnemyPoolBase
{
    [SerializeField] private GameObject FlyingEnemyPrefab;

    public static FlyingEnemyPool Instance;

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
        return Instantiate(FlyingEnemyPrefab);
    }
}
