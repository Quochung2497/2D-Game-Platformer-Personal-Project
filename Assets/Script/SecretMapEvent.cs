using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretMapEvent : MonoBehaviour
{
    public GameObject[] SpawnUnlock;

    private Enemy enemy;
    private bool eventTriggered = false;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!eventTriggered && enemy.Event())
        {
            foreach (GameObject obj in SpawnUnlock)
            {
                float randomRotation = Random.Range(0,180);
                Debug.Log($"Instantiating {obj.name} with rotation {randomRotation}");

                // Tạo đối tượng với góc quay ngẫu nhiên
                GameObject spawnedObject = Instantiate(obj, transform.position, Quaternion.identity);

                // Kiểm tra xem đối tượng đã được tạo với góc quay chính xác chưa
                Debug.Log($"Spawned {spawnedObject.name} with actual rotation {spawnedObject.transform.rotation.eulerAngles}");

                // Nếu đối tượng có Rigidbody2D ở chế độ kinematic, thay đổi vị trí trực tiếp
                Rigidbody2D rb = spawnedObject.GetComponent<Rigidbody2D>();
                if (rb != null && rb.isKinematic)
                {
                    float angleInRadians = randomRotation * Mathf.Deg2Rad;
                    Vector2 forceDirection = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
                    Vector2 newPosition = (Vector2)transform.position + forceDirection * 2f; // Điều chỉnh khoảng cách
                    spawnedObject.transform.position = newPosition;
                }
            }
            SecretMapTransition.Instance.Enable();
            eventTriggered = true;
        }
    }
}
