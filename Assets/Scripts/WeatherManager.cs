using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class WeatherManager : MonoBehaviour {
	GameObject sunImage;
	GameObject cloudImage;

	// Use this for initialization
	void Start () {
		sunImage = transform.Find ("SunImage").gameObject;
		cloudImage = transform.Find ("CloudImage").gameObject;	
		//MoveCloud();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AppearCloud () {
		iTween.MoveTo (cloudImage, iTween.Hash ("x", 0f,"time", 6, "easyType", "easeInCirc", "isLocal", true));
		Invoke ("DisappearSun", 0f);
	}

	public void DisappearCloud () {
		iTween.MoveTo (cloudImage, iTween.Hash ("x", -720.0f,"time", 6, "easyType", "easeInCirc", "isLocal", true));
		// 太陽が出てくるまでの時間
		Invoke ("AppearSun", 0f);
	}	

	void DisappearSun () {
		iTween.MoveTo (sunImage, iTween.Hash ("x", -720.0f, "time", 6, "isLocal", true));
	}

	void AppearSun () {
		iTween.MoveTo (sunImage, iTween.Hash ("x", 0f, "time", 6, "isLocal", true));
	}
}
