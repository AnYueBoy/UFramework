using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ZenjectTest : MonoBehaviour {
    // Start is called before the first frame update
    [Inject] private JectOneManager jectOneManager;
    [Inject] private JectTwoManager jectTwoManager;
    void Start () {
        jectTwoManager.connect ();
        jectOneManager.connect (this.ToString ());
    }

    // Update is called once per frame
    void Update () {

    }
}