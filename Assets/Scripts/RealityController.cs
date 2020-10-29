using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealityController : MonoBehaviour
{

    void Start()
    {
        TimeTickSystem.OnTick += delegate (object sender, TimeTickSystem.OnTickEventArgs e)
        {

        };
    }
    void Update()
    {

    }
}
