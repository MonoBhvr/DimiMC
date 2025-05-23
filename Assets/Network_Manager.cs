using System.Collections;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
 * This script is a simple UDP client that connects to a server at a specified IP address and port.
 * this is commands:
 * enter, 'playername' to set the player name
 * exit, 'playername' to exit the game
 * block, x, y, z, blocktype to set a block at the specified position
 *     'blocktype' is the type of block to set
 *     0 = dirt
 *     1 = grass
 *     2 = stone
 *     3 = OakPlank
 *     4 = GoldBlock
 *     5 = DiamondBlock
 *     6 = Bricks
 * distroy, x, y, z to destroy a block at the specified position
 * move, 'playername', position.x, position.y, position.z, rotation.x, rotation.y, rotation.z to move the player to the specified position
 * done, when the initialization is done
 */

public class Network_Manager : MonoBehaviour
//use UDP
{
    public string Ip = "172.19.2.15";
    public int Port = 12346;
    public string playername;
    
    UdpClient udpClient;
    
    public Queue<string> messageQueue = new Queue<string>();
    public Queue<string> taskQueue = new Queue<string>();
    
    public GameObject otherPlayerPrefab;
    
    World_Manager wm;

    public GameObject local_player;
    public GameObject Loading;

    public bool done = false;

    public bool connected = false;
    void Awake()
    {
        GameObject dat = GameObject.Find("Data");
        if (dat != null)
        {
            //get the ip and port from the data object
            Ip = dat.GetComponent<Data>().Ip; 
        }
        else
        {
            //에러창 출력
            Debug.LogError("Data object not found. Using default IP and port.");
            // Destroy(dat);
            // //메인 화면으로 돌아가기
            // SceneManager.LoadScene("Main");
        }
        playername = Random.Range(0, 999999999).ToString();
        wm = GameObject.Find("World_Manager").GetComponent<World_Manager>();
        //set frame rate
        Application.targetFrameRate = 30;
    }
    void Start()
    {
        Time.timeScale = 0;
        //start communication with server
        CommunicateWithServer();
        Invoke("connection_test", 5f);
    }
    void connection_test()
    {
        //test connection
        if (connected)
        {
            Debug.Log("Connected to server at " + Ip + ":" + Port);
        }
        else
        {
            Debug.LogError("Failed to connect to server.");
            //main menu로 돌아가기
            SceneManager.LoadScene("Main");
        }
    }
    
    public void task(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            Debug.LogError("Received a null or empty message.");
            return;
        }

        string[] parts = message.Split(',');
        //check if the message is a command
        if (parts[0] == "block")
        {
            //get the position and block type
            Vector3 position = new Vector3(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
            int blockType = int.Parse(parts[4]);
            //spawn the block
            wm.SpawnBlock(position, wm.BlockPrefab, blockType);
        }
        else if (parts[0] == "destroy")
        {
            //get the position
            Vector3 position = new Vector3(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
            //destroy the block
            wm.DestroyBlock(position);
        }
        else if (parts[0] == "move")
        {
            //get the position and rotation
            Vector3 position = new Vector3(float.Parse(parts[2]), float.Parse(parts[3]), float.Parse(parts[4]));
            Vector3 rotation = new Vector3(float.Parse(parts[5]), float.Parse(parts[6]), float.Parse(parts[7]));
            //move the player
            GameObject.Find(parts[1]).transform.position = position;
            GameObject.Find(parts[1]).transform.eulerAngles = rotation;
        }
        else if (parts[0] == "done")
        {
            if (done == false)
            {
                done = true;
                Time.timeScale = 1;
                local_player.SetActive(true);
                StartCoroutine(Loading_turn());
            }
            else
            {
                //end the game
                StopCommunication();
                Application.Quit();
            }
        }
        else if (parts[0] == "exit")
        {
            //find the player and destroy it
            GameObject player = GameObject.Find(parts[1]);
            if (player != null)
            {
                Destroy(player);
            }
        }
        else if (parts[0] == "enter")
        {
            //create a new player
            GameObject player = Instantiate(otherPlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            player.name = parts[1];
            player.tag = "Player";
        }
        else
        {
            Debug.Log("Unknown command: " + parts[0]);
        }
    }

    void Update()
    {
        //do task
        if (taskQueue.Count > 0)
        {
            for(int i = 0; i < taskQueue.Count; i++){
                string message = taskQueue.Dequeue();
                if (!string.IsNullOrEmpty(message))
                {
                    task(message);
                    Debug.Log("Task: " + message);
                }
                else
                {
                    Debug.LogError("Dequeued a null or empty message from taskQueue." + message);
                }
            }
        }
        
        //send message
        if(messageQueue.Count > 0)
        {
            for(int i = 0; i < messageQueue.Count; i++){
                string message = messageQueue.Dequeue();
                SendMessage(message);
                Debug.Log("Sent: " + message);
            }
        }
    }
    async void CommunicateWithServer()
    {
        try
        {
            udpClient = new UdpClient(Ip, Port);
            Debug.Log("Connected to server at " + Ip + ":" + Port);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to connect to server: " + ex.Message + Ip);
            Destroy(GameObject.Find("Data"));
            //메인 화면으로 돌아가기
            SceneManager.LoadScene("Main");
            return;
        }
        
        //send message
        string m = "enter," + playername;
        SendMessage(m);
        //initialize world
        
        await Task.Run(async () =>
        {
            while (true)
            {
                if (udpClient.Available <= 0)
                {
                    // await Task.Delay(5);
                    continue;
                }
                try
                {
                    //비동기 수신
                    UdpReceiveResult receivedResult = await udpClient.ReceiveAsync();
                    string receivedMessage = System.Text.Encoding.UTF8.GetString(receivedResult.Buffer);
                    Debug.Log("Received: " + receivedMessage);

                    taskQueue.Enqueue(receivedMessage);
                    connected = true;
                }
                catch (SocketException ex)
                {
                    Debug.LogError("SocketException: " + ex.Message);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Exception: " + ex.Message);
                }
            }
        });
    }

    public IEnumerator Loading_turn()
    {
        Color a = Loading.GetComponent<Image>().color;
        //Lerp the color from white to black
        for (float i = 0; i < 1; i += Time.deltaTime)
        {
            a.a = Mathf.Lerp(1, 0, i);
            Loading.GetComponent<Image>().color = a;
            yield return new WaitForSecondsRealtime(0.01f);
        }
        // 0.1초간 대기
        
        Loading.SetActive(false);
    }
    
    public void SendMessage(string message)
    {
        byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
        udpClient.Send(data, data.Length);
    }

    public void StopCommunication()
    {
        udpClient.Close();
    }
    
    private void OnApplicationQuit()
    {
        SendMessage("exit," + playername);
    }
}

