using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 oldmousepos = Vector3.zero;
    
    void Update()
    {
        transform.position += transform.forward * Input.GetAxis("Forward") * 0.1f;
        transform.position += transform.right * Input.GetAxis("Horizontal") * 0.1f;
        transform.position += transform.up * Input.GetAxis("Vertical") * 0.1f;

        //transform.localPosition += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetAxis("Forward")).normalized * 0.1f;
        if(Input.GetMouseButton(1))
        {
            transform.Rotate(-(Input.mousePosition - oldmousepos).normalized.y, (Input.mousePosition - oldmousepos).normalized.x, 0);
        }
        transform.Rotate(0, 0, Input.GetAxis("RotationZ"));
        oldmousepos = Input.mousePosition;
    }
}
