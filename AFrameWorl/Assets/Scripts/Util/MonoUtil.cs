using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoUtil : MonoBehaviour {

    public void deleay (float time, Action onFinished) {
        StartCoroutine (deleayCoroutine (time, onFinished));
    }

    private IEnumerator deleayCoroutine (float time, Action finished) {
        yield return new WaitForSeconds (time);
        finished ();
    }
}