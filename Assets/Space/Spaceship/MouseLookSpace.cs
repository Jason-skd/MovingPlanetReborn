using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MouseLookSpace : MonoBehaviour
{
    public float maximumVert = 90.0f;
    public float minimumVert = -90.0f;
    
    private float _verticalRot;
    private float _horizontalRot;

    private Transform _tr;
    private Quaternion _lastAppliedRotation;

    void Awake()
    {
        _tr = transform;
        
    }
    
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MouseRotate(float rotX, float rotY)
    {
        _verticalRot -= rotY;
        _verticalRot = Mathf.Clamp(_verticalRot, minimumVert, maximumVert);
        _horizontalRot += rotX;
        
        // 更换模型后，此处映射需要更改
        transform.localEulerAngles = new Vector3(0, _horizontalRot, _verticalRot);
    }
    
    public void ReInit()
    {   
        // 更换模型后，此处映射需要更改
        _horizontalRot = transform.localEulerAngles.x;
        _verticalRot = transform.localEulerAngles.z;
        maximumVert += _verticalRot;
        minimumVert += _verticalRot;
    }

    private static float NormalizeAngle(float angle)
    {
        angle = Mathf.Repeat(angle + 180f, angle) - 180f;
        return angle;
    }
}


// public class MouseLookSpace : MonoBehaviour
// {
//     public float maximumVert = 90.0f;
//     public float minimumVert = -90.0f;
//
//     private float _verticalRot;
//     private float _horizontalRot;
//
//     private float _baseMaximumVert;
//     private float _baseMinimumVert;
//
//     private Transform _tr;
//     private Quaternion _lastAppliedRotation;
//
//     void Awake()
//     {
//         _tr = transform;
//         _baseMaximumVert = maximumVert;
//         _baseMinimumVert = minimumVert;
//
//         // 初始化当前角度（按原映射：y = horizontal, z = vertical）
//         var e = _tr.localEulerAngles;
//         _horizontalRot = NormalizeAngle(e.y);
//         _verticalRot = NormalizeAngle(e.z);
//
//         // 初始化限制为基准 + 当前偏移（避免重复累加）
//         maximumVert = _baseMaximumVert + _verticalRot;
//         minimumVert = _baseMinimumVert + _verticalRot;
//
//         _lastAppliedRotation = _tr.localRotation;
//     }
//
//     public void MouseRotate(float rotX, float rotY)
//     {
//         _horizontalRot += rotX;
//         _verticalRot = Mathf.Clamp(NormalizeAngle(_verticalRot - rotY), minimumVert, maximumVert);
//
//         // 构建目标四元数（保持原先的轴顺序： x=0, y=_horizontalRot, z=_verticalRot）
//         Quaternion target = Quaternion.Euler(0f, _horizontalRot, _verticalRot);
//
//         // 只在变化明显时赋值，避免无谓更新
//         if (Quaternion.Angle(target, _lastAppliedRotation) > 0.01f)
//         {
//             _tr.localRotation = target;
//             _lastAppliedRotation = target;
//         }
//     }
//
//     public void ReInit()
//     {
//         // 重新读取当前变换并更新内部角度与限制（避免累加）
//         var e = _tr.localEulerAngles;
//         _horizontalRot = NormalizeAngle(e.y);
//         _verticalRot = NormalizeAngle(e.z);
//
//         maximumVert = _baseMaximumVert + _verticalRot;
//         minimumVert = _baseMinimumVert + _verticalRot;
//
//         // 应用一次以同步内部四元数缓存
//         _lastAppliedRotation = _tr.localRotation;
//     }
//
//     private static float NormalizeAngle(float angle)
//     {
//         angle = Mathf.Repeat(angle + 180f, 360f) - 180f;
//         return angle;
//     }
// }
