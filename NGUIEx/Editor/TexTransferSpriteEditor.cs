using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

public class TexTransferSpriteEditor : EditorWindow
{
	
	public class TexData
	{
		public bool isExport;
		public string path;
		public List<UICore> uicoreRefList;
		public Transform tex;
	}
	
	Vector2 scrollPos = Vector2.zero;
	private static Dictionary<int,TexData> findAllTexDic = new Dictionary<int, TexData>();
	private static Dictionary<int, TexData> selectTexDic = new Dictionary<int, TexData>();

	private static bool includeChild = true;
	private static bool isSelectAll = true;
	private static bool isUpdateUICore = true;
	private static GameObject[] selectObjects;
	private static bool showScroll = false;
	[MenuItem("GameObject/Tex转换为Sprite", false, 41)]
	public static void TexTransferSprite()
	{
		selectObjects = Selection.gameObjects;
		includeChild = true;
		isSelectAll = true;
		isUpdateUICore = true;
		InitAllTex();
		GetWindow<TexTransferSpriteEditor>(false, "Tex转换为Sprite");
	}

	void OnGUI()
	{

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("isSelect:",GUILayout.Width(60));
		EditorGUILayout.LabelField("transform:", GUILayout.Width(110));
		EditorGUILayout.LabelField("UICore引用:", GUILayout.Width(150));
		EditorGUILayout.LabelField("path:");
		EditorGUILayout.EndHorizontal();
		
		float height = selectTexDic.Count * 30 + 50;
		showScroll = height > 600 ? true : false;
		height = height > 600 ? 600 : height;
		if (showScroll)
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(900), GUILayout.Height(height));

		isSelectAll = true;
		foreach (var v in selectTexDic)
		{
			EditorGUILayout.BeginHorizontal();
			v.Value.isExport = EditorGUILayout.Toggle(v.Value.isExport, GUILayout.Width(60));
			EditorGUILayout.ObjectField(v.Value.tex, typeof(Transform), true, GUILayout.Width(110));
			if (v.Value.uicoreRefList.Count > 0)
			{
				EditorGUILayout.BeginVertical(GUILayout.Width(150));
				for (int i = 0;i<v.Value.uicoreRefList.Count;i++) {
					EditorGUILayout.ObjectField(v.Value.uicoreRefList[i], typeof(UICore), true, GUILayout.Width(150));
				}
				EditorGUILayout.EndVertical();

			}
			else {
				EditorGUILayout.LabelField("无", GUILayout.Width(150));
			}

			EditorGUILayout.LabelField(v.Value.path);
			EditorGUILayout.EndHorizontal();
			if (!v.Value.isExport)
				isSelectAll = false;
		}
		
		if (showScroll)
			EditorGUILayout.EndScrollView();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("totalCount:", selectTexDic.Count.ToString());
		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();

