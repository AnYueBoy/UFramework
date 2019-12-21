﻿using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/*
 * @Author: l hy 
 * @Date: 2019-12-16 23:05:55 
 * @Description: 工具类
 * @Last Modified by: l hy 
 * @Last Modified time: 2019-12-16 23:05:55 
 */

public class Util
{

    public static float getAspect()
    {
        float aspect = (float)Screen.width / Screen.height;
        return aspect;
    }

    public static void clearConsole()
    {
        Type log = typeof(EditorWindow).Assembly.GetType("UnityEditor.LogEntries");

        var clearMethod = log.GetMethod("Clear");
        clearMethod.Invoke(null, null);
    }

    public static bool isLandscape()
    {
        float aspect = getAspect();
        if (aspect > 1)
        {
            return true;
        }

        return false;
    }

    public static bool isPadResoluation()
    {
        return targetValue(4 / 3.0f);
    }

    public static bool isPhone()
    {
        return targetValue(16 / 9.0f);
    }

    public static bool targetValue(float value)
    {
        float offset = 0.05f;
        float aspect = getAspect();
        if (aspect > value - offset && aspect < value + offset)
        {
            return true;
        }
        return false;
    }

    public static void mergeSort(List<double> targetArray)
    {
        mergeSort(targetArray, 0, targetArray.Capacity - 1);
    }

    private void mSort(List<double> array, int l, int r)
    {
        if (l == r)
        {
            return;
        }

        // right move calculate
        int mid = l + (r - l) >> 1;
        mSort(array, l, mid);
        mSort(array, mid + 1, r);
        merge(array, l, mid, r);
    }

    private void merge(List<double> array, int l, int mid, int r)
    {
        // List<T> 避免ArrayList 频繁的装箱拆箱操作
        List<double> temp = new List<double>(r - l + 1);
        int leftPointer = l;
        int rightPointer = mid + 1;
        int index = 0;
        while (leftPointer <= mid && rightPointer <= r)
        {
            temp[index++] = array[leftPointer] <= array[rightPointer] ? array[leftPointer++] : array[rightPointer++];
        }

        while (leftPointer <= mid)
        {
            temp[index++] = array[leftPointer++];
        }

        while (rightPointer <= r)
        {
            temp[index++] = array[rightPointer++];
        }

        for (int i = 0; i < temp.Capacity; i++)
        {
            array[l + i] = temp[i];
        }
    }
}