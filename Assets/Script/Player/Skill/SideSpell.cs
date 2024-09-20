using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideSpell : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float hitForce;
    [SerializeField] float speed;
    [SerializeField] float lifetime = 1f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        // Di chuyển đạn với tốc độ đã được điều chỉnh theo thời gian thực
        transform.position += (speed * Time.fixedDeltaTime * transform.right);
    }

    // Phát hiện va chạm
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Enemy"))
        {
            // Đảm bảo rằng Enemy có script với phương thức EnemyHit phù hợp
            Enemy enemy = _other.GetComponent<Enemy>();
            if (enemy != null)
            {
                Vector2 hitDirection = (_other.transform.position - transform.position).normalized;
                enemy.EnemyGetsHit(damage, hitDirection, -hitForce);
            }
            Destroy(gameObject); // Hủy đạn sau khi gây sát thương
        }
    }
}
