using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    public GameObject[] Inventories;
    public PocketManager pocketManager;
    private string inventoryName;
    // Start is called before the first frame update
    void Start()
    {
        Inventories[0].gameObject.SetActive(false);
        Inventories[1].gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        inventoryName = pocketManager.pocket?.ItemName;
        if (inventoryName == "Cloud")
        {
            Inventories[0].gameObject.SetActive(false);
            Inventories[1].gameObject.SetActive(true);
        }
        else if (inventoryName == "Boost")
        {
            Inventories[0].gameObject.SetActive(true);
            Inventories[1].gameObject.SetActive(false);
        }
        else if (inventoryName == null)
        {
            Inventories[0].gameObject.SetActive(false);
            Inventories[1].gameObject.SetActive(false);
        }
        /*
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Inventories[0].gameObject.SetActive(true);
            Inventories[1].gameObject.SetActive(false);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            Inventories[0].gameObject.SetActive(false);
            Inventories[1].gameObject.SetActive(true);
        }
        */



    }
}
