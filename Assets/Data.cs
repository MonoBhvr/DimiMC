using TMPro;
using UnityEngine;

public class Data : MonoBehaviour
{
    public string Ip = "";
    public TMP_InputField ipText;

    public bool Started = false;
    void Start()
    {
        //씬 전환시 파괴 금지
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(Started) return;
        Ip = ipText.text;
    }

    public void Filter()
    {
        //ipText에서 . 과 숫자를 제외한 문자 제거
        string text = ipText.text;
        for (int i = 0; i < text.Length; i++)
        {
            if (!char.IsDigit(text[i]) && text[i] != '.')
            {
                text = text.Remove(i, 1);
                i--;
            }
        }
        ipText.text = text;
    }
    public void Set_ip()
    {
    }
}
