using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseListener : MonoBehaviour
{
    private MouseLookSpace mouseLook;
    
    public float SensitivityHor = 9.0f;
    public float SensitivityVert = 9.0f;
    // Start is called before the first frame update
    void Start()
    {
        ReInit();
    }

    // Update is called once per frame
    void Update()
    {
        // 鼠标的移动
        float rotX = Input.GetAxis("Mouse X") * SensitivityHor;
        float rotY = Input.GetAxis("Mouse Y") * SensitivityVert;
        mouseLook.MouseRotate(rotX, rotY);
    }

    void ReInit()
    {
        mouseLook = FindObjectOfType<MouseLookSpace>();
    }
}
