using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class MenuFileNew : MenuFile
{
    
    public Button acceptBtn;
    public TMP_InputField wInputField, hInputField;

    int myW, myH;

    void Start()
    {
        sc = GameObject.Find("ScenarioController(Clone)").GetComponent<SceneController>();
        //Add listeners and invoke a method when the values change.
        acceptBtn.onClick.AddListener(Accept);
    }

    override public void Accept()
    {
        int w=0, h=0;
        if(!System.Int32.TryParse(wInputField.text, out w)) return;
        if(!System.Int32.TryParse(hInputField.text, out h)) return;
        if(w==0 || h==0) return;
        w *= 2; h *= 2; // 1 meter = 2 blocks
        myW = w; myH = h;
        // tell sc to create a new with w and h
        StartCoroutine(CreateNewScenario(w,h));
    }
    
    private IEnumerator CreateNewScenario(int w_, int h_)
    {
        //yield return new WaitUntil()
        sc.SetFeedback("Creating new " + w_*.5 + " x " + h_*.5 + " scenario...");
        sc.LockUI();
        yield return new WaitForSeconds(.05f);
        sc.NewScenario(w_, h_);
    }
    
}