		//select
		bool old = isSelectAll;
		EditorGUILayout.LabelField("selectAll:", GUILayout.Width(60));
		isSelectAll = EditorGUILayout.Toggle(isSelectAll, GUILayout.Width(30));
		if (old != isSelectAll)
		{
			foreach (var v in selectTexDic)
			{
				v.Value.isExport = isSelectAll;
			}
		}
		//child
		old = includeChild;
		EditorGUILayout.LabelField("includeChild:", GUILayout.Width(80));
		includeChild = EditorGUILayout.Toggle(includeChild, GUILayout.Width(30));
		if (old != includeChild)
		{
			CheckSelectTex();
		}
		//uicore
		old = isUpdateUICore;
		EditorGUILayout.LabelField("更新uiCore引用(texure->sprite):", GUILayout.Width(200));
		isUpdateUICore = EditorGUILayout.Toggle(isUpdateUICore, GUILayout.Width(30));
		
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
		if (GUILayout.Button("确定", GUILayout.Width(150)))
		{
			ExportData();
		}
		
		
	}

	private void ExportData() {
		bool hasTransfer = false;
		foreach (var v in selectTexDic)
		{
			if (v.Value.isExport && v.Value.tex != null)
			{
				hasTransfer = true;
				UITexture tex = v.Value.tex.GetComponent<UITexture>();
				if (tex != null) {
					UISprite sp = v.Value.tex.GetComponent<UISprite>();
					if (sp == null) {
						sp = v.Value.tex.gameObject.AddComponent<UISprite>();
					}
					sp.width = tex.width;
					sp.height = tex.height;
					sp.depth = tex.depth;
					sp.leftAnchor = tex.leftAnchor;
					sp.rightAnchor = tex.rightAnchor;
					sp.topAnchor = tex.topAnchor;
					sp.bottomAnchor = tex.bottomAnchor;
					sp.inSafeArea = tex.inSafeArea;
					sp.updateAnchors = tex.updateAnchors;
					GameObject.DestroyImmediate(tex);
				}
				if (isUpdateUICore)
				{
					foreach (var core in v.Value.uicoreRefList)
					{
						UpdateUICoreRef(v.Value.tex, core);
					}

				}
			}
		}
		if (hasTransfer)
			EditorUtility.DisplayDialog("", "转换成功", "确定");
	}

    public static void GetAllTex(GameObject parentGameObject, Dictionary<int,TexData> result,bool includeChild)
	{
		if (parentGameObject == null) return;
		if (includeChild)
		{
			UITexture[] texs = parentGameObject.GetComponentsInChildren<UITexture>(true);
			foreach (UITexture tex in texs)
			{
				int key = tex.transform.GetInstanceID();
				if (!result.ContainsKey(key))
					result.Add(key, GetTex(tex.transform));
			}
		}
		else
		{
			UITexture tex = parentGameObject.GetComponent<UITexture>();
			if (tex)
			{
				int key = tex.transform.GetInstanceID();
				if (!result.ContainsKey(key))
					result.Add(key, GetTex(tex.transform));
			}
		}
	}
	private static TexData GetTex(Transform tran)
	{
		if (tran == null) return null; 
		TexData data = new TexData();
		data.isExport = true;
		data.uicoreRefList = new List<UICore>();
		FindUICore(tran, data.uicoreRefList);
		data.path = GetPath(null, tran, "", true);
		int index = data.path.IndexOf("Core/");
		if (index > 0)
		{
			data.path = data.path.Substring(index);
		}
		data.tex = tran;
		return data;
	}
	private static void InitAllTex() {
		findAllTexDic.Clear();
		if (selectObjects == null || selectObjects.Length == 0) return;
		for (int i = 0; i < selectObjects.Length; i++)
		{
			GetAllTex(selectObjects[i], findAllTexDic, includeChild);
		}
		CheckSelectTex();
	}
	private static void CheckSelectTex()
	{
		selectTexDic.Clear();
		if (selectObjects == null || selectObjects.Length == 0) return;
		for (int i = 0; i < selectObjects.Length; i++)
		{
			if (includeChild)
			{
				foreach (var v in findAllTexDic) {
					if (!selectTexDic.ContainsKey(v.Key)) 
						selectTexDic.Add(v.Key, v.Value);
				}
			}
			else
			{
				UITexture tex = selectObjects[i].GetComponent<UITexture>();
				if (tex)
				{
					int key = tex.transform.GetInstanceID();
					if (findAllTexDic.ContainsKey(key) && !selectTexDic.ContainsKey(key) ) {
						selectTexDic.Add(key, findAllTexDic[key]);
					}
				}
			}
		}
	}
	
	private static string GetPath(Transform root, Transform t,string path = "",bool isFirst = true) {

		if (t == null || t == root || t.name == "ui/pref/uipanel_base.bytes")
		{
			return path;
		}
		if (isFirst)
		{
			path = t.name;
		}
		else
		{
			path = t.name + "/" + path;
		}
		
		return GetPath(root,t.parent, path, false);
	}

	public static void UpdateUICoreRef(Transform tran, UICore uc) {
		foreach (UICore.Param p in uc.allParam)
		{
			if (p.transform == tran && p.type == UICore.ComponentType.Texture)
			{
				p.type = UICore.ComponentType.Sprite;
				
			}
		}
		foreach (UICore.Param p in uc.param)
		{
			if (p.transform == tran && p.type == UICore.ComponentType.Texture)
			{
				p.type = UICore.ComponentType.Sprite;
			}
		}
		foreach (UICore.ParamArray pa in uc.paramArray)
		{
			if (pa.first != null)
			{
				if (pa.parent != null && pa.parent.transform == tran && pa.parent.type == UICore.ComponentType.Texture)
				{
					pa.parent.type = UICore.ComponentType.Sprite;
				}
				if (pa.first.root != null && pa.first.root.transform == tran && pa.first.root.type == UICore.ComponentType.Texture)
				{
					pa.first.root.type = UICore.ComponentType.Sprite;
				}

				foreach (UICore.Param p in pa.first.childs)
				{
					if (p.transform == tran && p.type == UICore.ComponentType.Texture)
					{
						p.type = UICore.ComponentType.Sprite;
					}
				}
			}
		}

	}
	private static bool CheckInUICore(Transform tran, UICore uc) {
		foreach (UICore.Param p in uc.allParam)
		{
			if (p.transform == tran)
			{
				return true;
			}
		}
		foreach (UICore.Param p in uc.param)
		{
			if (p.transform == tran)
			{
				return true;
			}
		}
		foreach (UICore.ParamArray pa in uc.paramArray)
		{
			if (pa.first != null)
			{
				if (pa.parent != null && pa.parent.transform == tran)
				{
					return true;
				}
				if (pa.first.root != null && pa.first.root.transform == tran)
				{
					return true;
				}

				foreach (UICore.Param p in pa.first.childs)
				{
					if (p.transform == tran)
					{
						return true;
					}
				}
			}
		}
		
		return false;
	}
	private static void FindUICore(Transform tran,List<UICore> coreList) {
		UICore[] cores = tran.GetComponentsInChildren<UICore>(true);
		if (cores != null) {
			for (int i = 0; i < cores.Length; i++) {
				UICore core = cores[i];
				bool find = CheckInUICore(tran, core);
				if (find && !coreList.Contains(core))
					coreList.Add(core);
			}
		}
		cores = tran.GetComponentsInParent<UICore>(true);
		if (cores != null)
		{
			for (int i = 0; i < cores.Length; i++)
			{
				UICore core = cores[i];
				bool find = CheckInUICore(tran, core);
				if (find && !coreList.Contains(core))
					coreList.Add(core);
			}
		}
	}
}
