using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EditMenu : MonoBehaviour
{
    public void Close(){ gameObject.SetActive(false);}
    public void Open(){ gameObject.SetActive(true);}
    public abstract void SetEditableElement(GameObject element_);
}
