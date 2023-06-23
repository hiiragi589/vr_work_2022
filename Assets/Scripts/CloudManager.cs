using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CloudManager : MonoBehaviour {
	
    public GameObject Canvas;
	public float fadespeed = 0.3f;
	public float disturbTime = 10;
	public bool appearCloud = false;
    private RectTransform objectRectTransform;
	private Image imageCloud;
	
	void Start(){
		appearCloud = false;
		imageCloud = GetComponent<Image>();
		UnityEngine.Color imagecolor = imageCloud.color;
		imageCloud.color = new UnityEngine.Color(imagecolor.r, imagecolor.g, imagecolor.b, 0.0f);
		objectRectTransform = Canvas.GetComponent<RectTransform>();
	}
	
	void Update(){
		transform.position = new Vector3(objectRectTransform.rect.width / 2, objectRectTransform.rect.height / 2, 0);
		float WindowMax = Math.Max(objectRectTransform.rect.width, objectRectTransform.rect.height);
        GetComponent<RectTransform>().sizeDelta = new Vector2(WindowMax, WindowMax);
	}
	
	public void appear(){
		StartCoroutine(CloudStart(this.gameObject));
	}
	
	IEnumerator CloudStart(GameObject Cloud){
		imageCloud = Cloud.GetComponent<Image>();
		UnityEngine.Color imagecolor = imageCloud.color;
		imageCloud.color = new UnityEngine.Color(imagecolor.r, imagecolor.g, imagecolor.b, 0.0f);
		
		while (imageCloud.color.a < 1.0f){
			imagecolor = imageCloud.color;
			float fadeAmount = imagecolor.a + (fadespeed * Time.deltaTime);
			fadeAmount = (fadeAmount > 1.0f) ? 1.0f : fadeAmount;
			
			imagecolor = new UnityEngine.Color(imagecolor.r, imagecolor.g, imagecolor.b, fadeAmount);
			imageCloud.color = imagecolor;
			yield return null;
		}
		yield return new WaitForSeconds(disturbTime);
		StartCoroutine(CloudEnd(Cloud));
		yield return null;
	}
	
	IEnumerator CloudEnd(GameObject Cloud){
		UnityEngine.Color imagecolor = imageCloud.color;
		imageCloud.color = new UnityEngine.Color(imagecolor.r, imagecolor.g, imagecolor.b, 1.0f);
		
		while (imageCloud.color.a > 0.0f){
			imagecolor = imageCloud.color;
			float fadeAmount = imagecolor.a - (fadespeed * Time.deltaTime);
			fadeAmount = (fadeAmount < 0.0f) ? 0.0f : fadeAmount;
			
			imagecolor = new UnityEngine.Color(imagecolor.r, imagecolor.g, imagecolor.b, fadeAmount);
			imageCloud.color = imagecolor;
			yield return null;
		}
		yield return null;
	}
	
}
	
	