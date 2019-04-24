/*
               #########                       
              ############                     
              #############                    
             ##  ###########                   
            ###  ###### #####                  
            ### #######   ####                 
           ###  ########## ####                
          ####  ########### ####               
         ####   ###########  #####             
        #####   ### ########   #####           
       #####   ###   ########   ######         
      ######   ###  ###########   ######       
     ######   #### ##############  ######      
    #######  #####################  ######     
    #######  ######################  ######    
   #######  ###### #################  ######   
   #######  ###### ###### #########   ######   
   #######    ##  ######   ######     ######   
   #######        ######    #####     #####    
    ######        #####     #####     ####     
     #####        ####      #####     ###      
      #####       ###        ###      #        
        ###       ###        ###               
         ##       ###        ###               
__________#_______####_______####______________

                我们的未来没有BUG              
* ==============================================================================
* Filename: UIPolygonBar.cs
* Created:  2018/1/2 17:25:37
* Author:   To Hard The Mind
* Purpose:  
* ==============================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPolygonBar : UILayer {

    #region member
    [SerializeField]
    private List<float> m_valueList = new List<float> { 0.8f, 0.7f, 0.5f, 0.65f, 0.7f };
    [SerializeField]
    private float m_radio = 180;
    [SerializeField]
    [Range(1, 10)]
    private int m_polygonCount = 5;
    [SerializeField]
    private Color m_lineColor = Color.white;
    [SerializeField]
    private Color m_innerTopColor = new Color(0.102f, 0.608f, 0.918f, 0.972f);
    [SerializeField]
    private Color m_innerBottomColor = new Color(0.102f, 0.608f, 0.918f, 0.972f);
    [SerializeField]
    private float m_lineWidth = 2;
    [SerializeField]
    private UIPanel m_panel;

    [SerializeField]
    [HideInInspector]
    private List<LineRenderer> m_polygonList = new List<LineRenderer>();
    [SerializeField]
    [HideInInspector]
    private List<LineRenderer> m_centerLineList = new List<LineRenderer>();
    [SerializeField]
    [HideInInspector]
    private Material m_mat;
    [SerializeField]
    [HideInInspector]
    private MeshFilter m_centerMeshFilter;
    [SerializeField]
    [HideInInspector]
    private MeshRenderer m_centerMeshRender;
    [NonSerialized]
    private int m_renderQueue = 0;
    #endregion

    #region unity
#if UNITY_EDITOR
    void OnValidate() {
        if (!Application.isPlaying && NGUITools.GetActive(this)) {
            for (int i = 0, imax = m_valueList.Count; i < imax; i++) {
                m_valueList[i] = Mathf.Max(Mathf.Min(m_valueList[i], 1), 0);
            }
            if (m_centerLineList.Count != m_valueList.Count || m_polygonList.Count != m_polygonCount) {
                StartCoroutine(DelayReframeMeshs());
            }
            else {
                UpdateMeshs();
            }
        }
    }
    IEnumerator DelayReframeMeshs() {
        yield return null;
        ReframeMeshs();
    }
#endif
    void OnDestroy() {
        if (m_mat != null) {
            NGUITools.DestroyImmediate(m_mat);
        }
    }
    #endregion

    #region public
    public void SetValue(int index, float value, bool isUpdate) {
        m_valueList[index] = Mathf.Max(Mathf.Min(value, 1), 0);
        if (isUpdate) {
            UpdateMeshs();
        }
    }
    public void UpdateMeshs() {
        CreatMat();
        RefreshLineMesh();
        RefreshCenterPolygon();
        UpdateLayer();
    }
    #endregion

    #region menu
    [ContextMenu("execute")]

    public void ReframeMeshs() {
        if (!m_panel) {
            m_panel = GetComponent<UIPanel>();
        }
        DestoryMeshs();
        CreateMesh();
        UpdateMeshs();
    }

    [ContextMenu("reset")]
    private void DestoryMeshs() {
        for (int i = transform.childCount - 1, imin = 0; i >= imin; i--) {
            NGUITools.Destroy(transform.GetChild(i));
        }
        m_polygonList.Clear();
        m_centerLineList.Clear();
        m_centerMeshFilter = null;
    }
    #endregion

    #region update
    public override void UpdateLayer() {
        if (!m_panel) {
            return;
        }
        int queue = m_panel.startingRenderQueue;
        if (queue == m_renderQueue) {
            return;
        }

        if (m_mat == null) {
            UpdateMeshs();
            return;
        }
        if (!m_centerMeshRender) {
            m_centerMeshRender = m_centerMeshFilter.GetComponent<MeshRenderer>();
        }
        m_centerMeshRender.sharedMaterial = m_mat;
        m_centerMeshRender.sharedMaterial.renderQueue = queue;
        m_centerMeshRender.sortingOrder = m_panel.sortingOrder;
        m_renderQueue = queue;
    }
    private void CreateMesh() {
        int dotCount = m_valueList.Count;
        Shader shader = Shader.Find("Unlit/Polygon");
        m_mat = new Material(shader);
        m_mat.SetFloat("_SmoothDelta", 6.0f / m_radio);

        Vector3 scale = GetWorldScale(transform.parent);
        // split polygon
        for (int i = 0; i < m_polygonCount; i++) {
            LineRenderer lr = DrawLineMesh(scale, m_mat);
            lr.gameObject.name = string.Format("polygon mesh{0}", i);
            m_polygonList.Add(lr);
        }

        for (int i = 0; i < dotCount; i++) {
            LineRenderer lr = DrawLineMesh(scale, m_mat);
            lr.gameObject.name = string.Format("line mesh{0}", i);
            m_centerLineList.Add(lr);
        }

        // centervalue
        GameObject tmpGo = NGUITools.AddChild(gameObject);
        tmpGo.name = "center polygon";
        tmpGo.hideFlags = HideFlags.HideInHierarchy;
        m_centerMeshFilter = tmpGo.AddComponent<MeshFilter>();
        MeshRenderer mr = tmpGo.AddComponent<MeshRenderer>();
        mr.material = m_mat;
    }
    private void RefreshCenterPolygon() {
        if (m_centerMeshFilter == null) {
            return;
        }
        int dotCount = m_valueList.Count;
        Vector3 center = Vector3.zero;
        Vector3[] points = GenPolygonPoints(center, m_radio, dotCount);
        Mesh mesh = new Mesh();
        Vector3[] vetexs = new Vector3[dotCount + 1];
        int[] triangles = new int[3 * dotCount];
        Color[] colors = new Color[dotCount + 1];
        Vector2[] uvs = new Vector2[dotCount + 1];

        vetexs[0] = center;
        colors[0] = m_innerTopColor;
        uvs[0] = new Vector2(0, 0);

        int botIndex = dotCount / 2 - 1;
        for (int i = 0; i < dotCount; i++) {
            vetexs[i + 1] = points[i] * Mathf.Min(m_valueList[i], 1.0f);
            if (i >= botIndex && i <= 2 * botIndex) {
                colors[i + 1] = m_innerBottomColor;
            }
            else {
                colors[i + 1] = m_innerTopColor;
            }
            uvs[i + 1] = new Vector2(1, i % 2);

            triangles[3 * i] = 0;
            triangles[3 * i + 1] = i + 1;
            if (i == dotCount - 1) {
                triangles[3 * i + 2] = 1;
            }
            else {
                triangles[3 * i + 2] = i + 2;
            }
        }

        mesh.vertices = vetexs;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.uv = uvs;

        m_centerMeshFilter.mesh = mesh;
    }
    #endregion

    #region lines
    private void CreatMat() {
        if (m_mat) {
            return;
        }
        EnableDisable();
        Shader shader = Shader.Find("Unlit/Polygon");
        m_mat = new Material(shader);
        m_mat.SetFloat("_SmoothDelta", 6.0f / m_radio);

        for (int i = 0, imax = m_polygonList.Count; i < imax; i++) {
            LineRenderer lr = m_polygonList[i];
            lr.sharedMaterial = m_mat;
        }
        for (int i = 0, imax = m_centerLineList.Count; i < imax; i++) {
            LineRenderer lr = m_centerLineList[i];
            lr.sharedMaterial = m_mat;
        }
        m_centerMeshRender.sharedMaterial = m_mat;
    }
    private void RefreshLineMesh() {
        if (m_polygonList.Count <= 0) {
            return;
        }
        int dotCount = m_valueList.Count;
        Vector3 scale = GetWorldScale(transform.parent);
        Vector3 center = Vector3.zero;
        Vector3[] points = GenPolygonPoints(center, m_radio, dotCount);

        for (int i = 0, imax = m_polygonList.Count; i < imax; i++) {
            float tmpR = m_radio * (i + 1) / (float)imax;
            LineRenderer lr = m_polygonList[i];
            RefreshPolygonLineMesh(lr, tmpR, scale, dotCount);
        }

        for (int i = 0, imax = m_centerLineList.Count; i < imax; i++) {
            LineRenderer lr = m_centerLineList[i];
            RefreshCenterLineMesh(lr, center, points[i], scale);
        }
    }
    private void RefreshCenterLineMesh(LineRenderer lr, Vector3 center, Vector3 rim, Vector3 scale) {
        center.z = 2;
        rim.z = 2;

        lr.SetWidth(m_lineWidth * scale.x, m_lineWidth * scale.y);
        lr.SetVertexCount(2);
        lr.SetPosition(0, center);
        lr.SetPosition(1, rim);
        lr.SetColors(m_lineColor, m_lineColor);
    }
    private LineRenderer DrawLineMesh(Vector3 scale, Material mat) {
        LineRenderer result;
        GameObject tmpGo = NGUITools.AddChild(gameObject);
        tmpGo.hideFlags = HideFlags.HideInHierarchy;
        result = tmpGo.AddComponent<LineRenderer>();

        result.sharedMaterial = mat;
        result.useLightProbes = false;
        result.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        result.receiveShadows = false;
        result.useWorldSpace = false;

        return result;
    }
    private void RefreshPolygonLineMesh(LineRenderer result, Vector3[] points, Vector3 scale, int dotCount) {
        result.SetWidth(m_lineWidth * scale.x, m_lineWidth * scale.y);
        result.SetVertexCount(dotCount + 1);
        result.SetPositions(points);
        result.SetColors(m_lineColor, m_lineColor);
    }
    private void RefreshPolygonLineMesh(LineRenderer result, float rd, Vector3 scale, int dotCount) {
        Vector3[] points = GenPolygonPoints(Vector3.zero, rd, dotCount);
        RefreshPolygonLineMesh(result, points, scale, dotCount);
    }
    #endregion

    #region static
    private static Vector3[] GenPolygonPoints(Vector3 center, float radio, int dotCout) {
        float deltaAngle = 2 * Mathf.PI / dotCout;
        dotCout++;
        Vector3[] points = new Vector3[dotCout];

        float x = center.x;
        float y = center.y;
        for (int i = 0; i < dotCout; i++) {
            if (i == dotCout - 1) {
                points[i] = points[0];
            }
            else {
                //向上为0度
                float angle = i * deltaAngle;
                Vector3 point = new Vector3(x + Mathf.Sin(angle) * radio, y + Mathf.Cos(angle) * radio, 2);
                points[i] = point;
            }
        }

        return points;
    }
    private static Vector3 GetWorldScale(Transform transform) {
        Vector3 worldScale = transform.localScale;
        Transform parent = transform.parent;

        while (parent != null) {
            worldScale = Vector3.Scale(worldScale, parent.localScale);
            parent = parent.parent;
        }

        return worldScale;
    }
    #endregion

}