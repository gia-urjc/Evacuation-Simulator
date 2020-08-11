using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditMenuAutoPeople : EditMenu
{
    public TMP_Text densityText, aproxText;
    public Slider densitySlider;
    public Button acceptBtn;

    int density, approxPeople;
    SceneController sc;

    void Start()
    {
        sc = GameObject.Find("ScenarioController(Clone)").GetComponent<SceneController>();

        //Adds a listener to the main slider and invokes a method when the value changes.
        densitySlider.onValueChanged.AddListener(delegate { DensityValueChangeCheck(); });
        acceptBtn.onClick.AddListener(Accept);

        density = 10;
        densityText.text = "Density: " + density + "%";
        approxPeople = Mathf.RoundToInt((GetApproxCapacity() * density) / 100);
        aproxText.text = approxPeople +  " people approximately.";
    }

    public override void SetEditableElement(GameObject element_)
    {
        
    }

    public void Accept()
    {
        sc.AddPeopleAuto(density);
    }

    public void DensityValueChangeCheck()
    {
        density = Mathf.RoundToInt(densitySlider.value);
        densityText.text = "Density: " + density + "%";
        approxPeople = Mathf.RoundToInt((GetApproxCapacity() * density) / 100);
        aproxText.text = approxPeople + " people approximately.";
    }

    private int GetApproxCapacity()
    {
        int totalCapacity = 0;
        var sections = sc.GetMap().GetSections();
        foreach (Section s in sections)
        {
            
            if(!s.GetIsCP() && sc.GetClosestNode(s.GetPos(), s))
            {
                //Debug.Log(s.GetID()+" -> "+s.GetMaxCapacity());
                totalCapacity += s.GetMaxCapacity();
            }
        }
        return totalCapacity;
    }
}
