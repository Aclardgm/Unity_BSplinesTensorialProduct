using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TensorialProduct : MonoBehaviour
{
    //Maths
    [HideInInspector]
    public List<List<Vector3>> controlPoints = new List<List<Vector3>>();
    [HideInInspector]
    public List<float> nodalVectorU = new List<float>();
    [HideInInspector]
    public List<float> nodalVectorV = new List<float>();
    List<Vector3> Ptemp = new List<Vector3>();


    //Miscellaneous
    MeshFilter meshfilter;
    List<GameObject> controlPointsObj = new List<GameObject>();
    public GameObject controlPointPrefab;
    List<GameObject> surfaceobjects = new List<GameObject>();

    int k_u = -1;
    int n_u = -1;
    float intervalmin_u = -1;
    float intervalmax_u = -1;


    int k_v = -1;
    int n_v = -1;
    float intervalmin_v = -1;
    float intervalmax_v = -1;




    public void BuildCurves(int pointCount,ref List<List<Vector3>> _controlsPoints, ref List<float> _nodalVector_U, 
        ref List<float> _nodalVector_V, int _k_U, int _n_U, int _k_V, int _n_V)
    {
        controlPoints.Clear();
        nodalVectorU.Clear();
        nodalVectorV.Clear();

        controlPoints = new List<List<Vector3>>();
        for (int i = 0; i < _controlsPoints.Count; i++)
        {
            controlPoints.Add(new List<Vector3>());
            for (int j = 0; j < _controlsPoints[i].Count; j++)
            {
                controlPoints[i].Add(_controlsPoints[i][j]);
            }
        }
        SpawnControlsPoints();
        foreach (var item in _nodalVector_U)
        {
            nodalVectorU.Add(item);
        }
        foreach (var item in _nodalVector_V)
        {
            nodalVectorV.Add(item);
        }

        k_u = _k_U;
        k_v = _k_V;
        n_u = _n_U;
        n_v = _n_V;
        
        intervalmin_u = nodalVectorU[k_u - 1];
        intervalmax_u = nodalVectorU[n_u + 1];
        intervalmin_v = nodalVectorV[k_v - 1];
        intervalmax_v = nodalVectorV[n_v + 1];
        GenerateSurface(pointCount,intervalmin_u,intervalmin_v,intervalmax_u,intervalmax_v,ref nodalVectorU,ref nodalVectorV,ref Ptemp);
    }

    void GenerateSurface(int pointsCount, float intervalminU,
        float intervalminV, float intervalmaxU, float intervalmaxV, ref List<float> nodalVectorU, ref List<float> nodalVectorV,
        ref List<Vector3> P)
    {
        List<List<Vector3>> curvePoints = new List<List<Vector3>>(); // Store all surface's points 
        List<List<Vector3>> tempcurvePoints = new List<List<Vector3>>(); // Store curves generated in u 

        for (int i = 0; i < pointsCount + 1; i++)
        {
            curvePoints.Add(new List<Vector3>());
            for (int y = 0; y < pointsCount + 1; y++)
            {
                curvePoints[i].Add(Vector3.zero);
            }
        }

        for (int j = 0; j < n_v + 1; j++)
        {
            int index = 0;
            tempcurvePoints.Add(new List<Vector3>());
            List<Vector3> tempCtrlPoints = new List<Vector3>();
            for (int i = 0; i < controlPoints[j].Count; i++)
            {
                tempCtrlPoints.Add(controlPoints[i][j]);
                Debug.Log(controlPoints[i][j]);

            }

            for (float i = intervalminU; i < intervalmaxU; i += (intervalmaxU - intervalminU) / pointsCount)
            {
                tempcurvePoints[j].Add(Vector3.zero);
                CalculateP(i, ref P,  nodalVectorU, k_u, ref tempCtrlPoints);
                tempcurvePoints[j][index] = P[0];
                index++;
            }
        }

        for (int j = 0; j < tempcurvePoints[0].Count; j++)
        {
            int index = 0;
            List<Vector3> tempCtrlPoints = new List<Vector3>();
            for (int i = 0; i < tempcurvePoints.Count; i++)
            {
                tempCtrlPoints.Add(tempcurvePoints[i][j]);
            }

            for (float i = intervalminV; i < intervalmaxV; i += (intervalmaxV - intervalminV) / pointsCount)
            {
                CalculateP(i, ref P,  nodalVectorV, k_v, ref tempCtrlPoints);
                if(j >= curvePoints.Count || index >= curvePoints[j].Count)
                {
                    Debug.Log("test");
                }
                curvePoints[j][index] = P[0];
                index++;
            }
        }



        //DebugGeneratedVertices(ref curvePoints);
        EditMesh(ref curvePoints);
    }

    void CalculateP(float u,ref List<Vector3> P, List<float> nodalVector,int k,ref List<Vector3> controlPoints)
    {
        P.Clear();
        int offset = 0;
        float uvn = nodalVector[k];
        while (u > uvn)
        {
            offset++;
            uvn = nodalVector[k + offset];
        }
        for (int i = offset; i <= offset + k - 1; i++)
        {
            P.Add(controlPoints[i]);
        }

        for (int j = 0; j <= k - 2; j++)
        {
            for (int i = 0; i <= k - 2 - j; i++)
            {
                P[i] = ((nodalVector[offset + k + i] - u) * P[i] / (nodalVector[offset + k + i] - nodalVector[offset + 1 + i + j]))
                        + (u - nodalVector[offset + 1 + i + j]) * P[i + 1] / (nodalVector[offset + k + i] - nodalVector[offset + 1 + i + j]);

            }
        }
    }

    void SpawnControlsPoints()
    {
        foreach (GameObject go in controlPointsObj)
        {
            Destroy(go);
        }
        controlPointsObj.Clear();
        foreach (List<Vector3> l in controlPoints)
        {
            foreach (var pos in l)
            {
                GameObject go = Instantiate(controlPointPrefab, pos, Quaternion.identity);
                go.transform.localScale /= 10;
                controlPointsObj.Add(go);
            }
        }
    }


    private void Start()
    {
        meshfilter = GetComponent<MeshFilter>();
        //TestTensorialProduct(3, 5);
    }



    void TestTensorialProduct(int k,int n)
    {
        int pc = 100;
        List<List<Vector3>> ctrlps = new List<List<Vector3>>();
        List<float> nodalvecU = new List<float>();
        List<float> nodalvecV = new List<float>();

        for (int i = 0; i < n + 1; i++)
        {
            ctrlps.Add(new List<Vector3>());
            for (int y = 0; y < n + 1; y++)
            {
                Vector3 test = new Vector3(i, y);

                if(i > (n/2) - 1 && i < (n/2) + 1) 
                {
                    test.z++;
                }

                ctrlps[i].Add(test);
            }
        }
        for (int i = 0; i < n + k + 1; i++)
        {
            nodalvecU.Add(i);
            nodalvecV.Add(i);
        }

        BuildCurves(pc,ref ctrlps,ref nodalvecU,ref nodalvecV, k, n, k, n);

    }

    void DebugGeneratedVertices(ref List<List<Vector3>> surfP)
    {

        for (int i = 0; i < surfP.Count; i++)
        {
            for (int y = 0; y < surfP[i].Count; y++)
            {
                Debug.Log(surfP[i][y]);
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.position = surfP[i][y];
                go.transform.localScale /= 100;
            }
        }
    }

    void EditMesh(ref List<List<Vector3>> surfPoints)
    {
        List<Vector3> p = new List<Vector3>();
        foreach (var item in surfPoints)
        {
            p.AddRange(item);
        }
        List<int> indices = new List<int>();
        for (int i = 0; i < surfPoints.Count - 1 ; ++i)
        {
            for (int y = 0; y < surfPoints[i].Count - 1; y++)
            {

                /*
                 *      1-------2
                 *      |      /|     
                 *      |     / |
                 *      |    /  |
                 *      |   /   |
                 *      |  /    |
                 *      | /     |
                 *      3 ------4           
                 */

                // Clock circle
                indices.Add(y + surfPoints[i].Count *  (i )); // 3
                indices.Add((y + 1) + surfPoints[i].Count * (i + 1)); // 2
                indices.Add(y + surfPoints[i].Count * (i + 1)); // 4

                indices.Add(y + surfPoints[i].Count * (i )); // 3
                indices.Add((y + 1) + surfPoints[i].Count * (i)); // 1
                indices.Add((y + 1) + surfPoints[i].Count * (i + 1)); // 2
            }
        }


        Vector3[] vert = p.ToArray();
        List<Vector3> normals = new List<Vector3>();

        Vector3 vec32 = vert[indices[1]] - vert[indices[0]];
        Vector3 vec34 = vert[indices[2]] - vert[indices[0]];
        Vector3 normal = Vector3.Cross(-vec32.normalized, vec34.normalized);

        //for (int i = 0; i < surfPoints.Count; ++i)
        //{
        //    for (int y = 0; y < surfPoints[i].Count; y++)
        //    {

        //        Vector3 toadd;
        //        if (i == surfPoints.Count - 1 && y != 0)
        //        {
        //            Vector3 vec21_ = vert[indices[i + surfPoints.Count * (y - 1)]] - vert[indices[i + surfPoints.Count * y]];
        //            Vector3 vec24_ = vert[indices[i - 1 + surfPoints.Count * y]] - vert[indices[i + surfPoints.Count * y]];
        //            Vector3 normal_ = Vector3.Cross(vec21_.normalized, vec24_.normalized);
        //            toadd = normal_.normalized;
        //        }
        //        else
        //        if (i == surfPoints.Count - 1 && y == 0)
        //        {
        //            Vector3 vec21_ = vert[indices[i + surfPoints.Count * (y + 1)]] - vert[indices[i + surfPoints.Count * y]];
        //            Vector3 vec31_ = vert[indices[i - 1 + surfPoints.Count * y]] - vert[indices[i + surfPoints.Count * y]];
        //            Vector3 normal_ = Vector3.Cross(vec21_.normalized, vec31_.normalized);
        //            toadd = normal_.normalized;
        //        }
        //        else
        //        {
        //            Vector3 vec32_ = vert[indices[i + 1 + surfPoints.Count * y]] - vert[indices[i + surfPoints.Count * y]];
        //            Vector3 vec34_ = vert[indices[i + surfPoints.Count * (y + 1)]] - vert[indices[i + surfPoints.Count * y]];
        //            Vector3 normal_ = Vector3.Cross(vec32_.normalized, vec34_.normalized);
        //            toadd = normal_.normalized;
        //        }

        //        if (toadd == Vector3.zero)
        //            toadd = normal;
        //        normals.Add(toadd);
        //    }
        //}
        //for (int i = surfPoints.Count - 1; i < surfPoints.Count; ++i)
        //{
        //    for (int y = 0; y < surfPoints[i].Count; y++)
        //    {

        //        Vector3 vec32_ = vert[indices[i - 1 + surfPoints.Count * y]] - vert[indices[i + surfPoints.Count * y]];
        //        Vector3 vec34_ = vert[indices[i - 2 + surfPoints.Count * y]] - vert[indices[i + surfPoints.Count * y]];
        //        Vector3 normal_ = Vector3.Cross(-vec32_.normalized, vec34_.normalized);
        //        normals.Add(normal_);
        //    }
        //}

        for (int i = 0; i < vert.Length; i++)
        {
            normals.Add(normal);
        }

        meshfilter.mesh.vertices = vert;
        meshfilter.mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
        meshfilter.mesh.normals = normals.ToArray();
    }


}
