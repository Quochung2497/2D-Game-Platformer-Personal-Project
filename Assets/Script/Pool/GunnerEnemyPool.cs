using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunnerEnemyPool : EnemyPoolBase
{
    [SerializeField] private GameObject GunnerEnemyPrefab;

    public static GunnerEnemyPool Instance;

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
        return Instantiate(GunnerEnemyPrefab);
    }
}
