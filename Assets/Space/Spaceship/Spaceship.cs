using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ReInit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ReInit()
    {
        var mouselook = GetComponent<MouseLookSpace>();
        mouselook.ReInit();
    }
}
