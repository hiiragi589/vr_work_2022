using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyUIManager : MonoBehaviour
{
    public GameObject Pocket;
    public TMPro.TextMeshProUGUI textMesh;
    private int keyNumber = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        keyNumber = Pocket.GetComponent<PocketManager>().KeyNum;
        textMesh.text = keyNumber.ToString();
        /*
        if (Input.GetKeyDown("space"))
        {
            keyNumber++;
        }
        textMesh.text = keyNumber.ToString();
        */



    }
}
