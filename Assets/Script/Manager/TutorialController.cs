using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private GameObject WASD;
    private GameObject currentPanel;
    // Start is called before the first frame update
    void Start()
    {
        OpenPanel(WASD);
    }

    // Update is called once per frame
    void Update()
    {
        ClosePopWASD();
    }
    private void ClosePopWASD()
    {
        if(currentPanel.activeInHierarchy)
        {
            if (currentPanel == WASD)
            {
                PlayerController player = PlayerController.Instance;
                if (player.rb.velocity.x != 0 || player.rb.velocity.y != 0)
                {
                    StartCoroutine(Close(currentPanel));
                }
            }
            else
            {
                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    StartCoroutine(Close(currentPanel));
                }
            }
        }

    }
    private IEnumerator Close(GameObject currentPanel)
    {
        Animator anim = currentPanel.GetComponent<Animator>();
        anim.SetTrigger("MoveOut");
        yield return new WaitForSeconds(2f);
        currentPanel.SetActive(false);
    }
    public void OpenPanel(GameObject panel)
    {
        currentPanel = panel;
        currentPanel.SetActive(true);
    }
    public void ClosePanel()
    {
        StartCoroutine(Close(currentPanel));
    }
}
