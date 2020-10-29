using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIController))]
public class Player : MonoBehaviour
{
    [HideInInspector] public AIController aIController;
    // Start is called before the first frame update
    void Start()
    {
        aIController = GetComponent<AIController>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
