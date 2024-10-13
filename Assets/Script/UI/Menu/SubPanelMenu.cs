using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SubPanelMenu : MonoBehaviour
{
    public GameObject ControllerMenuEnter, ControllerMenuLeave;
    public void EnterControllerMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(ControllerMenuEnter);
    }

    public void OkeControllerButton()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(ControllerMenuLeave);
        
    }
    public void UnActive(CanvasGroup canvasGroup)
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    public void Active(CanvasGroup canvasGroup)
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
