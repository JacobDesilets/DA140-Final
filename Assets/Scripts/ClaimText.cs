using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClaimText : MonoBehaviour
{

    public string text = "Unset";
    // Start is called before the first frame update
    void Start()
    {
        TMP_Text tmp_text = GetComponent<TMP_Text>();
        tmp_text.text = text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}