using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocketManager : MonoBehaviour {
	
    public ItemAction pocket = null;
    public int KeyNum;

    void PickUp(ItemAction action) {
        switch(action.ItemName){
            case "Key":
                KeyNum++;
                break;
            default:
				if(pocket == null)	pocket = action;
                break;
        }
    }

    public void Use() {
        if(pocket != null){
            pocket.Use();
            pocket = null;
        }
    }

    void Start() {
        KeyNum = 0;
    }
	
}