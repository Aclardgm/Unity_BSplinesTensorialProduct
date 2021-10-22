using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIControllerTensorialProduct : MonoBehaviour
{
    public InputField KUInput;
    public InputField NUInput;
    public InputField KVInput;
    public InputField NVInput;
    public InputField SurfaceVerticesCountInput;
    public GameObject ControlPointUIParent;
    public GameObject NodesUIParent_U;
    public GameObject NodesUIParent_V;
    public GameObject UIVectorPrefab;
    public GameObject UINodePrefab;
    public TensorialProduct TensorialProduct;

    List<List<Vector3>> controlPoints = new List<List<Vector3>>();
    List<float> nodalVector_U = new List<float>();
    List<float> nodalVector_V = new List<float>();

    List<NodeValueCatcher> nodeValueCatchers_U = new List<NodeValueCatcher>();
    List<NodeValueCatcher> nodeValueCatchers_V = new List<NodeValueCatcher>();
    List<List<VectorValueCatcher>> vectorValueCatchers = new List<List<VectorValueCatcher>>();

    public GameObject UIDisabler;

    bool wireframe = false;
    public Shader wireframeshader;
    public Shader nobackfacecullingshader;

    void OnValueChanged(string str)
    {
        try
        {
            float value = float.Parse(str);
        }
        catch (FormatException e)
        {
            Debug.Log("Wrong Format Input !!");
        }
    }


    public void GenerateCurve()
    {

        foreach (var item in controlPoints)
        {
            item.Clear();
        }

        controlPoints.Clear();
        nodalVector_U.Clear();
        nodalVector_V.Clear();

        int count = 0;
        foreach (var item in vectorValueCatchers)
        {
            controlPoints.Add(new List<Vector3>());
            foreach (var i in item)
            {
                controlPoints[count].Add(i.GetVectorValue());
            }
            count++;
        }

        foreach (var item in nodeValueCatchers_U)
        {
            nodalVector_U.Add(item.GetNodeValue());
        }

        foreach (var item in nodeValueCatchers_V)
        {
            nodalVector_V.Add(item.GetNodeValue());
        }


        int k_u = (int)float.Parse(KUInput.text);
        int n_u = (int)float.Parse(NUInput.text);
        int k_v = (int)float.Parse(KVInput.text);
        int n_v = (int)float.Parse(NVInput.text);
        int surfacepointcount = int.Parse(SurfaceVerticesCountInput.text);

        TensorialProduct.BuildCurves(surfacepointcount, ref controlPoints, ref nodalVector_U,ref nodalVector_V, k_u, n_u,k_v,n_v);
    }


    public void ResetPoints()
    {
        int _k_u = (int)float.Parse(KUInput.text);
        int _n_u = (int)float.Parse(NUInput.text);
        int _k_v = (int)float.Parse(KVInput.text);
        int _n_v = (int)float.Parse(NVInput.text);

        foreach (var item in vectorValueCatchers)
        {
            foreach (var i in item)
            {
                Destroy(i.gameObject);

            }
            item.Clear();
        }
        vectorValueCatchers.Clear();
        foreach (var item in nodeValueCatchers_U)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in nodeValueCatchers_V)
        {
            Destroy(item.gameObject);
        }
        nodeValueCatchers_U.Clear();
        nodeValueCatchers_V.Clear();

        for (int i = 0; i < _n_u + 1; i++)
        {
            vectorValueCatchers.Add(new List<VectorValueCatcher>());
            for (int y = 0; y < _n_v + 1; y++)
            {
                GameObject newCtrlPoint = Instantiate(UIVectorPrefab, ControlPointUIParent.transform);
                VectorValueCatcher vec = newCtrlPoint.GetComponent<VectorValueCatcher>();
                vec.SetX( i );
                vec.SetY( y );
                vec.SetZ(1.0f + UnityEngine.Random.value * 2);
                vectorValueCatchers[i].Add(vec);
            }
        }

        for (int i = 0; i < _n_u + _k_u + 1; i++)
        {
            GameObject newNodePoint = Instantiate(UINodePrefab, NodesUIParent_U.transform);
            NodeValueCatcher node = newNodePoint.GetComponent<NodeValueCatcher>();
            node.SetNode(i + 1);
            nodeValueCatchers_U.Add(node);
        }

        for (int i = 0; i < _n_v + _k_v + 1; i++)
        {
            GameObject newNodePoint = Instantiate(UINodePrefab, NodesUIParent_V.transform);
            NodeValueCatcher node = newNodePoint.GetComponent<NodeValueCatcher>();
            node.SetNode(i + 1);
            nodeValueCatchers_V.Add(node);
        }
    }


    public void ToogleUI()
    {
        UIDisabler.SetActive(!UIDisabler.activeInHierarchy);
    }


    public void SwitchtoTensorialProductScene()
    {
        SceneManager.LoadScene(0);
    }

    public void ToggleWireframe()
    {
        //GL.wireframe = !GL.wireframe;
        //Debug.Log("test");
        wireframe = !wireframe;
        if (wireframe)
            TensorialProduct.gameObject.GetComponent<MeshRenderer>().material.shader = wireframeshader;
        else
            TensorialProduct.gameObject.GetComponent<MeshRenderer>().material.shader = nobackfacecullingshader;

    }
}
