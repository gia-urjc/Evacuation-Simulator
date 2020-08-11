using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class MenuFileLoad : MenuFile
{
    public TMP_Dropdown namesDD, loadUpToDD;
    public GameObject loadUpToGO;
    public Toggle loadAllToggle;
    public Button acceptBtn;

    void Start()
    {
        sc = GameObject.Find("ScenarioController(Clone)").GetComponent<SceneController>();
        //Add listeners and invoke a method when the values change.
        acceptBtn.onClick.AddListener(Accept);
        loadAllToggle.onValueChanged.AddListener(delegate{LoadAllChangeCheck();});
    }

    override public void Open()
    {
        base.Open();
        // get names from sc and set them to the DD
        namesDD.ClearOptions();
        namesDD.AddOptions(DataController.GetFolders());
    }

    override public void Accept()
    {
        if(namesDD.options.Count<1) return;

        // sc load scenario w/ name, up to string phase/"all"
        int upto=-1;
        if(loadAllToggle.isOn) upto = 4;
        else
        {
            switch(loadUpToDD.captionText.text)
            {
                case "Topology": upto = 0; break;
                case "Graph": upto = 1; break;
                case "People": upto = 2; break;
                case "Paths": upto = 3; break;
                default: break;
            }
        }

        if(upto != -1) StartCoroutine(LoadScenario(namesDD.captionText.text, upto));
        
    }

    public void LoadAllChangeCheck()
    {
        loadUpToGO.SetActive(!loadAllToggle.isOn);
    }

    private IEnumerator LoadScenario(string name_, int upto_)
    {
        sc.SetFeedback("Loading " + name_ + " scenario...");
        sc.LockUI();
        yield return new WaitForSeconds(.05f);
        sc.Load(name_, upto_);
    }
}
