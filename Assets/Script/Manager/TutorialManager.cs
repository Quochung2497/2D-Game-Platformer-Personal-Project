using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("")]
    [SerializeField] public GameObject[] Popup;
    private GameObject currentPanel;
    public bool finishTutorial = false;
    // Start is called before the first frame update
    void Start()
    {
        OpenPanel(Popup[0]);
    }

    public void ClosePopup()
    {
        if(currentPanel != null && currentPanel.activeInHierarchy)
        {
            PlayerController player = PlayerController.Instance;
            if (currentPanel == Popup[0])
            {
                if (player.rb.velocity.x != 0 || player.rb.velocity.y != 0)
                {
                    StartCoroutine(Close(currentPanel, Popup[1]));
                }
            }
            else if(currentPanel == Popup[1])
            {
                if(player.attack)
                {
                    StartCoroutine(Close(currentPanel, Popup[2]));
                }
            }
            else if(currentPanel == Popup[2])
            {
                if (player.rb.velocity.y != 0)
                {
                    StartCoroutine(Close(currentPanel));
                    finishTutorial = true;
                }
            }
        }

    }
    private IEnumerator Close(GameObject currentPanel, GameObject nextPanel = null)
    {
        yield return new WaitForSeconds(1f);
        Animator anim = currentPanel.GetComponent<Animator>();
        anim.SetTrigger("MoveOut");
        yield return new WaitForSeconds(2f);
        currentPanel.SetActive(false);
        if (nextPanel != null)
        {
            StartCoroutine(Open(nextPanel));
        }
    }
    private IEnumerator Open(GameObject nextPanel)
    {
        yield return new WaitForSeconds(1f);
        nextPanel.SetActive(true);
        currentPanel = nextPanel;
    }
    public void OpenPanel(GameObject panel)
    {
        if (currentPanel != null && currentPanel.activeInHierarchy)
        {
            currentPanel.SetActive(false);
        }
        currentPanel = panel;
        currentPanel.SetActive(true);
    }
}
