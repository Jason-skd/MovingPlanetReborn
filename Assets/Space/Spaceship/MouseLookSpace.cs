using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookSpace : MonoBehaviour
{
    public float MaximumVert = 90.0f;
    public float MinimumVert = -90.0f;
    
    public float VerticalRot;
    public float HorizontalRot;
    // Start is called before the first frame update
    void Start()
    {
        ReInit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MouseRotate(float rotX, float rotY)
    {
        VerticalRot -= rotY;
        VerticalRot = Mathf.Clamp(VerticalRot, MinimumVert, MaximumVert);
        HorizontalRot += rotX;
        
        // 更换模型后，此处映射需要更改
        transform.localEulerAngles = new Vector3(0, HorizontalRot, VerticalRot);
    }
    
    public void ReInit()
    {   
        // 更换模型后，此处映射需要更改
        HorizontalRot = transform.localEulerAngles.x;
        VerticalRot = transform.localEulerAngles.z;
        MaximumVert += VerticalRot;
        MinimumVert += VerticalRot;
    }
}
