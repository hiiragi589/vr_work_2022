using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour {
	
	public string ItemName;
    private GameObject Cloud1;
    private GameObject Cloud2;
	private MoveSpeedChanger spchange;
	
	void Satrt() {
		spchange = GameObject.Find("XR Origin").GetComponent<MoveSpeedChanger>();
		GameObject canvas = GameObject.Find("Canvas");
		Cloud1 = canvas.transform.Find("Cloud1").gameObject;
		Cloud2 = canvas.transform.Find("Cloud2").gameObject;
	}
	
	public ItemAction taken(){
		if(this.ItemName == "Cloud") {
			return new CloudItemAction(Cloud1,Cloud2);
		}else if(this.ItemName == "Boost") {
			return new BoostItemAction(spchange);
		}else{
			ItemAction act = new ItemAction();
			act.ItemName = "Key";
			return act;
		}
	}
}