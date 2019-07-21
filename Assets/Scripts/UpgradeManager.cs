using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UpgradeManager : MonoBehaviour {

	public static int sumLevelCount = 1;
	public static int upgradePoint = 10;
	public static int startSizeLevel = 1; 
	public static int startLifetimeLevel = 1;
	public static int gravityModifierLevel = 1;
	public static int bounceLevel = 1;
	public static int lifetimeLossLevel = 1;
	public static List<int> levelArray = new List<int> ();
	List<string> contentTexts = new List<string> ();

	Text upgradePointText;

	// Use this for initialization
	void Start () {
		levelArray.Clear();
		// 新しいアップグレードはここに追加
		levelArray.Add (startSizeLevel);
		contentTexts.Add ("雨粒の大きさ");
		levelArray.Add (startLifetimeLevel);
		contentTexts.Add ("雨の生存時間");
		levelArray.Add (gravityModifierLevel);
		contentTexts.Add ("雨の落ちる速度");
		levelArray.Add (bounceLevel);
		contentTexts.Add ("雨の跳ね返る高さ");
		levelArray.Add (lifetimeLossLevel);
		contentTexts.Add ("雨の消えにくさ");



		Button gameStartButton = GameObject.Find ("GameStartButton").GetComponent<Button> ();
		gameStartButton.onClick.AddListener (LoadGameScene);

		Button upgradeCancelButton = GameObject.Find ("UpgradeCancelButton").GetComponent<Button> ();
		upgradeCancelButton.onClick.AddListener (UpgradeCancelTap);

		Button allUpgradeCancelButton = GameObject.Find ("AllUpgradeCancelButton").GetComponent<Button> ();
		allUpgradeCancelButton.onClick.AddListener (AllUpgradeCancelTap);

		upgradePointText = GameObject.Find ("UpgradePointText").GetComponent<Text> ();

		GameObject content = GameObject.Find ("Content");

		// 必要ポイント共有アップグレード
		GameObject upgradePartBack = Resources.Load<GameObject> ("Prefabs/UpgradePartBack2");

		for (int i = 0; i < levelArray.Count; i++) {
			GameObject pf = Instantiate<GameObject> (upgradePartBack);
			pf.transform.SetParent (content.transform, false);
			UpgradePartManager manager = pf.GetComponent<UpgradePartManager> ();
			manager.SetParameter(levelArray[i], contentTexts[i], i);
		}

		// 一度のみのアップグレード系 途中

		GameObject onlyOneUpgrade = Resources.Load<GameObject> ("Prefabs/OnlyOneUpgradePartBack");
	}

	void Update () {
		upgradePointText.text = "残り振り分けポイント\n" + upgradePoint;
	}

	void LoadGameScene () {

		// 新しいアップグレードしたらここで戻すのを忘れずに
		startSizeLevel = levelArray[0];
		startLifetimeLevel = levelArray[1];
		gravityModifierLevel = levelArray[2];
		bounceLevel = levelArray[3];
		lifetimeLossLevel = levelArray[4];
		SceneManager.LoadScene ("GameScene");
		
	}

	void UpgradeCancelTap () {
		GameObject content = GameObject.Find ("Content");

		foreach (Transform child in content.transform) {
			UpgradePartManager manager = child.GetComponent<UpgradePartManager> ();
			manager.UpgradeCancel();
		}
	}

	void AllUpgradeCancelTap () {
		GameObject content = GameObject.Find ("Content");

		foreach (Transform child in content.transform) {
			UpgradePartManager manager = child.GetComponent<UpgradePartManager> ();
			manager.AllUpgradeCancel();
		}
	}

	public void RefleshAllText () {
		GameObject content = GameObject.Find ("Content");

		foreach (Transform child in content.transform) {
			UpgradePartManager manager = child.GetComponent<UpgradePartManager> ();
			manager.RefleshText();
		}
	}
}
