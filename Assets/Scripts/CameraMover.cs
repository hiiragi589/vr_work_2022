using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 15.0f, 0.0f);
	private Vector3 ad;
	public float multiplier;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = offset;
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKey(KeyCode.LeftArrow))
        {
           ad = new Vector3(-1 * multiplier, 0, 0);
		   transform.position = transform.position + ad; 
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            ad = new Vector3(multiplier, 0, 0);
			transform.position = transform.position + ad; 
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            ad = new Vector3(0, 0, multiplier);
			transform.position = transform.position + ad; 
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            ad = new Vector3(0, 0, -1 * multiplier);
			transform.position = transform.position + ad; 
        }
        
    }
}
