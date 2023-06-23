using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Drawing;

public class BoostItemAction : ItemAction {
	
	public MoveSpeedChanger spchange;
	
	public BoostItemAction(MoveSpeedChanger sp) {
		spchange = sp;
		this.ItemName = "Boost";
	}
	
	public new void Use(){
		spchange.accel();
	}
}