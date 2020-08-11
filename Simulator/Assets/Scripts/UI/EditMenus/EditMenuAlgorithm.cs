using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditMenuAlgorithm : EditMenu
{
    public TMP_Text alg_text;
    public TMP_Dropdown alg_dd;
    public Button ok_btn;

    SceneController sc;

    private void Start()
    {
        sc = GameObject.Find("ScenarioController(Clone)").GetComponent<SceneController>();
        alg_dd.onValueChanged.AddListener(delegate { AlgCheck(); });
        ok_btn.onClick.AddListener(SetAlg);

        alg_text.text = "Current: "+ alg_dd.captionText.text;

    }

    public override void SetEditableElement(GameObject element_)
    {
    }

    private void AlgCheck()
    {
        
        sc.SetAlgorithm(alg_dd.captionText.text);
    }

    private void SetAlg()
    {
        alg_text.text = "Current: " + alg_dd.captionText.text;
    }

}
