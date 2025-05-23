using UnityEngine;
using UnityEngine.SceneManagement;

public class Start_Manager : MonoBehaviour
{
    public Data dat;
    public void Start()
    {
        Time.timeScale = 1;
        dat = GameObject.Find("Data").GetComponent<Data>();
    }
    public void StartGame()
    {
        dat.Started = true;
        SceneManager.LoadScene("SampleScene");
    }
}
