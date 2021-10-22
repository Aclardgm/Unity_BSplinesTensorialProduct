using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSpline : MonoBehaviour
{

    //Miscellaneous
    LineRenderer lr;
    List<GameObject> controlPointsObj = new List<GameObject>();
    public GameObject controlPointPrefab;

    //Math
    [HideInInspector]
    public List<Vector3> controlPoints = new List<Vector3>();
    [HideInInspector]
    public List<float> nodalVector = new List<float>();
    List<Vector3> P = new List<Vector3>();
    int k = -1;
    int degree = -1;
    int n = -1;
    float intervalmin = -1;
    float intervalmax = -1;
    int offset = -1;


    public void BuildCurve(List<Vector3> _controlsPoints, List<float> _nodes, int _k, int _n, int _curvePointsCount)
    {
        controlPoints.Clear();
        nodalVector.Clear();
        k = _k;
        n = _n;
        degree = k - 1;
        foreach (var item in _controlsPoints)
        {
            controlPoints.Add(item);
        }
        foreach (var item in _nodes)
        {
            nodalVector.Add(item);
        }
        intervalmin = nodalVector[k - 1];
        intervalmax = nodalVector[n + 1];
        GenerateCurve(_curvePointsCount);
    }
    void GenerateCurve(int pointsCount)
    {
        List<Vector3> curvePoints = new List<Vector3>();
        for (float i = intervalmin; i <= intervalmax; i += (intervalmax - intervalmin) / pointsCount)
        {
            CalculateP(i);
            curvePoints.Add(P[0]);
        }

        //Debug.Log("curvePoints Size = " + curvePoints.Count);
        //for (int i = 0; i < curvePoints.Count; i++)
        //{
        //    Debug.Log(curvePoints[i]);
        //}

        lr.positionCount = curvePoints.Count;
        lr.SetPositions(curvePoints.ToArray());
        SpawnControlsPoints();
    }
    void CalculateP(float u)
    {
        P.Clear();
        offset = 0;
        float uvn = nodalVector[k];
        while(u > uvn)
        {
            offset++;
            uvn = nodalVector[k + offset];
        }
        for(int i = offset; i <= offset + k - 1;i++)
        {
            P.Add(controlPoints[i]);
        }

        for(int j = 0; j <= k-2;j++)
        {
            for (int i = 0; i <= k - 2 - j; i++)
            {
                P[i] = ((nodalVector[offset + k + i] - u) * P[i] / (nodalVector[offset + k + i] - nodalVector[offset + 1 + i + j])) 
                        + (u - nodalVector[offset + 1 + i + j]) * P[i + 1] / (nodalVector[offset + k + i] - nodalVector[offset + 1 + i + j]);

            }
        }
    }

    
    // Miscellaneous
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }


    //Debug 
    void Initialize(int _k, int _n)
    {
        k = _k;
        n = _n;
        degree = k - 1;

        while (controlPoints.Count > n + 1)
        {
            controlPoints.RemoveAt(controlPoints.Count - 1);
        }

        for (int i = 0; i < k + n + 1; i++)
        {
            nodalVector.Add(i);
        }

        intervalmin = nodalVector[k - 1];
        intervalmax = nodalVector[n + 1];
    }
    void SpawnControlsPoints()
    {
        foreach (GameObject go in controlPointsObj)
        {
            Destroy(go);
        }
        controlPointsObj.Clear();
        foreach (Vector3 pos in controlPoints)
        {
            controlPointsObj.Add(Instantiate(controlPointPrefab, pos, Quaternion.identity));
        }
    }

    void Log()
    {
        Debug.Log("k = " + k);
        Debug.Log("degree = " + degree);
        Debug.Log("n = " + n);
        Debug.Log("intervalmin = " + intervalmin);
        Debug.Log("intervalmax = " + intervalmax);
        Debug.Log("offset = " + offset);
        Debug.Log("NodalVector Size = " + nodalVector.Count);
        for (int i = 0; i < nodalVector.Count; i++)
        {
            Debug.Log(nodalVector[i]);
        }
        Debug.Log("controlPoints Size = " + controlPoints.Count);
        for (int i = 0; i < controlPoints.Count; i++)
        {
            Debug.Log(controlPoints[i]);
        }
        Debug.Log("P Size = " + P.Count);
        for (int i = 0; i < P.Count; i++)
        {
            Debug.Log(P[i]);
        }
    }
}
