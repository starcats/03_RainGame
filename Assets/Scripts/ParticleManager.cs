using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///// なるほど！変数名長すぎてわかりずら！20181225
public class ParticleManager : MonoBehaviour {
	// 雨粒の大きさ
	float defaultStartSizeMax = 0.1f;
	// 生存時間	
	float defaultStartLifetime = 3.0f;
	// 雨の落ちる速度
	float defaultGravityModifierMax = 1.0f;
	// 跳ね返る距離
	float defaultBounceMax = 0.3f;
	// バウンド時のlifetime減少緩和
	float defaultLifetimeLoss = 0.6f;

	void OnParticleCollision () {
		GameManager.rainPoint ++;
		GameManager.prestageNeedPoint --;
	}

	// Use this for initialization
	void Start () {
		ParticleSystem ps = this.gameObject.GetComponent<ParticleSystem> ();
		var main = ps.main;
		var collision = ps.collision;
		main.startSize = new ParticleSystem.MinMaxCurve (0.1f, defaultStartSizeMax + (UpgradeManager.startSizeLevel * 0.02f));
		main.startLifetime = UpgradeManager.startLifetimeLevel * defaultStartLifetime;
		// マイナスにならないように
		// なんか1行でかけるやつあったはず、それ使って書き換える
		float GM = defaultGravityModifierMax - (UpgradeManager.gravityModifierLevel * 0.1f);
		if (GM <= 0) {
			GM = 0;
		}
		main.gravityModifier = new ParticleSystem.MinMaxCurve (0.3f, GM);
		collision.bounce = new ParticleSystem.MinMaxCurve (0.1f, UpgradeManager.bounceLevel * defaultBounceMax);
		collision.lifetimeLoss = UpgradeManager.lifetimeLossLevel * defaultLifetimeLoss;
	}
	
	// Update is called once per frame
	void Update () {
	}
}
