﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditMenuDoor : EditMenu
{
    public TMP_Text IDText, capacityText;
    public Slider capacitySlider;
    public Toggle StairToggle;

    private Door p;

    void Start()
    {
        //Adds a listener to the main slider and invokes a method when the value changes.
        capacitySlider.onValueChanged.AddListener(delegate {CapacityValueChangeCheck();});
        StairToggle.onValueChanged.AddListener(delegate { StairToggleValueChangedCheck(); });
    }

    override public void SetEditableElement(GameObject element_)
    {
        p = element_.GetComponent<Door>();
        if(p != null)
        {
            StairToggle.onValueChanged.RemoveAllListeners();

            IDText.text = "ID: "+p.GetID();
            capacityText.text = "Capacity: "+p.GetCurrentCapacity();
            capacitySlider.maxValue = p.GetMaxCapacity();
            capacitySlider.value = p.GetCurrentCapacity();

            StairToggle.isOn = p.GetIsStair();
            StairToggle.onValueChanged.AddListener(delegate { StairToggleValueChangedCheck(); });
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
    public void StairToggleValueChangedCheck()
    {
        if (p != null)
        {
            p.SetIsStair(StairToggle.isOn);
        }
    }
}
