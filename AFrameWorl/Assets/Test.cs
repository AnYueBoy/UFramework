using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    private void Awake () {
     
    }
    void Start () {
      UIManager.getInstance().showBoard("GameBoard"); 
    }

    void Update () {

    }
}