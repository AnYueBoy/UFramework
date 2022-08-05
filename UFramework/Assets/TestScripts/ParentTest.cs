using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentTest : MonoBehaviour
{
    void Start()
    {
        for (int i = 0; i < switchCount; i++)
        {
            SwitchParent();
        }
    }

    [SerializeField] private Transform oneTrans;
    [SerializeField] private Transform twoTrans;
    [SerializeField] private Transform imageTrans;
    [SerializeField] private int switchCount;

    private void SwitchParent()
    {
        imageTrans.SetParent(oneTrans);
        imageTrans.SetParent(twoTrans);
    }
}