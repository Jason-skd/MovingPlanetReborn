using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpace : MonoBehaviour
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
        var mouseListener = GetComponent<MouseListener>();
        mouseListener.ReInit();
    }
}
