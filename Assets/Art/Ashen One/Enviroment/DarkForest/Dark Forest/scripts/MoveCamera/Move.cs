using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AEA
{
    public class Move : MonoBehaviour
    {
        [SerializeField] private float _speed = 5f;

        // Update is called once per frame

        void Update()
        {
            MoveCamera();
        }


        private void MoveCamera()
        {
            if (Input.GetKey(KeyCode.A) && transform.position.x > 0f)
            {
                transform.Translate(new Vector2(-_speed * Time.deltaTime, 0));
            }
            else if (Input.GetKey(KeyCode.D) && transform.position.x < 36f)
            {
                transform.Translate(new Vector2(_speed * Time.deltaTime, 0));

            }
            else
            {
                return;
            }
        }
    }
}
