using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerTest : MonoBehaviour
{
    [SerializeField] private int count;

    private List<string> layerName = new List<string>()
    {
        "UI",
        "Test"
    };


    private void Start()
    {
        layerTest();
    }

    private void layerTest()
    {
        for (int i = 0; i < count; i++)
        {
            gameObject.layer = LayerMask.NameToLayer(layerName[i % layerName.Count]);
        }
    }
}