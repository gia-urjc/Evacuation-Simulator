using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class MenuFileSave : MenuFile
{
    public TMP_InputField nameInputField, descriptionInputField;
    public Button acceptBtn;
    
    void Start()
    {
        sc = GameObject.Find("ScenarioController(Clone)").GetComponent<SceneController>();
        //Add listeners and invoke a method when the values change.
        acceptBtn.onClick.AddListener(Accept);
    }

    override public void Accept()
    {
        string name = nameInputField.text;
        if(name.Length<1) return;
        string description = descriptionInputField.text;
        // sc. set this stuff
        sc.SaveAs(name, description);

        Close();
    }
}
