using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour {

	public static int rainPoint;
	public static int prestageNeedPoint;
	GameObject prestageNeedPointBack;
	Text prestageNeedPointText;
	ParticleSystem rainPS;
	GameObject canvas;
	GameObject canvasGameBack;
	GameObject startRainButtonPf;
	GameObject startRainButton;
	GameObject upgradeBack;
	WeatherManager weatherManager;
	GameObject resetButton;
	GameObject resetReadyImage;
	bool upgradeSlideUp = true;
	Text remainTimeText;


	/////	Upgrade関連
	int upgradeLevel = 1;
	int upgradeCost = 10;

	// 雨の降る量
	Button upgradeRainButton;
	int defaultRainCost = 10;
	int upgradeRainCost;
	float defaultRainNum = 1.0f;
	float upgradeRainNum;			
	float upgradeRainUpNum = 1.0f;			// 一回で上がる量

	// 雨の降る時間
	Button upgradeRainTimeButton;
	int defaultRainTimeCost = 30;
	int upgradeRainTimeCost;
	float rainTime;
	float defaultRainTimeNum = 20.0f;
	float upgradeRainTimeNum;
	float upgradeRainTimeUpNum = 10.0f;
	bool isRainButtonOn = false;			// rainButtonを押したか判定

	// タップによる雨の降る時間増加
	int tapRainTime = 1;

	// リセット関連
	bool isResetReadyImageOn = false;		// リセット時ボタン非活性化
	Text resetGetPointText;
	int getUpgradePoint;
	const int NEED_MIN_RP = 10;

	// 放置管理用
	DateTime lastTime;

	// Use this for initialization
	void Start () {

		GameObject weatherGameObject = GameObject.Find ("WeatherBack");
		weatherManager = weatherGameObject.GetComponent<WeatherManager> ();
		GameObject cloudImage = weatherGameObject.transform.Find ("CloudImage").gameObject;
		remainTimeText = cloudImage.transform.Find ("RemainTimeText").GetComponent<Text> ();
		remainTimeText.text = "" + rainTime;

		/////	リセット関連

		resetButton = GameObject.Find ("ResetButton");	
		Button RB = resetButton.GetComponent<Button> ();
		RB.onClick.AddListener (ResetReadyImageAppear);

		// リセット確認画面
		resetReadyImage = GameObject.Find ("ResetReadyImage");
		
		Button yesBtn = GameObject.Find ("YesButton").GetComponent<Button> ();
		yesBtn.onClick.AddListener (ResetYesButton);
		Button noBtn = resetReadyImage.transform.Find ("NoButton").GetComponent<Button> ();
		noBtn.onClick.AddListener (ResetNoButton);
		resetReadyImage.SetActive (false);
		resetGetPointText =  resetReadyImage.transform.Find ("ResetGetPointText").GetComponent<Text> ();

		canvas = GameObject.Find ("CanvasBack");
		canvasGameBack = GameObject.Find ("CanvasGameBack");
		RainParticleSystemStart();

		// 転生まで残りポイント
		prestageNeedPointBack = GameObject.Find ("PrestageNeedPointBack");
		prestageNeedPointText = GameObject.Find ("PrestageNeedPointText").GetComponent<Text> ();

		/////	startRainButton系

		startRainButtonPf = Resources.Load<GameObject> ("Prefabs/StartRainButton");
		StartRainButton();

		/////	upgread関連

		GameObject upgradeBackPf = Resources.Load<GameObject> ("Prefabs/UpgradePanel");
		upgradeBack = Instantiate<GameObject> (upgradeBackPf);
		upgradeBack.transform.SetParent (canvas.transform, false);
		Button upgradeArrowImage = upgradeBack.transform.Find ("ArrowImage").GetComponent<Button> ();
		upgradeArrowImage.onClick.AddListener (MoveUpgradeBack);
		// upgradeボタン
		upgradeRainButton = upgradeBack.transform.Find ("UpgradeRainButton").GetComponent<Button> ();
		upgradeRainButton.onClick.AddListener (UpgradeRain);
		upgradeRainTimeButton = upgradeBack.transform.Find ("UpgradeRainTimeButton").GetComponent<Button> ();
		upgradeRainTimeButton.onClick.AddListener (UpgradeRainTime);

		// 変数初期化
		StartGame();
		lastTime = DateTime.UtcNow;

	}
	
	void Update () {
		
		/////	放置判定
		DateTime nowTime = DateTime.UtcNow;
		TimeSpan timeSpan = nowTime - lastTime;
	
		if (timeSpan > TimeSpan.FromSeconds (10) && rainTime >= 0){
			print ("放置しましたね");
			int maxIdleTime = (int)rainTime;
			if (timeSpan > TimeSpan.FromSeconds (maxIdleTime)) {
				timeSpan = TimeSpan.FromSeconds (maxIdleTime);
				print (timeSpan);
			} 
			while (timeSpan >= TimeSpan.FromSeconds (0)) {
				timeSpan -= TimeSpan.FromSeconds(10);
				rainTime -= 10.0f;
				// ここで直接RainPoint加算するのではなく、preRainPointを作り加算判定後にポイント数を表示させる
				// 絵を出して、確認ボタンを押した後にゆっくりと加算するようにしたい。
				rainPoint += 10;
				print ("RP加算しました");
			}
		}

		lastTime = DateTime.UtcNow;

		/////	雨の降る時間

		rainTime -= Time.deltaTime;
		if (rainTime < 0 && isRainButtonOn == false ) {
			rainPS.Stop();
			StartRainButton();
			// 雲を隠す
			weatherManager.DisappearCloud();
		}

		/////	リセットボタン出現条件

		if (0 >= prestageNeedPoint) {
			resetButton.SetActive (true);
			prestageNeedPointBack.SetActive(false);
		}

		RefleshText ();

		/////	タップでRainTimeを増やす
		// これはアップグレードで解放できるようにしたい

		if (Input.GetMouseButtonDown(0)) {
			// 雨が降ってる状態　or MaxRainTime以内の時タップで時間が増える
			if (rainTime < 0 || upgradeRainTimeNum < rainTime) {
				return;
			}
			// rainTime += tapRainTime;
		}
	}

	/////	雨Prefab生成

	void RainParticleSystemStart () {
		GameObject rainBackPf = Resources.Load<GameObject> ("Prefabs/RainBack");
		GameObject pf = Instantiate<GameObject> (rainBackPf);
		pf.transform.SetParent (canvas.transform, false);
		rainPS = GameObject.Find ("RainParticleSystem").GetComponent<ParticleSystem> ();
	}

	/////	雨降らすボタン関連

	void StartRainButton () {
		isRainButtonOn = true;
		startRainButton = Instantiate<GameObject> (startRainButtonPf);
		startRainButton.GetComponent<Button> ().onClick.AddListener (PreStartRain);
		startRainButton.transform.SetParent (canvas.transform, false);
	}

	// ボタン押してから雨を降らすまでにタイムラグを作る
	void PreStartRain () {

		if (isResetReadyImageOn != false) {
			return;
		}
		Invoke ("StartRain", 3.0f); // 手動で雲が出来るまで待機させている
		rainTime = upgradeRainTimeNum;
		Destroy (startRainButton);
		isRainButtonOn = false;
		weatherManager.AppearCloud();
	}

	void StartRain () {
		
		if (rainTime > 0) {
			rainPS.Play(); 
		}
	}

	/////	ゲーム中アップグレード関連

	void MoveUpgradeBack () {
		if (upgradeSlideUp == false) {
			iTween.MoveTo (upgradeBack, iTween.Hash ("y", -390.0f, "time", 1, "easyType", "easeInCirc", "isLocal", true));
			upgradeSlideUp = true;
			upgradeBack.transform.Find ("ArrowImage").GetComponent<Image> ().transform.rotation =  Quaternion.Euler(new Vector3 (0, 0, -90.0f));
		} else  {
			iTween.MoveTo (upgradeBack, iTween.Hash ("y", -790.0f, "time", 1, "easyType", "easeInCirc", "isLocal", true));
			upgradeSlideUp = false;
			upgradeBack.transform.Find ("ArrowImage").GetComponent<Image> ().transform.rotation =  Quaternion.Euler(new Vector3 (0, 0, 90.0f));
		}
	}

	void UpgradeRain () {
		var needCost = upgradeLevel * upgradeCost;
		if (needCost > rainPoint || isResetReadyImageOn != false) {
			return;
		}

		rainPoint -= needCost;//upgradeRainCost;
		int hundred = upgradeRainCost / 100;
		upgradeRainCost = upgradeRainCost + 10;// + (10 * hundred);
		var em = rainPS.emission;
		upgradeRainNum += upgradeRainUpNum;
		em.rateOverTime = upgradeRainNum;
		upgradeLevel ++;
		///// ゲームバランス調整中
		// upgradeRainUpNum *= 2;
		UpgradeTextReflesh();
	}

	void UpgradeRainTime () {
		var needCost = upgradeLevel * upgradeCost;
		if (needCost > rainPoint || isResetReadyImageOn != false) {
			return;
		}
		rainPoint -= needCost;//upgradeRainTimeCost;
		upgradeRainTimeCost = upgradeRainTimeCost + 30;
		upgradeRainTimeNum += upgradeRainTimeUpNum;
		rainTime = upgradeRainTimeNum;
		upgradeLevel ++;
		UpgradeTextReflesh();
	}

	void UpgradeTextReflesh () {
		Text upText = upgradeRainButton.transform.Find ("Text").GetComponent<Text> ();
		upText.text = "雨を多く降らせる\n" + "RP : " + upgradeLevel * upgradeCost;//upgradeRainCost;
		Text upText2 = upgradeRainTimeButton.transform.Find ("Text").GetComponent<Text> ();
		upText2.text = "雨を長く降らせる\n" + "RP : " + upgradeLevel * upgradeCost;//upgradeRainTimeCost;
	}

	/////	リセット関連
	// 初期化
	void StartGame () {
		rainPoint = 0;
		prestageNeedPoint = 10000;
		upgradeRainCost = defaultRainCost;
		upgradeRainNum = defaultRainNum;
		rainTime = 0;
		upgradeRainTimeCost = defaultRainTimeCost;
		upgradeRainTimeNum = defaultRainTimeNum;
		isRainButtonOn = false;
		resetButton.SetActive (false);
		prestageNeedPointBack.SetActive(true);

		if (startRainButton != null) {
			Destroy (startRainButton);
		}
		UpgradeTextReflesh();
	}

	void ResetReadyImageAppear () {
		float point = rainPoint / 1;
		getUpgradePoint = (int)point;
		resetGetPointText.text = getUpgradePoint + "ポイント";
		resetReadyImage.SetActive (true);
		isResetReadyImageOn = true;
	}

	void ResetNoButton () {
		isResetReadyImageOn = false;
		resetReadyImage.SetActive (false);
	}

	void ResetYesButton () {
		SceneManager.LoadScene ("UpgradeScene");
		// ここで転生時に得られるポイントを計算
		UpgradeManager.upgradePoint += getUpgradePoint;
		//StartGame();
		//isResetReadyImageOn = false;
		//resetReadyImage.SetActive (false);

	}

	/////	テキスト更新

	void RefleshText () {
		// 後でlistを使ってtextを管理する。transform毎回使うと重そう。かといって変数無駄に増やしたくない。
		// とりあえずはとりあえずは全てtextで入れる。
		// てかupgradeに入れること自体どうなんだろう。
		Text rainPointText = upgradeBack.transform.Find ("RainPointText").GetComponent<Text> ();
		rainPointText.text = "RP : " + rainPoint;

		// 雨の降る残り時間
		float second = rainTime % 60.0f - 0.5f;
		float minute = (rainTime - second) / 60.0f;
		string ss = null;
		if (second < 9.5f) {
			ss = "0";
			if (second < 0) {
				second = 0;
			}
		} else {
			ss = null;
		}

		// 転生残りtext
		remainTimeText.text = minute.ToString ("f0") + ":" + ss + second.ToString ("f0");

		prestageNeedPointText.text = "転生可能まで\n" + prestageNeedPoint + " ml";

	}
}
