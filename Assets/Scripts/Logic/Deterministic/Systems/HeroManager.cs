using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroManager : MonoBehaviour {

	private List<HeroController> freeHeroPool;
	public static HeroManager singleton;
	
	private void Awake() {
		
		singleton = this;
		freeHeroPool = new List<HeroController> ( GetComponentsInChildren<HeroController>() );
		
		foreach(HeroController hero in freeHeroPool)
			hero.gameObject.active = false;
		
	}
	
	private Dictionary<NetworkPlayer, HeroController> heroes = new Dictionary<NetworkPlayer, HeroController>();
	
	public int freePlayers {
		get {
			return freeHeroPool.Count;
		}
	}
	
	public HeroController NewHero(NetworkPlayer player) {
		
		if (freeHeroPool.Count == 0) {
			Debug.LogWarning("No more heroes!");
			return null;
		}
		
		HeroController newHero = freeHeroPool[0];
		ObjectManager.singleton.InitObject(newHero.gameObject);
		freeHeroPool.Remove (newHero);
		heroes.Add (player, newHero);
		newHero.gameObject.active = true;

		return newHero;
		
	}
	
	public HeroController Hero(NetworkPlayer player) {
		
		if ( heroes.ContainsKey(player) ) {
			return heroes[player];
		} else {
			Debug.Log ("I am: " + Network.player.guid );
			Debug.LogError ("No hero for player: " + player.guid );
			return null;
		}
		
	}
	
}
