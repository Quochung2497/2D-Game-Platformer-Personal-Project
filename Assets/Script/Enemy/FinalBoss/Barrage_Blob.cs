using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrage_Blob : MonoBehaviour
{
    [SerializeField] Vector2 startForceMinMax;
    [SerializeField] float turnSpeed = 0.5f;
    public GameObject hitGroundsfx;

    Rigidbody2D finalbossRb;
    SpriteRenderer sr;
    CircleCollider2D circle;
    public LayerMask excludeLayers;
    // Start is called before the first frame update
    void Start()
    {
        finalbossRb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        circle = GetComponent<CircleCollider2D>();
        excludeLayers = LayerMask.GetMask("Player");
        Destroy(gameObject, 6f);
        finalbossRb.AddForce(transform.right * Random.Range(startForceMinMax.x, startForceMinMax.y), ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        var _dir = finalbossRb.velocity;

        if (_dir != Vector2.zero)
        {
            Vector3 _frontVector = Vector3.right;

            Quaternion _targetRotation = Quaternion.FromToRotation(_frontVector, _dir - (Vector2)transform.position);
            if (_dir.x > 0)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, turnSpeed);
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, turnSpeed);
            }

        }
    }
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.tag == "Player" && !PlayerController.Instance.pState.Invincible && !PlayerController.Instance.pState.cutscenes)
        {
            _other.GetComponent<PlayerController>().TakeDamage(FinalBoss.Instance.damage);
            if (PlayerController.Instance.pState.alive)
            {
                PlayerController.Instance.HitStopTime(0, 5, 0.25f);
            }
            Destroy(gameObject);
        }
        else if(_other.tag == "Ground" || _other.tag == "Wall")
        {

            sr.enabled = false;
            circle.isTrigger = false;
            circle.excludeLayers = excludeLayers;
            StartCoroutine(hitGround());
        }
        else if(_other.tag == "Attackable")
        {
            Destroy(gameObject);
        }
    }

    IEnumerator hitGround()
    {
        finalbossRb.velocity = new Vector3(0, 0, 0);
        hitGroundsfx.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        hitGroundsfx.SetActive(false);
        circle.enabled = false;
    }
}
