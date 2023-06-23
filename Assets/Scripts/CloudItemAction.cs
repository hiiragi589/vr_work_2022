using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Drawing;

public class CloudItemAction : ItemAction {
	
    public GameObject Cloud1;
    public GameObject Cloud2;
	
	private System.Random rand = new System.Random();
	
	public CloudItemAction(GameObject C1, GameObject C2) {
		Cloud1 = C1;	Cloud2 = C2;
		ItemName = "Cloud";
	}
	
	public new void Use(){
		
		int r = rand.Next(0,2);
		if(r == 0 && !Cloud1.GetComponent<CloudManager>().appearCloud){
			Cloud1.GetComponent<CloudManager>().appear();
		}else if(!Cloud2.GetComponent<CloudManager>().appearCloud){
			Cloud2.GetComponent<CloudManager>().appear();
		}
	}
}
	