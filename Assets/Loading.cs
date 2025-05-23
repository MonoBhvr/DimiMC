using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Loading : MonoBehaviour
{
    public float time = 0.5f;
    public TMP_Text text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.fixedDeltaTime;
        if (time <= 0)
        {
            time = 0.5f;
            if (text.text == "Loading...")
            {
                text.text = "Loading";
            }
            else if (text.text == "Loading")
            {
                text.text = "Loading.";
            }
            else if (text.text == "Loading.")
            {
                text.text = "Loading..";
            }
            else if (text.text == "Loading..")
            {
                text.text = "Loading...";
            }
        }
    }
}
