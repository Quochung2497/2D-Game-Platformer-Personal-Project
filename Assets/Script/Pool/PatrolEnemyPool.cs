using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemyPool : EnemyPoolBase
{
    [SerializeField] private GameObject PatrolEnemyPrefab;

    public static PatrolEnemyPool Instance;

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
        return Instantiate(PatrolEnemyPrefab);
    }
}
