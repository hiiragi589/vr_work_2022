using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAction {
	
	// 初期設定は継承先のコンストラクタで (Startは使えません)
	// アイテムを獲得したときはこのクラスを継承したクラスをnewで新しく生成したものをPocketマネージャーで呼び出してください
	
	public string ItemName; 
	
	public void Use(){
		// 継承先でアイテムの効果を記述
	}
	
}