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
}
