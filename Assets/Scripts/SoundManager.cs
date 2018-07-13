using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    AudioClip[] bips;
    AudioSource source;

	// Use this for initialization
	void Awake () {
        bips = new AudioClip[4];
        bips[0] = UnityEngine.Resources.Load<AudioClip>("Sounds/bip0");
        bips[1] = UnityEngine.Resources.Load<AudioClip>("Sounds/bip1");
        bips[2] = UnityEngine.Resources.Load<AudioClip>("Sounds/bip2");
        bips[3] = UnityEngine.Resources.Load<AudioClip>("Sounds/bip3");
        source = gameObject.AddComponent<AudioSource>();
	}
	
	public void playMatch() {
        if (GameManager.Instance.IsSoundOn)
            source.PlayOneShot(bips[0]);
    }

    public void playWrong() {
        if (GameManager.Instance.IsSoundOn)
            source.PlayOneShot(bips[1]);
    }

    public void playWin() {
        if (GameManager.Instance.IsSoundOn)
            source.PlayOneShot(bips[2]);
    }

    public void playLose() {
        if (GameManager.Instance.IsSoundOn)
            source.PlayOneShot(bips[3]);
    }
}
