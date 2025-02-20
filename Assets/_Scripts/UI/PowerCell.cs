using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class PowerCell : MonoBehaviour
{
    [SerializeField]
    private Vector3 aVertex = new Vector3(-50f, 0, 0);
    [SerializeField]
    private Vector3 bVertex = new Vector3(0, Mathf.Sqrt(.75f)*100, 0);
    [SerializeField]
    private Vector3 cVertex = new Vector3(50f, 0, 0);
    [SerializeField]
    private Color aColor;
    [SerializeField]
    private Color bColor;
    [SerializeField]
    private Color cColor;

    [SerializeField]
    private Vector3 sliderPosition;



    [SerializeField]
    ComponentStat aValueName;
    [SerializeField]
    ComponentStat bValueName;
    [SerializeField]
    ComponentStat cValueName;

    private Dictionary<ComponentStat, float> values;

    [SerializeField]
    private GameObject slider;

    public Action<Dictionary<ComponentStat,float>> OnSliderValueUpdate;

    private void Awake()
    {
        if (Application.IsPlaying(gameObject))
        {
            values = new();
            values.Add(aValueName, 1f / 3f);
            values.Add(bValueName, 1f / 3f);
            values.Add(cValueName, 1f / 3f);
    
        }
    }
    private void Start()
    {
        {
            ResetPowerCell();
        }
    }



    MeshCollider col;
    MeshFilter meshFilter;
    Mesh mesh;
    private void UpdatePowerCell()
    {

        col = gameObject.GetComponent<MeshCollider>();
        col.convex = false;
        meshFilter = gameObject.GetComponent<MeshFilter>();



        if (meshFilter.sharedMesh == null)
        {
            mesh = new Mesh();
            meshFilter.mesh = mesh;
        }
        else
        {
            mesh = meshFilter.sharedMesh;
        }
        mesh.Clear();
        mesh.vertices = new Vector3[] { aVertex, bVertex, cVertex };// (a+b+c)/3+new Vector3(0,0,-1) };
        mesh.colors = new Color[] { aColor, bColor, cColor };
        mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0) };
        mesh.triangles = new int[] { 0, 1, 2 };//, 0, 3, 1, };
        
        col.sharedMesh = mesh;
    }

    private void Update()
    {
        UpdatePowerCell();

        if (Application.IsPlaying(gameObject))
        {
            
            if (Input.GetMouseButton(0))
            {
                SetSliderPosition();
            }
        }


    }
    public void SetSliderPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane tPlane = new(transform.TransformPoint(aVertex), transform.TransformPoint(bVertex), transform.TransformPoint(cVertex));
        tPlane.Raycast(ray, out float distance);
        Vector3 globalTarget = ray.GetPoint(distance);
        Vector3 localTarget = transform.InverseTransformPoint(globalTarget);



        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)
            || hit.collider == null 
            || hit.collider.gameObject != gameObject)
        {
            return;
        }

        slider.transform.localPosition = localTarget;

        Vector3 baryCenter = hit.barycentricCoordinate;



        values[aValueName] = baryCenter.x;
        values[bValueName] = baryCenter.y;
        values[cValueName] = baryCenter.z;

        OnSliderValueUpdate(values);
  
        Debug.Log(hit.barycentricCoordinate);
    }

    public void ResetPowerCell()
    {
        sliderPosition = (aVertex + bVertex + cVertex) / 3;
        values[aValueName] = 1f / 3f;
        values[bValueName] = 1f / 3f;
        values[cValueName] = 1f / 3f;

        slider.transform.localPosition = sliderPosition;
        OnSliderValueUpdate(values);

    }

}


    

