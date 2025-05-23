using UnityEngine;
using System.Collections.Generic;

public class World_Manager : MonoBehaviour
{
    public GameObject BlockPrefab;
    
    private Network_Manager nm;
    
    public List<Material> BlockSprites = new List<Material>();
    
    void Awake()
    {
        nm = GameObject.Find("Network_Manager").GetComponent<Network_Manager>();
    }
    
    public void SpawnBlock(Vector3 position, GameObject blockPrefab, int blockType = 1)
    {
        BlockPrefab = blockPrefab;
        GameObject a = Instantiate(BlockPrefab, position, Quaternion.identity);
        a.GetComponent<Renderer>().material = BlockSprites[blockType];
        if (blockType == 1)
        {
            //rotate the block randomly but 90 degrees
            a.transform.Rotate(Random.Range(0, 4) * 90, Random.Range(0, 4) * 90, Random.Range(0, 4) * 90);
        }
    }
    
    public void DestroyBlock(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapBox(position, new Vector3(0.2f, 0.2f, 0.2f), Quaternion.identity);
        Debug.Log($"Found {colliders.Length} colliders at position {position}");
        foreach (var x in colliders)
        {
            Debug.Log($"Checking collider: {x.gameObject.name}");
            if (x.gameObject.CompareTag("Block"))
            {
                Debug.Log($"Destroying block: {x.gameObject.name}");
                Destroy(x.gameObject);
                break;
            }
        }
    }
}