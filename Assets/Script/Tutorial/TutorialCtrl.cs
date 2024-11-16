using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialCtrl : MonoBehaviour
{
    public TutorialManager tut;
    public TutorialBoard Board;
    [SerializeField] private GameObject nextScene;
    [SerializeField] private GameObject canvasUI;
    [SerializeField] private GameObject Effect;

    private Animator anim;
    private Rigidbody2D rb;
    void Awake()
    {
        tut = GetComponentInParent<TutorialManager>();
        anim = PlayerController.Instance.GetComponent<Animator>();
        rb = PlayerController.Instance.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        tut.ClosePopup();
        if(tut.finishTutorial && !Effect.activeInHierarchy)
        {
            StartCoroutine(StartEffect());
        }
    }

    private IEnumerator StartEffect()
    {
        yield return new WaitForSeconds(4f);
        Effect.SetActive(true);
    }

    public void CheckPopup()
    {
        if(tut.finishTutorial && Effect.activeInHierarchy)
        {
            canvasUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                PlayerController.Instance.Interact();
                if(Vector2.Distance(PlayerController.Instance.transform.position,Effect.transform.position) > 0.3f)
                {
                    StartCoroutine(PortalIn());
                }
            }
        }
    }
    private IEnumerator PortalIn()
    {
        yield return new WaitForSeconds(1f);
        rb.simulated = false;
        anim.SetTrigger("Portalin");
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(MoveInPortal());
        yield return new WaitForSeconds(0.5f);
        nextScene.SetActive(true);
        rb.simulated = true;
    }
    private IEnumerator MoveInPortal()
    {
        float timer = 0;
        while(timer < 0.5f)
        {
            PlayerController.Instance.transform.position = Vector2.MoveTowards(PlayerController.Instance.transform.position, Effect.transform.position, 3 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
    }
    public void ClosepopUP()
    {
        canvasUI.SetActive(false);
    }
}
