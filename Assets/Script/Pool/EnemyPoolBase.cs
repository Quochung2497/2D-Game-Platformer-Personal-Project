using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyPoolBase : MonoBehaviour
{
    [SerializeField] protected int poolSize = 10;
    public List<GameObject> pool{ get; private set; } = new List<GameObject>();
    

    protected abstract GameObject CreateEnemy();

    protected virtual void GrowPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = CreateEnemy();
            enemy.transform.SetParent(transform);
            enemy.SetActive(false);
            pool.Add(enemy);
        }
    }

    public GameObject GetEnemyFromPool()
    {
        foreach (GameObject enemy in pool)
        {
            if (!enemy.activeInHierarchy)
            {
                enemy.SetActive(true);
                return enemy;
            }
        }

        GameObject newEnemy = CreateEnemy();
        newEnemy.SetActive(true);
        pool.Add(newEnemy);
        return newEnemy;
    }
    public void ReturnEnemyToPool(GameObject enemy)
    {
        enemy.SetActive(false);
    }
    public void ReturnAllEnemiesToPool()
    {
        foreach (GameObject enemy in pool)
        {
            enemy.SetActive(false);
        }
    }
}
