using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public GameObject subMenu;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(button.interactable) subMenu.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(button.interactable) 
        {
            subMenu.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(button.interactable) subMenu.SetActive(!subMenu.activeSelf);
    }
    
    public void Activate(){button.interactable = true;}

    public void DeActivate(){button.interactable = false; subMenu.SetActive(false); EventSystem.current.SetSelectedGameObject(null);}
}
