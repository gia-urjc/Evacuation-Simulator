using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuFile : MonoBehaviour
{
    protected SceneController sc;
    public void Close(){ gameObject.SetActive(false); gameObject.transform.parent.gameObject.SetActive(false);}
    public virtual void Open(){ gameObject.SetActive(true); gameObject.transform.parent.gameObject.SetActive(true);}
    public abstract void Accept();
}
