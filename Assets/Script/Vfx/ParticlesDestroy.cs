using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesDestroy : MonoBehaviour
{
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlaying();
    }

    private void CheckPlaying()
    {
        if (ps)
        {
            // Kiểm tra xem particle system đã dừng chưa
            if (!ps.isPlaying)
            {
                Destroy(gameObject); // Tự động xóa GameObject sau khi particle dừng
            }
        }
    }
}
