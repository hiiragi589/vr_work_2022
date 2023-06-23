using System.Collections;
using UnityEngine;
using System;
using System.Drawing;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class MapManager : MonoBehaviour {
	
	[SerializeField] private Camera CameraOnPC;

    public GameObject Wall1Prefab;
    public GameObject Wall2Prefab;
	public GameObject GroundPrefab;
	public GameObject PortalPrefab;
	public GameObject PortalWallPrefab;
	public GameObject PortalGroundPrefab;
	public GameObject PortalDoorPrefab;
	
	public GameObject ListParent;
	
	private bool ItemReborn = false;
	
	public int Width;
	public int Height;
	public int deleteGroundMax = 50;
	public int spellNumber;
	private int[,] Map;
	private GameObject[,,] GameObjectMap;
	public int ItemMaxNum;
	public GameObject[] ItemList;
	[SerializeField] private float ItemTime = 3.0f;	// アイテムが再生成される時間
	private float CellY;
	private float CellX;
	private float WallY;
	private float PortalY;
	
	private System.Random rand = new System.Random();
	
	private bool[,] isInTransition;		// 消している床
	[SerializeField] private int fadesize = 0;	// 消える広さ
	[SerializeField] private int fadesizeMax = 2;	// 消える広さ 0:1, 1:3x3=9, 2:5x5=25
	[SerializeField] private float fadespeed = 0.5f;	// 消える・再生するスピード
	[SerializeField] private float rebornTime = 3.0f;	// 床が再生成される時間
	
	[SerializeField] private string selectableTag = "Selectable";
	[SerializeField] private Material highlightMaterial;
	private Material[] defaultMaterial;
	[SerializeField] private Material forbidMaterial;
	private GameObject[] _selection;
	private coord[] point;
	private int selectNum = 0;
	private Vector3[,] defaultPos;
	
	public int getCellState(float x, float y){
		int i = (int)((x + CellX/2) / CellX);
		int j = (int)((y + CellY/2) / CellY);
		return Map[i,j];
	}
	
	private void divideArea(int x1, int y1, int x2, int y2){
		int px = rand.Next(x1 + 2, x2 - 1);
		int py = rand.Next(y1 + 2, y2 - 1);
		while( px - x1 < 3 || x2 - px < 3){
			px = rand.Next(x1 + 2, x2 - 1);
		}
		while( py - y1 < 3 || y2 - py < 3){
			py = rand.Next(y1 + 2, y2 - 1);
		}
		
		for (int i = x1 + 1; i < x2; i++){
			if(i == px || Map[i-1,py] > 1 || i == x1 || i == x2){
				Map[i,py] = 1;
			}else{
				if(rand.Next(0,2) == 0)	Map[i,py] = rand.Next(2,4);
				else	Map[i,py] = 1;
			}
		}
		for (int i = y1 + 1; i < y2; i++){
			if(i == py || Map[px,i-1] > 1 || i == y1 || i == y2){
				Map[px,i] = 1;
			}else{
				if(rand.Next(0,2) == 0)	Map[px,i] = rand.Next(2,4);
				else	Map[px,i] = 1;
			}
		}
		
		Map[x1 + 1,py] = 0;
		Map[px,y1 + 1] = 0;
		Map[px,y2 - 1] = 0;
		Map[x2 - 1,py] = 0;
		Map[x1 + 2,py] = 1;
		Map[px,y1 + 2] = 1;
		Map[px,y2 - 2] = 1;
		Map[x2 - 2,py] = 1;
		
		if (px - x1 >= 6 + rand.Next(0,3)){
			if (y2 - py >= 6 + rand.Next(0,3))	this.divideArea(x1, py, px, y2);
			if (py - y1 >= 6 + rand.Next(0,3))	this.divideArea(x1, y1, px, py);
		}
		if (x2 - px >= 6 + rand.Next(0,3)){
			if (y2 - py >= 6 + rand.Next(0,3))	this.divideArea(px, py, x2, y2);
			if (py - y1 >= 6 + rand.Next(0,3))	this.divideArea(px, y1, x2, py);
		}
		return;
	}
	
	private void buildMaze(){
		
		int portalX = Width / 2 + 1;
		int portalY = Height / 2 + 1;
		int portalWallX = portalX - 5;
		int portalWallY = portalY - 5;
		
		for(int j=0; j<Height; j++){
			for(int i=0; i<Width; i++){
				if( j%(Height-1)==0 || i%(Width-1)==0 )
					Map[i,j] = 1;
				else if( i==portalX && j==portalY )
					Map[i,j] = 5; // portal
				else if( i>=portalWallX && j>=portalWallY
						&& i<=portalWallX+10 && j<=portalWallY+10 ){
							
					if( i==portalWallX || j==portalWallY
						|| i==portalWallX+10 || j==portalWallY+10 )
						Map[i,j] = 4; // portal wall
					else
						Map[i,j] = 6; // portalground
					
				}else if( i==portalWallX || j==portalWallY
						|| i==portalWallX+10 || j==portalWallY+10 )
						Map[i,j] = 1;
				else
					Map[i,j] = 0;
			}
		}
		
		for(int j=portalWallY; j<=portalWallY+10; j+=10){
			for(int i=1; i<Width-1; i++) if(Map[i,j] == 1 && Map[i-1,j] == 1){
				if(rand.Next(0,2) == 0)	Map[i,j] = rand.Next(2,4);
			}
			Map[rand.Next(1,portalWallX),j] = 0;
			Map[rand.Next(portalWallX+11,Width-1),j] = 0;
		}
		
		for(int i=portalWallX; i<=portalWallX+10; i+=10){
			for(int j=1; j<Height-1; j++) if(Map[i,j] == 1 && Map[i,j-1] == 1){
				if(rand.Next(0,2) == 0)	Map[i,j] = rand.Next(2,4);
			}
			Map[i,rand.Next(1,portalWallY)] = 0;
			Map[i,rand.Next(portalWallY+11,Height-1)] = 0;
		}
		
		Map[portalWallX,rand.Next(portalWallY+1,portalWallY+10)] = 7;
		Map[portalWallX+10,rand.Next(portalWallY+1,portalWallY+10)] = 7;
		Map[rand.Next(portalWallX+1,portalWallX+10),portalWallY] = 7;
		Map[rand.Next(portalWallX+1,portalWallX+10),portalWallY+10] = 7;
		
		this.divideArea(0,0,portalWallX,portalWallY);
		this.divideArea(0,portalWallY,portalWallX,portalWallY+10);
		this.divideArea(0,portalWallY+10,portalWallX,Height-1);
		this.divideArea(portalWallX,portalWallY+10,portalWallX+10,Height-1);
		this.divideArea(portalWallX,0,portalWallX+10,portalWallY);
		this.divideArea(portalWallX+10,0,Width-1,portalWallY);
		this.divideArea(portalWallX+10,portalWallY,Width-1,portalWallY+10);
		this.divideArea(portalWallX+10,portalWallY+10,Width-1,Height-1);
		
	}
	
	private void arrangeObjects(){
		for(int j=0; j<Height; j++){
			for(int i=0; i<Width; i++){
				Vector3 pos = new Vector3(CellX*i, 0.0f, CellY*j);
				if(4 <= Map[i,j] && Map[i,j] <= 7){
					GameObjectMap[i,j,0] = 
						Instantiate(PortalGroundPrefab, pos, Quaternion.identity, this.transform);
				}else{
					GameObjectMap[i,j,0] = 
						Instantiate(GroundPrefab, pos, Quaternion.identity, this.transform);
				}
				if(Map[i,j] == 1){
					pos = new Vector3(CellX*i, WallY/2, CellY*j);
					GameObjectMap[i,j,1] = 
						Instantiate(Wall1Prefab, pos, Quaternion.identity, this.transform);
				}else if(Map[i,j] == 2){
					pos = new Vector3(CellX*i, WallY/4, CellY*j);
					GameObjectMap[i,j,1] = 
						Instantiate(Wall2Prefab, pos, Quaternion.identity, this.transform);
				}else if(Map[i,j] == 3){
					pos = new Vector3(CellX*i, WallY*3/4, CellY*j);
					GameObjectMap[i,j,1] = 
						Instantiate(Wall2Prefab, pos, Quaternion.identity, this.transform);
				}else if(Map[i,j] == 4){
					pos = new Vector3(CellX*i, WallY/2, CellY*j);
					GameObjectMap[i,j,1] = 
						Instantiate(PortalWallPrefab, pos, Quaternion.identity, this.transform);
				}else if(Map[i,j] == 5){
					pos = new Vector3(CellX*i, PortalY/2, CellY*j);
					GameObjectMap[i,j,1] = 
						Instantiate(PortalPrefab, pos, Quaternion.identity, this.transform);
				}else if(Map[i,j] == 7){
					pos = new Vector3(CellX * i, WallY / 2, CellY*j);
					GameObjectMap[i,j,1] = 
						Instantiate(PortalDoorPrefab, pos, Quaternion.identity, this.transform);
				}
			}
		}
		
		foreach (Transform child in ListParent.transform) {
			ItemGroup Igroup = child.GetComponent<ItemGroup>();
			for (int j=0; j<Igroup.MaxNum; j++){
				int xTemp;
				int yTemp;
				do{
					xTemp = rand.Next(1, Width - 1);
					yTemp = rand.Next(1, Height - 1);
				} while (Map[xTemp,yTemp] != 0);
				Map[xTemp,yTemp] = 8;
				Vector3 pos = new Vector3(CellX*xTemp, WallY/2, CellY*yTemp);
				Instantiate(Igroup.Prefab, pos, Quaternion.Euler(-49, -11, 8), Igroup.transform);
			}
		}
	}
	
    void Start() {
		selectNum = 0;
		int arraySize = (2*fadesizeMax+1)*(2*fadesizeMax+1);
		defaultMaterial = new Material[arraySize];
		_selection = new GameObject[arraySize];
		point = new coord[arraySize];
		for(int i=0; i<arraySize; i++){
			defaultMaterial[i] = null; point[i] = null;
			_selection[i] = null;
		}
		Map = new int[Width,Height];
		GameObjectMap = new GameObject[Width,Height,2];
		CellX = GroundPrefab.GetComponent<Renderer>().bounds.size.x;
		CellY = GroundPrefab.GetComponent<Renderer>().bounds.size.z;
		WallY = Wall1Prefab.GetComponent<Renderer>().bounds.size.y;
		if(Width > 20 && Height > 20)	this.buildMaze();
		this.arrangeObjects();
		isInTransition = new bool[Height, Width];
		for (int j = 0; j < Height; j++) {
			for (int i = 0; i < Width; i++) {
				isInTransition[i, j] = false;
			}
		}
		spellNumber = deleteGroundMax;
		ItemMaxNum = 0;
		foreach (Transform child in ListParent.transform) {
			ItemMaxNum += child.GetComponent<ItemGroup>().MaxNum;
		}
		ItemList = new GameObject[ItemMaxNum];
    }
	
	private void CheckItem(){
		int i = 0;
		foreach (Transform child in ListParent.transform) {
			ItemGroup Igroup = child.GetComponent<ItemGroup>();
			if(!ItemReborn && Igroup.RebornEnable && child.childCount < Igroup.MaxNum){
				ItemReborn = true;
				StartCoroutine(rebornItem(Igroup));
			}
			foreach (Transform item in child.transform) {
				ItemList[i++] = item.gameObject;
			}
		}
	}
    
	void Update() {
		
		CheckItem();
		
        var mouse = Mouse.current;
		if(mouse == null) return;
		
		if (mouse.rightButton.wasPressedThisFrame) {
            if(++fadesize > fadesizeMax) fadesize = 0;
        }
		for(int i=0; i<selectNum; i++) {
			if (_selection[i] != null) {
				var selectionRenderer = _selection[i].GetComponent<Renderer>();
				selectionRenderer.material = defaultMaterial[i];
				_selection[i] = null;
			}
		}
		selectNum = 0;
		
		Vector2Control vec2 = mouse.position;
		Vector3 pos = new Vector3(vec2.ReadValue().x,vec2.ReadValue().y,0);
		var ray = CameraOnPC.ScreenPointToRay(pos);
		RaycastHit hit;
		
		if (Physics.Raycast(ray, out hit)) {
			Vector3 hitPos = hit.point;
			int i = (int)((hitPos.x + CellX/2) / CellX);
			int j = (int)((hitPos.z + CellY/2) / CellY);
			for(int x=i-fadesize; x<=i+fadesize; x++){
				for(int y=j-fadesize; y<=j+fadesize; y++){
					if (0<x && x<Width && 0<y && y<Height && Map[x,y] == 0 && !isInTransition[x,y]) {
						GameObject selection = GameObjectMap[x,y,0];
						if (selection.CompareTag(selectableTag)) {
							_selection[selectNum] = selection;
							point[selectNum++] = new coord(x,y);
						}
					}
				}
			}
			
			int cost = selectNum + fadesize * 2;
			bool deleteGround = false;
			
			for(int k=0; k<selectNum; k++) {
				var selectionRenderer = _selection[k].GetComponent<Renderer>();
				if( spellNumber >= cost ){
					if( mouse.leftButton.wasPressedThisFrame ){
						StartCoroutine(Fadeout(point[k].x,point[k].y));
						deleteGround = true;
					} else {
						if (selectionRenderer != null) {
							defaultMaterial[k] = selectionRenderer.material;
							selectionRenderer.material = highlightMaterial;
						}
					}
				} else {
					if (selectionRenderer != null) {
						defaultMaterial[k] = selectionRenderer.material;
						selectionRenderer.material = forbidMaterial;
					}
				}
			}
			
			if(deleteGround)	spellNumber -= cost;
		}
	}
	
	private class coord {
		public int x; public int y;
		public coord(int i, int j){
			x = i; y = j;
		}
	}
	
	public IEnumerator rebornItem(ItemGroup Igroup){
		int xTemp;
		int yTemp;
		do{
			xTemp = rand.Next(1, Width - 1);
			yTemp = rand.Next(1, Height - 1);
		} while (Map[xTemp,yTemp] != 0);
		Map[xTemp,yTemp] = 8;
		Vector3 pos = new Vector3(CellX*xTemp, WallY/2, CellY*yTemp);
		Instantiate(Igroup.Prefab, pos, Quaternion.Euler(-49, -11, 8), Igroup.transform);
		yield return new WaitForSeconds(ItemTime);
		if(Igroup.transform.childCount < Igroup.MaxNum) StartCoroutine(rebornItem(Igroup));
		else	ItemReborn = false;
		yield return null;
	}

	public IEnumerator Fadeout(int i, int j) {
        isInTransition[i,j] = true;
		var selectGroundMaterial = GameObjectMap[i,j,0].GetComponent<Renderer>().material;
		// judge transparent (透明/不透明)
		while (selectGroundMaterial.color.a > 0.0f) {
			UnityEngine.Color GroundColor = selectGroundMaterial.color;
			float fadeAmount = GroundColor.a - (fadespeed * Time.deltaTime);
			fadeAmount = (fadeAmount < 0.0f) ? 0.0f : fadeAmount;

			GroundColor = new UnityEngine.Color(GroundColor.r, GroundColor.g, GroundColor.b, fadeAmount);
			selectGroundMaterial.color = GroundColor;
			yield return null;
		}
		Destroy(GameObjectMap[i,j,0]);
		StartCoroutine(Reset(i,j));
		yield return null;

	}
	
	public IEnumerator Reset(int i, int j) {
		yield return new WaitForSeconds(rebornTime);
		StartCoroutine(Fadein(i,j));
		yield return null;
	}
	
	public IEnumerator Fadein(int i, int j) {
		Vector3 pos = new Vector3(CellX*i, 0.0f, CellY*j);
		GameObjectMap[i,j,0] =
			Instantiate(GroundPrefab, pos, Quaternion.identity, this.transform);
		var selectGroundMaterial = GameObjectMap[i,j,0].GetComponent<Renderer>().material;
		UnityEngine.Color GroundColor = selectGroundMaterial.color;
		selectGroundMaterial.color = new UnityEngine.Color(GroundColor.r, GroundColor.g, GroundColor.b, 0.0f);
		
		while (selectGroundMaterial.color.a < 1.0f){
			GroundColor = selectGroundMaterial.color;
			float fadeAmount = GroundColor.a + (fadespeed * Time.deltaTime);
			fadeAmount = (fadeAmount > 1.0f) ? 1.0f : fadeAmount;
			
			GroundColor = new UnityEngine.Color(GroundColor.r, GroundColor.g, GroundColor.b, fadeAmount);
			selectGroundMaterial.color = GroundColor;
			yield return null;
		}
        isInTransition[i,j] = false;
		yield return null;
	}
}
