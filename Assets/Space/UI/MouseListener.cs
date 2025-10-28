using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MouseListener : MonoBehaviour
{
    private MouseLookSpace _mouseLook;
    
    public float sensitivityHor = 9.0f;
    public float sensitivityVert = 9.0f;
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        // 鼠标的移动
        float rotX = Input.GetAxis("Mouse X") * sensitivityHor;
        float rotY = Input.GetAxis("Mouse Y") * sensitivityVert;
        _mouseLook.MouseRotate(rotX, rotY);
    }

    public void ReInit()
    {
        _mouseLook = FindObjectOfType<MouseLookSpace>();
    }
}
