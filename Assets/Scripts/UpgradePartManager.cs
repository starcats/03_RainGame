using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class UpgradePartManager : MonoBehaviour {

	UpgradeManager manager;

	Text currentLevelText;
	Text needPointText;
	int needPoint;
	int currentLevel;
	string contentText;
	int prefabNo;
	int increaseLevel = 0;

	// Use this for initialization
	void Start () {

		manager = GameObject.Find ("UpgradeManager").GetComponent<UpgradeManager> ();

		Button upgradeUpButton = this.gameObject.transform.Find ("UpgradeUpButton").GetComponent<Button> ();
		upgradeUpButton.onClick.AddListener (Upgrade);

		Button upgradeDownButton = this.gameObject.transform.Find ("UpgradeDownButton").GetComponent<Button> ();
		upgradeDownButton.onClick.AddListener (Downgrade);

		Text upgradeContentText = this.gameObject.transform.Find ("UpgradeContentText").GetComponent<Text> ();
		upgradeContentText.text = contentText;

		currentLevelText = this.gameObject.transform.Find ("CurrentLevelText").GetComponent<Text> ();

		needPointText = this.gameObject.transform.Find ("NeedPointText").GetComponent<Text> ();


		RefleshText();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Upgrade () {

		if (UpgradeManager.upgradePoint - needPoint <= -1) {
			// 足りなかったら左右に揺れるアニメーション入れたい
			return;
		}

		UpgradeManager.upgradePoint -= needPoint;
		increaseLevel ++;
		UpgradeManager.sumLevelCount ++;
		currentLevel += 1;	
		UpgradeManager.levelArray[prefabNo] = currentLevel;
		manager.RefleshAllText();
	}

	void Downgrade () {
 		 
 		print (increaseLevel);
		if (increaseLevel <= 0) {
			// 足りなかったら左右に揺れるアニメーション入れたい
			return;
		}
		DoDowngrade();
	}

	void DoDowngrade () {
		UpgradeManager.upgradePoint += (UpgradeManager.sumLevelCount - 1);
		increaseLevel --;
		currentLevel -= 1;
		UpgradeManager.sumLevelCount --;
		UpgradeManager.levelArray[prefabNo] = currentLevel;
		manager.RefleshAllText();
	}

	public void RefleshText () {
		needPoint = UpgradeManager.sumLevelCount;
		needPointText.text = "必要ポイント: " + needPoint.ToString ("f0");
		currentLevelText.text = "現在のレベル: " + currentLevel;
	}

	public void SetParameter (int currentLevel, string contentText, int prefabNo) {
		this.currentLevel = currentLevel;
		this.contentText = contentText;
		this.prefabNo = prefabNo;
	}

	public void UpgradeCancel () {
		int j = increaseLevel;
		for (int i = 0; j >= i; i++) {
			Downgrade();
		}
	}

	public void AllUpgradeCancel () {
		while (currentLevel > 1) {
			DoDowngrade();
		}
		if (increaseLevel < 0) {
			increaseLevel = 0;
		}
	}
}


