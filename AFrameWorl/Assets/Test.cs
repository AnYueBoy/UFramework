using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoUtil
{
    // Start is called before the first frame update


    void Start()
    {
        Vector2[] path =new Vector2[5];
        path[1] = new Vector2(0,5);
        path[2] = new Vector2(5,10);
        path[3] = new Vector2(10,15);
        this.drawLine(path);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
