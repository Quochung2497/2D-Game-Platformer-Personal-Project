using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private TutorialController controller;
    [SerializeField] private GameObject canvasUI;
    [SerializeField] private GameObject[] Panel;

    private int counted = 0;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponentInParent<TutorialController>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canvasUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                PanelToOpen();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canvasUI.SetActive(false);
        }
    }

    private void PanelToOpen()
    {
        controller.OpenPanel(Panel[0]);
        counted++;
    }
}
