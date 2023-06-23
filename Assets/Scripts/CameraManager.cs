using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CameraManager : MonoBehaviour
{
    public MapManager map;
	public Camera CameraOnPC;
	
	public float ScrollDivider = 120.0f;
	
    public GameObject WallPrefab;
	public GameObject GroundPrefab;
	
	// テスト用
    public GameObject C1;
    public GameObject C2;
	// ここまで
	
    private float normalMoveSpeed = 90;
    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3;
	private float magnification = 1.0f;
	
    private float mapX, mapY,mapZ;
	private float wallH;

    private bool isInTransition;
    private bool isDisturb;
    private Transform myTransform;

    void Start() {
		// マウスカーソルの設定
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // ワールド座標基準で、現在の回転量へ加算する
        this.transform.eulerAngles = new Vector3(90, 180, 180);
		
		// マップの生成範囲を取得
        mapX = GroundPrefab.GetComponent<Renderer>().bounds.size.x * map.Width;
        mapY = WallPrefab.GetComponent<Renderer>().bounds.size.y * 5;
        mapZ = GroundPrefab.GetComponent<Renderer>().bounds.size.z * map.Height;
        this.transform.position = new Vector3(mapX/2, mapY, mapZ/2);
        
        CameraOnPC.orthographic = true;
        CameraOnPC.orthographicSize = ((mapX > mapZ) ? mapZ/2 : mapX/2);
    }

    void Update(){
		
		Keyboard keyboard = Keyboard.current;
        var mouse = Mouse.current;
		
		if(mouse == null || keyboard == null) return;
		
		Vector3 tmpPos = 
			new Vector3(transform.position.x,transform.position.y,transform.position.z);
		float posSize = CameraOnPC.orthographicSize;
		
        if (keyboard.shiftKey.isPressed) {
			
			magnification = fastMoveFactor;

        } else if (keyboard.ctrlKey.isPressed) {
            
			magnification = slowMoveFactor;

        } else {
            
			magnification = 1.0f;
			
        }
		
		int vertical = 0;
		int horizontal = 0;
		
		if(keyboard.rightArrowKey.isPressed) horizontal = 1;
		if(keyboard.leftArrowKey.isPressed) horizontal = -1;
		if(keyboard.upArrowKey.isPressed)	vertical = 1;
		if(keyboard.downArrowKey.isPressed)	vertical = -1;
		
        tmpPos += transform.up * (normalMoveSpeed * magnification) * vertical * Time.deltaTime;
        tmpPos += transform.right * (normalMoveSpeed * magnification) * horizontal * Time.deltaTime;
        transform.position = new Vector3(((tmpPos.x >= 0 && tmpPos.x <= mapX) ? tmpPos.x : transform.position.x),
                                        transform.position.y,
										((tmpPos.z >= 0 && tmpPos.z <= mapZ) ? tmpPos.z : transform.position.z));
        
        posSize -= (normalMoveSpeed * magnification) * mouse.scroll.ReadValue().y * Time.deltaTime / ScrollDivider;
		CameraOnPC.orthographicSize = ((posSize >= 20 && posSize <= ((mapX > mapZ) ? mapZ/2 : mapX/2)) ? posSize : CameraOnPC.orthographicSize);
		
    }
}
