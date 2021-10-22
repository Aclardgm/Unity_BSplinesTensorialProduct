using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeValueCatcher : MonoBehaviour
{
    public InputField inputfieldNode;

    public float GetNodeValue()
    {
        return float.Parse(inputfieldNode.text);
    }

    public void SetNode(float u)
    {
        inputfieldNode.text = u.ToString();
    }
}
