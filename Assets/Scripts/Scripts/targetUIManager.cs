using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetUIManager : MonoBehaviour
{
    public int targetNumber;
    public GameObject Pocket;
    public TMPro.TextMeshProUGUI textMesh;
    // Start is called before the first frame update
    private int keyNumber = 0;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        keyNumber = Pocket.GetComponent<PocketManager>().KeyNum;
        if (keyNumber < targetNumber)
        {
            textMesh.text = "Key needed : " + targetNumber;
        }
        else
        {
            textMesh.text = "Objective\nAccomplished!";
        }
    }

}
