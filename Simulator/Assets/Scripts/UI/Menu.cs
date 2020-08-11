using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    private MenuButton[] stateButtons;
    private Button[] simulatorButtons;
    private EditMenu[] editMenus;
    public MenuFileNew menuFileNew;
    public MenuFileLoad menuFileLoad;
    public MenuFileSave menuFileSave;
    public TMP_Text timeText;
    public TMP_Text feedbackText;
    public GameObject lockScreen;
    public TMP_Text filenameText;

    void Awake()
    {
        stateButtons = FindObjectsOfType(typeof(MenuButton)) as MenuButton[];
        simulatorButtons = new Button[3];
        simulatorButtons[0] = transform.Find("DownMenu/PlayBtn").GetComponent<Button>();
        simulatorButtons[1] = transform.Find("DownMenu/StopBtn").GetComponent<Button>();
        simulatorButtons[2] = transform.Find("DownMenu/BackBtn").GetComponent<Button>();
        editMenus = FindObjectsOfType(typeof(EditMenu)) as EditMenu[];
        CloseEditMenus();
    }

    // Top menu
    public void EnableMenu(string name_)
    {
        for(int i=0; i<stateButtons.Length; i++)
        {
            if(stateButtons[i].gameObject.name == name_)
            {
                stateButtons[i].Activate();
                break;
            }
        }
    }
    
    public void DisableMenu(string name_)
    {
        for(int i=0; i<stateButtons.Length; i++)
        {
            if(stateButtons[i].gameObject.name == name_)
            {
                stateButtons[i].DeActivate();
                break;
            }
        }
    }

    // Down menu
    public void EnableSimulatorButtons()
    {
        for(int i=0; i<simulatorButtons.Length; i++)
        {
            simulatorButtons[i].interactable = true;
        }
    }

    public void DisableSimulatorButtons()
    {
        for(int i=0; i<simulatorButtons.Length; i++)
        {
            simulatorButtons[i].interactable = false;
        }
    }

    // Editable menus
    public void EnableEditMenu(GameObject element)
    {
        CloseEditMenus();

        EditMenu menu = null;
        if(element == null)
        {
            menu = FindEditMenu("AutoPeopleEdit");
            menu?.Open();
        }
        else if(element.name == "alg")
        {
            menu = FindEditMenu("AlgorithmEdit");
            menu?.Open();
        }
        else
        {
            switch (element.tag)
            {
                case "door": menu = FindEditMenu("DoorEdit"); break;
                case "tile": menu = FindEditMenu("SectionEdit"); break;
                case "node": menu = FindEditMenu("NodeEdit"); break;
                case "edge": menu = FindEditMenu("EdgeEdit"); break;
                case "person": menu = FindEditMenu("PersonEdit"); break;
                default: break;
            }

            if (menu != null) { menu.Open(); menu.SetEditableElement(element); }
        }
    }

    public void CloseEditMenus()
    {
        for(int i=0; i<editMenus.Length; i++)
        {
            editMenus[i].Close();
        }
    }

    private EditMenu FindEditMenu(string name_)
    {
        for(int i=0; i<editMenus.Length; i++)
        {
            if(editMenus[i].gameObject.name == name_)
            {
                return editMenus[i];
            }
        }
        return null;
    }

    public void OpenNewScenarioMenu()
    {
        menuFileNew.Open();
    }

    public void OpenLoadScenarioMenu()
    {
        menuFileLoad.Open();
    }

    public void OpenSaveScenarioMenu()
    {
        menuFileSave.Open();
    }

    public void SetTimer(float t_){timeText.text = t_+"s";}

    public void SetPlayText(bool b_)
    { 
        string txt;
        if(b_) txt = "PLAY";
        else txt = "PAUSE";

        simulatorButtons[0].transform.Find("PlayPauseText").GetComponent<TMP_Text>().text = txt;
    }


    public void LockUI()
    {
        lockScreen.SetActive(true);
    }

    public void UnlockUI()
    {
        lockScreen.SetActive(false);
    }

    public void SetFeedback(string msg)
    {
        feedbackText.text = msg;
    }

    public void SetFilename(string filename)
    {
        filenameText.text = filename;
    }
}
