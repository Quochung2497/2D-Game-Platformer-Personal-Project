using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour
{
    //public GameObject BackGround1,BackGround2;
    [SerializeField] fading Door;
    public GameObject canvasUI;

    public float liftHeight = 3f;
    public float liftDuration = 3f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canvasUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(BossTransition());
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        canvasUI.SetActive(false);
    }
    IEnumerator BossTransition()
    {
        PlayerController.Instance.Interact();
        //BackGround1.SetActive(false);
        //BackGround2.SetActive(false);

         fading[] fadingObjects = FindObjectsOfType<fading>();

        foreach (fading fadingObject in fadingObjects)
        {
            if (fadingObject != null)
            {
                // Bắt đầu coroutine StartFading() trên từng đối tượng fading
                StartCoroutine(fadingObject.StartFading());
            }
        }
        yield return StartCoroutine(LiftObject());
        PlayerController.Instance.WalkIntoDoor();
        yield return new WaitForSeconds(1);
    }
    IEnumerator LiftObject()
    {
        Vector2 startPosition = transform.position;
        Vector2 targetPosition = new Vector2(transform.position.x, transform.position.y + liftHeight);
        float elapsedTime = 0;

        while (elapsedTime < liftDuration)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / liftDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
}
