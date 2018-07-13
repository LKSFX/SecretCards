using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigToggle : MonoBehaviour {

    // Use this for initialization
    void Awake() {
        GetComponent<Toggle>().onValueChanged.AddListener(OnToggleValueChanged);
        OnToggleValueChanged(GetComponent<Toggle>().isOn);
	}

    private void OnToggleValueChanged(bool isOn) {
        transform.Find("Label").GetComponent<Text>().color 
            = isOn ? new Color(146f/255f, 175f/255, 69/255f) : new Color(114/255f,94/255f,38/255f);
        Debug.Log("toggle set color");
    }
    
}
