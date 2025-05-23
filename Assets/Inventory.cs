using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public List<Sprite> sprites;
    
    public List<GameObject> items;

    public GameObject selection;
    
    public int selectedIndex;
    
    public World_Manager wm;
    public GameObject cube;

    void Awake()
    {
        wm = GameObject.Find("World_Manager").GetComponent<World_Manager>();
        //각 아이템에 스프라이트를 할당
        for (int i = 0; i < items.Count; i++)
        {
            items[i].GetComponent<Image>().sprite = sprites[i];
            items[i].GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
        }
    }

    void Update()
    {
        float mousewheel = Input.GetAxis("Mouse ScrollWheel")*20;
        mousewheel = Mathf.Clamp(mousewheel, -1, 1);
        // print(mousewheel);
        selectedIndex += (int)mousewheel;
        //인덱스가 0에서 6까지
        if (selectedIndex < 0)
        {
            selectedIndex = items.Count - 1;
        }
        else if (selectedIndex >= items.Count)
        {
            selectedIndex = 0;
        }
        
        cube.GetComponent<Renderer>().material = wm.BlockSprites[selectedIndex];
    
        selection.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(
            selection.GetComponent<RectTransform>().anchoredPosition, 
            items[selectedIndex].GetComponent<RectTransform>().anchoredPosition, 
            Time.deltaTime * 10
        );
    
        //선택된 아이템의 크기를 64로, 아니면 50으로
        for (int i = 0; i < items.Count; i++)
        {
            Vector2 targetSize = (i == selectedIndex) ? new Vector2(64, 64) : new Vector2(50, 50);
            items[i].GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(
                items[i].GetComponent<RectTransform>().sizeDelta, 
                targetSize, 
                Time.deltaTime * 10
            );
        }
    }
}
