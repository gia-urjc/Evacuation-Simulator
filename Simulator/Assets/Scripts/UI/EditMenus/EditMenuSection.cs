using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditMenuSection : EditMenu
{
    public TMP_Text IDText, capacityText;
    public Slider capacitySlider;
    public Toggle CPToggle;

    private Tile t;
    private Section p;


    void Start()
    {
        //Adds a listener to the main slider and invokes a method when the value changes.
        capacitySlider.onValueChanged.AddListener(delegate {CapacityValueChangeCheck();});
        CPToggle.onValueChanged.AddListener(delegate {CPToggleValueChangedCheck();});
    }

    override public void SetEditableElement(GameObject element_)
    {
        t = element_.GetComponent<Tile>();
        if(t != null)
        {
            p = t.GetSection();
            if(p!=null)
            {
                capacitySlider.onValueChanged.RemoveAllListeners();
                CPToggle.onValueChanged.RemoveAllListeners();

                IDText.text = "ID: "+p.GetID();
                capacityText.text = "Capacity: "+p.GetCurrentCapacity();
                capacitySlider.maxValue = p.GetMaxCapacity();
                capacitySlider.value = p.GetCurrentCapacity();
                CPToggle.isOn = p.GetIsCP();
                
                capacitySlider.onValueChanged.AddListener(delegate {CapacityValueChangeCheck();});
                CPToggle.onValueChanged.AddListener(delegate {CPToggleValueChangedCheck();});
                
                // hacer max y actual y current capacidades para secciones y nodos y puertas y edges
                // de edges...distance / ancho ó 100 y ea
                // preguntar a qasim
                // familias etc
                // file manager
            }
        }   
    }

    // Invoked when the value of the slider changes.
    public void CapacityValueChangeCheck()
    {
        if(p!=null)
        {
            Debug.Log("wejejjeje");
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
