using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public InputField KInput;
    public InputField NInput;
    public InputField CurvePointCountInput;
    public GameObject ControlPointUIParent;
    public GameObject NodesUIParent;
    public GameObject UIVectorPrefab;
    public GameObject UINodePrefab;
    public BSpline BSpline;

    List<Vector3> controlPoints = new List<Vector3>();
    List<float> nodalVector = new List<float>();

    List<NodeValueCatcher> nodeValueCatchers = new List<NodeValueCatcher>();
    List<VectorValueCatcher> vectorValueCatchers = new List<VectorValueCatcher>();

    public GameObject UIDisabler;

    void OnValueChanged(string str)
    {
        try
        {
            float value = float.Parse(str);
        }
        catch(FormatException e)
        {
            Debug.Log("Wrong Format Input !!");
        }
    }


    public void GenerateCurve()
    {
        controlPoints.Clear();
        nodalVector.Clear();

        foreach (var item in vectorValueCatchers)
        {
            controlPoints.Add(item.GetVectorValue());
        }

        foreach (var item in nodeValueCatchers)
        {
            nodalVector.Add(item.GetNodeValue());
        }

        int k = (int)float.Parse(KInput.text);
        int n = (int)float.Parse(NInput.text);
        int curvepointcount = int.Parse(CurvePointCountInput.text);

        BSpline.BuildCurve(controlPoints, nodalVector, k, n, curvepointcount);
    }


    public void ResetPoints()
    {
        int _k = (int)float.Parse(KInput.text);
        int _n = (int)float.Parse(NInput.text);

        foreach (var item in vectorValueCatchers)
        {
            Destroy(item.gameObject);
        }
        vectorValueCatchers.Clear();
        foreach (var item in nodeValueCatchers)
        {
            Destroy(item.gameObject);
        }
        nodeValueCatchers.Clear();

        for(int i = 0; i < _n + 1; i ++)
        {
            GameObject newCtrlPoint = Instantiate(UIVectorPrefab, ControlPointUIParent.transform);
            VectorValueCatcher vec = newCtrlPoint.GetComponent<VectorValueCatcher>();
            vec.SetX(UnityEngine.Random.value * 20);
            vec.SetY(UnityEngine.Random.value * 20);
            vec.SetZ(UnityEngine.Random.value * 20);
            vectorValueCatchers.Add(vec);
        }

        for (int i = 0; i < _n + _k + 1; i++)
        {
            GameObject newNodePoint = Instantiate(UINodePrefab, NodesUIParent.transform);
            NodeValueCatcher node = newNodePoint.GetComponent<NodeValueCatcher>();
            node.SetNode(i + 1);
            nodeValueCatchers.Add(node);
        }
    }


    public void ToogleUI()
    {
        UIDisabler.SetActive(!UIDisabler.activeInHierarchy);
    }
    

    public void SwitchtoTensorialProductScene()
    {
        SceneManager.LoadScene(1);
    }
    public void SwitchtoBSplineScene()
    {
        SceneManager.LoadScene(0);
    }
}
