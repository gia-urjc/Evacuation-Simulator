using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditMenuEdge : EditMenu
{
    public TMP_Text IDText, distanceText;
    private Edge p;

    override public void SetEditableElement(GameObject element_)
    {
        p = element_.GetComponent<Edge>();
        if(p != null)
        {
            IDText.text = "ID: "+p.GetID();
            distanceText.text = "Distance: "+p.GetDistance();
        }   
    }
}
