using System;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    float speed = 5f;
    Rigidbody rb;

    private World_Manager wm;
    private Network_Manager nm;

    public GameObject tg;

    public Animator anim;

    public Inventory inv;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        wm = GameObject.Find("World_Manager").GetComponent<World_Manager>();
        nm = GameObject.Find("Network_Manager").GetComponent<Network_Manager>();
        inv = GameObject.Find("Inv").GetComponent<Inventory>();
        
        //mouse lock
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = transform.forward * moveVertical + transform.right * moveHorizontal;
        rb.linearVelocity = new Vector3(movement.x * speed, rb.linearVelocity.y, movement.z * speed);
        
        RaycastHit[] bb = Physics.BoxCastAll(transform.position, new Vector3(0.2f, 0.4f, 0.2f), -transform.up, Quaternion.identity, 0.65f, LayerMask.GetMask("Block"));
        bool onGround = false;
        foreach (var x in bb)
        {
            if (x.collider != null)
            {
                onGround = true;
                break;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 5f, rb.linearVelocity.z);
        }
        
        nm.SendMessage("move," + nm.playername + "," + transform.position.x + "," + transform.position.y + "," +
                       transform.position.z + "," + transform.rotation.eulerAngles.x + "," + transform.rotation.eulerAngles.y + "," +
                       transform.rotation.eulerAngles.z);
    }

    private void OnDrawGizmos()
    {
        //bb Boxcast visualization
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.2f, 0.4f, 0.2f));
        Gizmos.DrawWireCube(transform.position + new Vector3(0, -0.65f, 0), new Vector3(0.2f, 0.4f, 0.2f));
    }

    bool able_to_set_block(Vector3 pos)
    {
        Collider[] colliders = Physics.OverlapBox(pos, new Vector3(0.45f, 0.45f, 0.45f), Quaternion.identity);
        foreach (var x in colliders)
        {
            if (x)
            {
                return false;
            }
        }
        return true;
    }
    
    private void Update()
    {
        //set animation
        //y축 제외한 x축과 z축기준 속도
        float speed = Mathf.Sqrt(Mathf.Pow(rb.linearVelocity.x, 2) + Mathf.Pow(rb.linearVelocity.z, 2));
        anim.SetBool("Walk", speed > 0.1f);
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Block"));
        
        if(hit.collider != null){
            tg.transform.position = hit.collider.gameObject.transform.position;
        }
        else
        {
            tg.transform.position = new Vector3(0, -1000, 0);
        }
        if(Input.GetMouseButtonDown(1))
        {
            Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit1;
            Physics.Raycast(ray1, out hit1, 100f, LayerMask.GetMask("Block"));
            if(hit1.collider == null)
                return;
            GameObject block = hit1.collider?.gameObject;
            if(block == null)
                return;
            Vector3 posAdd = hit1.normal.normalized/2;
            Vector3 pos = hit1.collider.gameObject.transform.position + posAdd;
            //make pos int
            pos.x = Mathf.Floor(pos.x);
            pos.y = Mathf.Floor(pos.y);
            pos.z = Mathf.Floor(pos.z);
            if(able_to_set_block(pos + posAdd))
            {
                wm.SpawnBlock(pos + posAdd, block, inv.selectedIndex);
                nm.messageQueue.Enqueue("block," + (pos + posAdd).x + "," + (pos + posAdd).y + "," + (pos + posAdd).z + "," + inv.selectedIndex);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit1;
            Physics.Raycast(ray1, out hit1, 100f, LayerMask.GetMask("Block"));
            if(hit1.collider == null)
                return;
            Vector3 pos = hit1.collider.gameObject.transform.position;
            //make pos int
            pos.x = Mathf.Floor(pos.x);
            pos.y = Mathf.Floor(pos.y);
            pos.z = Mathf.Floor(pos.z);
            wm.DestroyBlock(pos);
            nm.messageQueue.Enqueue("destroy," + pos.x + "," + pos.y + "," + pos.z);
        }
    }
}
