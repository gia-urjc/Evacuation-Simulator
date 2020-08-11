using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextInfoController : MonoBehaviour
{
    public TMP_Text AddInfoText(string n_, Vector3 pos_)
    {
        GameObject newGO = new GameObject(n_);
        newGO.transform.SetParent(this.transform);
        newGO.AddComponent<TMPro.TextMeshPro>();

        TMP_Text newText = newGO.GetComponent<TMPro.TextMeshPro>();
        newText.text = n_;
        newText.transform.position = pos_;
        return newText;
    }
}
