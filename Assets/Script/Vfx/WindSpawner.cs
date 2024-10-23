using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] spawnPos;
    [SerializeField] private GameObject[] windPrefab;
    [SerializeField] private int maxPrefabToSpawn = 5;
    private float nextSpawnTime = 0f;
    // Start is called before the first frame update
    private void Update()
    {
        Spawn();
    }

    private void Spawn()
    {
        if (Time.time >= nextSpawnTime)
        {
            StartCoroutine(WindSpawn());  // Gọi hàm spawn prefab
            nextSpawnTime = Time.time + Random.Range(6f, 10f);  // Tính toán thời gian cho lần spawn tiếp theo
        }
    }

    private IEnumerator WindSpawn()
    {
        // Số lượng windPrefab ngẫu nhiên để spawn trong lần này (1 đến maxPrefabToSpawn)
        int numberOfWindsToSpawn = Random.Range(1, maxPrefabToSpawn + 1);

        // Tạo danh sách các vị trí đã chọn để đảm bảo không spawn nhiều hơn 1 windPrefab ở cùng một vị trí trong cùng một lần
        List<int> chosenPositions = new List<int>();

        // Spawn từng windPrefab
        for (int i = 0; i < numberOfWindsToSpawn; i++)
        {
            // Chọn vị trí spawn ngẫu nhiên chưa được chọn trong lần này
            int randomPosIndex;

            do
            {
                randomPosIndex = Random.Range(0, spawnPos.Length);
            } while (chosenPositions.Contains(randomPosIndex)); // Đảm bảo vị trí chưa được chọn trong lần này

            chosenPositions.Add(randomPosIndex); // Lưu vị trí đã chọn

            // Chọn prefab ngẫu nhiên để spawn
            int randomPrefabIndex = Random.Range(0, windPrefab.Length);

            Vector3 randomSpawnPosition = spawnPos[randomPosIndex].transform.position;
            randomSpawnPosition.x += Random.Range(-6f, 6f); // Random từ -6 đến 6 cho trục x
            randomSpawnPosition.y += Random.Range(1f, 3f);  // Random từ 1 đến 3 cho trục y
            // Instantiate windPrefab tại vị trí đã chọn
            Instantiate(windPrefab[randomPrefabIndex], randomSpawnPosition, Quaternion.Euler(0, 180, 0));

            // Tạo khoảng thời gian trễ ngẫu nhiên giữa mỗi lần spawn
            float randomDelay = Random.Range(0.2f, 1f);
            yield return new WaitForSeconds(randomDelay); // Đợi một khoảng thời gian ngẫu nhiên
        }
    }
}

