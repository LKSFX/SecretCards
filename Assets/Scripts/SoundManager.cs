using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    AudioClip bip0, bip1;
    AudioSource source;

	// Use this for initialization
	void Awake () {
        bip0 = UnityEngine.Resources.Load<AudioClip>("Sounds/bip0");
        bip1 = UnityEngine.Resources.Load<AudioClip>("Sounds/bip1");
        source = gameObject.AddComponent<AudioSource>();
	}
	
	public void playMatch() {
        source.PlayOneShot(bip0);
    }

    public void playWrong() {
        source.PlayOneShot(bip1);
    }
}
