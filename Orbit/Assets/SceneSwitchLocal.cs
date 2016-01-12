using UnityEngine;
using System.Collections;

public class SceneSwitchLocal : MonoBehaviour {
    private GameObject controller;
    private GameController control;

    public void SceneSwitchers(int target) { //consider using this for level load stat
        controller = GameObject.Find("GameController"); //finds gamecontroller
        control = controller.GetComponent<GameController>();
        control.SceneSwitchers(target);
    }

    public void CamSwitch() {
        controller = GameObject.Find("GameController"); //finds gamecontroller
        control = controller.GetComponent<GameController>();
        if (control.stratcam) {
            control.stratcam = false;
            Debug.LogWarning("Dynacam Enabled");
        } else {
            control.stratcam = true;
            Debug.LogWarning("Stratcam Enabled");
        }
    }
}
