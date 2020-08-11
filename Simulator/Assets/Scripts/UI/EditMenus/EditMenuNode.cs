using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditMenuNode : EditMenu
{
    public TMP_Text IDText, capacityText;
    public Slider capacitySlider;
    public Toggle CPToggle;

    private Node p;

    void Start()
    {
        //Adds a listener to the main slider and invokes a method when the value changes.
        capacitySlider.onValueChanged.AddListener(delegate {CapacityValueChangeCheck();});
        CPToggle.onValueChanged.AddListener(delegate {CPToggleValueChangedCheck();});
    }

    override public void SetEditableElement(GameObject element_)
    {
        p = element_.GetComponent<Node>();
        if(p != null)
        {
            IDText.text = "ID: "+p.GetID();
            capacityText.text = "Capacity: "+p.GetCurrentCapacity();
            capacitySlider.maxValue = p.GetMaxCapacity();
            capacitySlider.value = p.GetCurrentCapacity();
            CPToggle.isOn = p.GetIsCP();
        }   
    }

    // Invoked when the value of the slider changes.
    public void CapacityValueChangeCheck()
    {
        if(p!=null)
        {
            p.SetCurrentCapacity(Mathf.RoundToInt(capacitySlider.value));
            capacityText.text = "Capacity: "+p.GetCurrentCapacity();
        }
    }

    // Invoked when the value of the slider changes.
    public void CPToggleValueChangedCheck()
    {
        if(p!=null)
        {
            p.SetIsCP(CPToggle.isOn);
        }
    }
}
