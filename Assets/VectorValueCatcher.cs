using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VectorValueCatcher : MonoBehaviour
{
    public InputField inputfieldX;
    public InputField inputfieldY;
    public InputField inputfieldZ;

    public Vector3 GetVectorValue()
    {
        return new Vector3(float.Parse(inputfieldX.text), float.Parse(inputfieldY.text), float.Parse(inputfieldZ.text));
    }

    public void SetX(float x)
    {
        inputfieldX.text = x.ToString();
    }
    public void SetY(float y)
    {
        inputfieldY.text = y.ToString();
    }
    public void SetZ(float z)
    {
        inputfieldZ.text = z.ToString();
    }
}
