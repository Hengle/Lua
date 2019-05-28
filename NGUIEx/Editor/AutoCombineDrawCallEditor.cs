using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

public class AutoCombineDrawCallEditor : EditorWindow
{
	private static GameObject selectObject;
	private static List<SimpleDC> dcList = new List<SimpleDC>();
	private static List<GameObject> hideList = new List<GameObject>();
	private static List<GameObject> emptyMatList = new List<GameObject>();
	private static Dictionary<SimpleDC, DrawCallData> allWidgetDic = new Dictionary<SimpleDC, DrawCallData>();
	private static Dictionary<int, DepthChangeData> changeDepthDic = new Dictionary<int, DepthChangeData>();
	private static Dictionary<SimpleDC, DrawCallData> preAllWidgetDic = new Dictionary<SimpleDC, DrawCallData>();

	private static bool mShowAllHide;
	private static bool isInitHide;

	[MenuItem("GameObject/合并drawcall", false, 41)]
	public static void AutoCombineDrawCall()
	{
		selectObject = Selection.activeGameObject;
		isInitHide = false;
		mShowAllHide = true;
		changeDepthDic.Clear();
		InitAllHide();
		ChangeHideObjShow(mShowAllHide);
		GetWindow<AutoCombineDrawCallEditor>(false, "合并drawcall");
	}
	
	private static void InitAllWidgets()
	{
		dcList.Clear();
		allWidgetDic.Clear();
		emptyMatList.Clear();

		DrawCallData data = null;
		List<UIDrawCall> tempList = new List<UIDrawCall>();
		foreach (var drawcall in UIDrawCall.activeList) {
			tempList.Add(drawcall);
		}
		tempList.Sort((a, b) => {
			if (a.manager.depth != b.manager.depth) {
				return a.manager.depth - b.manager.depth;
			}
			if (a.manager.GetInstanceID() != b.manager.GetInstanceID()) {
				return a.manager.GetInstanceID() - b.manager.GetInstanceID();
			}
			return a.depthStart - b.depthStart;
		});
		foreach (var drawcall in tempList)
		{
			var simpleDc = new SimpleDC(false, drawcall);
			dcList.Add(simpleDc);
			if (allWidgetDic.ContainsKey(simpleDc))
			{

				data = allWidgetDic[simpleDc];
			}
			else
			{
				data = new DrawCallData();
				data.simple = simpleDc;
				allWidgetDic.Add(simpleDc, data);
			}
			foreach (var panel in UIPanel.list)
			{
				if (panel == drawcall.manager)
				{
					foreach (var widget in panel.widgets)
					{

						if (widget.drawCall == drawcall)
						{
							if (!data.widgets.Contains(widget))
								data.widgets.Add(widget);

						}
						else
						{
							if (!emptyMatList.Contains(widget.gameObject))
							{
								UITexture tex = widget.GetComponent<UITexture>();
								UISprite sp = widget.GetComponent<UISprite>();
								UILabel lb = widget.GetComponent<UILabel>();
								/*if ((tex != null || sp != null || lb != null) && widget.drawCall == null)

								{
									emptyMatList.Add(widget.gameObject);
								}*/
								if (tex != null && tex.mainTexture == null)
								{
									emptyMatList.Add(widget.gameObject);
								}
								else if (sp != null && sp.atlas == null)
								{
									emptyMatList.Add(widget.gameObject);
								}
								else if (lb != null && lb.text == "")
								{
									emptyMatList.Add(widget.gameObject);
								}
								else
								{
									UIWidget ew = widget.GetComponent<UIWidget>();
									BoxCollider box = widget.GetComponent<BoxCollider>();
									if (ew != null && ew.drawCall == null && box != null)
									{
										emptyMatList.Add(widget.gameObject);
									}
								}
							}
							
						}
					}
				}
			}
		}
		
		foreach (var dic in allWidgetDic)
		{
			dic.Value.Sort();
		}
		foreach (var go in emptyMatList)
		{
			var w = go.GetComponent<UIWidget>();
			if (w != null)
			{
				CreateEmptyDC(w);
			}
		}
		dcList.Sort((a, b) => {
			if (a.manager.depth != b.manager.depth)
			{
				return a.manager.depth - b.manager.depth;
			}
			if (a.manager.GetInstanceID() != b.manager.GetInstanceID())
			{
				return a.manager.GetInstanceID() - b.manager.GetInstanceID();
			}
			return allWidgetDic[a].DepthStart - allWidgetDic[b].DepthStart;
		});
	}
	private static void CreateEmptyDC(UIWidget w)
	{
		for (int j = 0; j < dcList.Count; j++)
		{
			var d = allWidgetDic[dcList[j]];
			bool needCreate = true;
			if (w.panel != d.simple.manager)
				continue;
			if (w.depth <= d.DepthStart)
			{
				if (d.simple.isEmpty)
				{
					d.AddW(w);
					return;
				}
				if (needCreate && j > 0)
				{
					DrawCallData dpre = null;
					for (int k = j - 1; k >= 0; k--)
					{
						if (dcList[k].manager == w.panel)
						{
							dpre = allWidgetDic[dcList[k]];
							break;
						}
					}
					if (dpre != null && dpre.simple.isEmpty)
					{
						if (dpre.simple.manager == w.panel)
						{
							dpre.AddW(w);
							return;
						}
					}
				}
				CreateEmptyDC(j, w);
				return;
			}
			else {
				if (w.depth > d.DepthStart && w.depth < d.DepthEnd) { 
					if (d.simple.isEmpty)//空
					{
						d.AddW(w);
						return;
					}
					else {
						CreateEmptyDC(j+1, w);
						//depth大的 移到后一个新dc中
						SimpleDC newDC = new SimpleDC(false,d.simple.dc);
						dcList.Insert(j + 2, newDC);
						DrawCallData dcData = new DrawCallData();
						dcData.simple = newDC;
						allWidgetDic.Add(newDC, dcData);
						for (int k = 0; k < d.widgets.Count; k++) {
							UIWidget temp = d.widgets[k];
							if (temp.depth > w.depth) {
								dcData.AddW(temp);
							}
						}
						for (int k = 0; k < dcData.widgets.Count; k++) {
							d.RemoveW(dcData.widgets[k]);
						}
						return;
					}
				}
			}
		}
		CreateEmptyDC(dcList.Count, w);
	}
	private static void CreateEmptyDC(int index, UIWidget w) {
		SimpleDC dc = new SimpleDC(true, null, w.panel);
		dcList.Insert(index, dc);
		DrawCallData dcData = new DrawCallData();
		dcData.simple = dc;
		dcData.AddW(w);
		allWidgetDic.Add(dc, dcData);
	}
	private static int GetDCIndex(UIDrawCall dc)
	{
		for (int i = 0; i < dcList.Count; i++)
		{
			if (!dcList[i].isEmpty && dcList[i].dc == dc)
			{
				return i;
			}
		}
		return -1;
	}
	private static bool ChangeWidgetDepth(UIWidget widget,int drawcallIndex)
	{
		SimpleDC curDC = dcList[drawcallIndex];
		var curData = allWidgetDic[curDC];
		if (curData.widgets.Count == 0)
			return false;
		int _end = GetDCIndex(widget.drawCall);
		if (_end == -1) return false;
		for (int j = _end; j > drawcallIndex; j--)
		{
			var dcData = allWidgetDic[dcList[j]];
			foreach (var w in dcData.widgets)
			{
				if (CheckWidgetCollider(widget, w))
				{
					if (j == _end) //自己层的drawcall  widget交错
					{
						if (widget.depth > w.depth) //层级比别人高 不改变depth		
						{
							return false;
						}
					}
					else
					{
						return false;
					}

				}

			}
		}
		
		int depth = curData.DepthStart;
		foreach (var w in curData.widgets)
		{
			if (CheckWidgetCollider(widget, w))
			{
				if (w.depth >= depth)
					depth = w.depth + 1;
			}
		}
		
		var endData = allWidgetDic[dcList[_end]];
		endData.RemoveW(widget);

		curData.AddW(widget);
		int oldDepth = widget.depth;
		widget.depth = depth;
		AddChange(widget, oldDepth, depth,"[合并dc]",curData.simple.name);
		return true;
	}
	private static void ChangeWidgetDepth(UIWidget widget) {
		for (int i = 0; i < dcList.Count; i++) {
			SimpleDC dc = dcList[i];
			if (dc.dc != null && widget.drawCall == dc.dc)
				return;
			if (dc.dc != null && widget.drawCall != null && widget.drawCall != dc.dc && widget.drawCall.manager == dc.dc.manager && widget.drawCall.baseMaterial == dc.dc.baseMaterial)
			{
				if (ChangeWidgetDepth(widget, i))
				{
					return;
				}
			}
		}
	}
	private static bool CheckWidgetCollider(UIWidget w1, UIWidget w2)
	{
		Vector3[] corners = w1.worldCorners;
		Vector3[] corners2 = w2.worldCorners;
		float Xc1 = Mathf.Max(corners[0].x, corners2[0].x);

		float Yc1 = Mathf.Max(corners[0].y, corners2[0].y);

		float Xc2 = Mathf.Min(corners[2].x, corners2[2].x);

		float Yc2 = Mathf.Min(corners[2].y, corners2[2].y);

		if (Xc1 < Xc2 && Yc1 < Yc2)
		{
			return true;
		}
		/*Vector3 b0 = corners2[0];
		Vector3 b2 = corners2[2];
		if (corners[0] == corners2[0] && corners[1] == corners2[1] && corners[2] == corners2[2] && corners[3] == corners2[3])
		{
			return true;
		}
		for (int i = 0; i < corners.Length; i++)
		{
			Vector3 a = corners[i];
			if (a.x > b0.x && a.x < b2.x && a.y > b0.y && a.y < b2.y)
			{
				return true;
			}
		}
		b0 = corners[0];
		b2 = corners[2];
		for (int i = 0; i < corners2.Length; i++)
		{
			Vector3 a = corners2[i];
			if (a.x > b0.x && a.x < b2.x && a.y > b0.y && a.y < b2.y)
			{
				return true;
			}
		}*/
		return false;
	}
	private static void AddChange(UIWidget widget, int oldDepth, int newDepth,string flag = "",string dcName = "")
	{
		
		if (changeDepthDic.ContainsKey(widget.GetInstanceID()))
		{
			var data = changeDepthDic[widget.GetInstanceID()];
			data.lastDepth = newDepth;
			data.flag = flag;
			data.dcName = dcName;
		}
		else
		{
			var data = new DepthChangeData(widget, oldDepth, newDepth);
			data.flag = flag;
			data.dcName = dcName;
			changeDepthDic.Add(widget.GetInstanceID(), data);
		}
	}
	private static void StartCombine()
	{
		
		InitAllWidgets();
		changeDepthDic.Clear();

		Dictionary<SimpleDC, DrawCallData> tempAllWidgetDic = new Dictionary<SimpleDC, DrawCallData>();
		foreach (var value in allWidgetDic)
		{
			tempAllWidgetDic.Add(value.Key, value.Value.Clone());
		}
		foreach (var value in tempAllWidgetDic)
		{
			foreach (var widget in value.Value.widgets)
			{
				ChangeWidgetDepth(widget);
			}
		}

		Dictionary<UIPanel,List<SimpleDC>> tempDic = new Dictionary<UIPanel, List<SimpleDC>>();
		for (int i = 0; i < dcList.Count; i++) {
			var dc = dcList[i];
			if (!tempDic.ContainsKey(dc.manager)) {
				tempDic.Add(dc.manager, new List<SimpleDC>());
			}
			var list = tempDic[dc.manager];
			list.Add(dc);
		}

		foreach (var value in tempDic) {
			int maxDepth = int.MinValue;
			for (int i = 0; i < value.Value.Count; i++) {
				if (i != 0)
				{
					var data = allWidgetDic[value.Value[i - 1]];
					if (maxDepth < data.DepthEnd)
					{
						maxDepth = data.DepthEnd;
					}
					if (maxDepth > int.MinValue)
					{
						var data2 = allWidgetDic[value.Value[i]];
						int min = data2.DepthStart;
						int sub = maxDepth - min;
						if (sub >= 0)
						{
							foreach (var w in data2.widgets)
							{
								int newDepth = w.depth + sub + 1;
								AddChange(w, w.depth, newDepth, "[dc整体后移]",data2.simple.name);
								w.depth = newDepth;
							}
							data2.SetDirty(true);
						}
					}
				}
			}
		}

	}
	private static void GetHideObject(GameObject go, List<GameObject> list)
	{
		if (go == null)
			return;
		if (!go.activeSelf)
		{
			if(!list.Contains(go))
				list.Add(go);
		}
		for (int i = 0; i < go.transform.childCount; i++)
		{
			Transform child = go.transform.GetChild(i);
			GetHideObject(child.gameObject, list);
		}
	}
	private static void InitAllHide() {
		if (!isInitHide) {
			hideList.Clear();
			isInitHide = true;
			GetHideObject(selectObject, hideList);
		}
	}
	private static void ChangeHideObjShow(bool isShow)
	{
		for (int i = 0; i < hideList.Count; i++)
		{
			if (hideList[i] != null)
			{
				hideList[i].SetActive(isShow);
			}
		}
	}
	private void AddDeltaDepth(DrawCallData data, int depth) {
		for (int i = 0; i < data.widgets.Count; i++) {
			UIWidget widget = data.widgets[i];
			AddChange(widget, widget.depth, widget.depth + depth, "[Add]", data.simple.name);
			widget.depth = widget.depth + depth;
		}
		data.SetDirty(true);
		depthChangeExpand = true;
	}
	private void ShrinkDepth(DrawCallData data)
	{
		int start = data.DepthStart;
		UIWidget widget;
		UIWidget w;
		int maxDepth = 0;
		for (int i = 0; i < data.widgets.Count; i++)
		{
			maxDepth = start;
			widget = data.widgets[i];
			for (int j = 0; j < i; j++)
			{
				w = data.widgets[j];
				if (widget.depth != w.depth && w.depth >= maxDepth && CheckWidgetCollider(widget, w))
				{
					maxDepth = w.depth + 1;
				}
			}
			if (widget.depth > maxDepth)
			{
				AddChange(widget, widget.depth, maxDepth, "[shrink]" ,data.simple.name);
				widget.depth = maxDepth;
			}
		}
		data.SetDirty(true);
		data.Sort();
		depthChangeExpand = true;
	}
	void OnGUI()
	{

		EditorGUILayout.BeginVertical();
		EditorGUILayout.Space();
		bool oldAllHide = mShowAllHide;
		mShowAllHide = EditorGUILayout.Toggle("显示隐藏节点", mShowAllHide, GUILayout.Width(200));
		if (oldAllHide != mShowAllHide)
		{
			InitAllHide();
			ChangeHideObjShow(mShowAllHide);
		}

		EditorGUILayout.Space();
		GUI.color = new Color(0.2f, 1,0.5f, 1);
		EditorGUILayout.LabelField("[提示] ：");
		EditorGUILayout.LabelField("1、 空图集: 空的texture、sprite、label以及点击事件collider");
		EditorGUILayout.LabelField("[注意] ：");
		EditorGUILayout.LabelField("1、 50到60之间为动态替换图集depth,如果有动态图集，要预留一下！！！");
		EditorGUILayout.LabelField("2、 查看drawcall，有标红的，自动合并depth会整体后移，有可能层级错误，depth不要前后交叉");
		GUI.color = Color.white;
		EditorGUILayout.Space();
		if (GUILayout.Button("查看drawcall"))
		{
			InitAllWidgets();
			preAllWidgetDic.Clear();
			foreach (var value in allWidgetDic)
			{
				var clone = value.Value.Clone();
				clone.Sort();
				preAllWidgetDic.Add(value.Key, clone);
			}
			preCallChangeExpand = true;
			depthChangeExpand = false;
		}
		EditorGUILayout.Space();
		if (GUILayout.Button("合并")) {
			StartCombine();
			preAllWidgetDic.Clear();
			preCallChangeExpand = false;
			depthChangeExpand = true;
		}
		EditorGUILayout.Space();
		EditorGUILayout.EndVertical();
		DrawPreCall();
		DrawChange();
	}
	public void OnDestroy()
	{
		ChangeHideObjShow(false);
		preAllWidgetDic.Clear();
		allWidgetDic.Clear();
		dcList.Clear();
		hideList.Clear();
		emptyMatList.Clear();
	}
	private bool preCallChangeExpand = false;
	private bool preCallShowScroll = false;
	private Vector2 preCallScrollPos = Vector2.zero;
	private int preCallAddDepth = 0;
	private int preCallStartDepth = 0;
	private Color highligh = new Color(1, 1, 0.5f, 1);
	private Color selectColor = new Color(0, 1, 1, 1);
	public void DrawPreCall()
	{
		preCallChangeExpand = EditorGUILayout.Foldout(preCallChangeExpand, "draw call count:" + preAllWidgetDic.Count);
		if (preCallChangeExpand && preAllWidgetDic.Count > 0)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("选中:",GUILayout.Width(50));
			EditorGUILayout.LabelField("图集:", GUILayout.Width(130));
			EditorGUILayout.LabelField("panel:", GUILayout.Width(100));
			EditorGUILayout.LabelField("层级start->end:", GUILayout.Width(150));
			EditorGUILayout.LabelField("控件:", GUILayout.Width(200));
			EditorGUILayout.EndHorizontal();
			float height = preAllWidgetDic.Count * 30 + 10;
			preCallShowScroll = height > 300 ? true : false;
			height = height > 300 ? 300 : height;
			if (preCallShowScroll)
				preCallScrollPos = EditorGUILayout.BeginScrollView(preCallScrollPos, false, true, GUILayout.Width(1000), GUILayout.Height(height));
			bool hasSelect = false;
			bool singleSelect = false;
			int preStart = int.MinValue;
			int preEnd = int.MinValue;
			foreach (var dc in dcList)
			{
				var data = preAllWidgetDic[dc];
				EditorGUILayout.BeginHorizontal();
				if (dc.isSelect)
					GUI.color = highligh;
				else
					GUI.color = Color.white;
				dc.isSelect = EditorGUILayout.Toggle(dc.isSelect, GUILayout.Width(50));
				EditorGUILayout.LabelField(dc.name, GUILayout.Width(130));
				EditorGUILayout.ObjectField(dc.manager, typeof(UIPanel), false, GUILayout.Width(100));

				Color oldC = GUI.color;
				if ((data.DepthStart >= preStart && data.DepthStart <= preEnd) || (data.DepthEnd >= preStart && data.DepthEnd <= preEnd))
				{
					GUI.color = Color.red;
				}
				EditorGUILayout.LabelField("depth:" + data.DepthStart + " -> " + data.DepthEnd, GUILayout.Width(140));
				GUI.color = oldC;
				preStart = data.DepthStart;
				preEnd = data.DepthEnd;
				EditorGUILayout.LabelField("", GUILayout.Width(10));
				EditorGUILayout.BeginVertical(GUILayout.Width(200));
				
				if (data.widgets.Count > 0)
				{
					dc.isExpand = EditorGUILayout.Foldout(dc.isExpand, "widgets "+data.widgets.Count);
					if (dc.isExpand)
					{
						/*height = data.widgets.Count * 20;
						dc.isUseScroll = height > 500 ? true : false;
						height = height > 500 ? 500 : height;
						if (dc.isUseScroll)
							dc.scrollPos = EditorGUILayout.BeginScrollView(dc.scrollPos, false, true, GUILayout.Width(700), GUILayout.Height(height));*/
						foreach (var widget in data.widgets)
						{
							EditorGUILayout.BeginHorizontal();
							Color old = GUI.color;
							if (widget != null && Selection.activeGameObject == widget.gameObject)
								GUI.color = selectColor;
							EditorGUILayout.ObjectField(widget, typeof(UIWidget), false, GUILayout.Width(200));
							if (widget != null)
							{
								EditorGUILayout.LabelField(GetPath(dc.manager.transform, widget.transform));
								if (Selection.activeGameObject == widget.gameObject)
									GUI.color = old;
							}
							EditorGUILayout.EndHorizontal();
						}
						//if (dc.isUseScroll)
						//	EditorGUILayout.EndScrollView();
					}
				}
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
				if (dc.isSelect)
				{
					if (hasSelect)
						singleSelect = false;
					else
						singleSelect = true;
					hasSelect = true;

				}
				
			}
			if (preCallShowScroll)
				EditorGUILayout.EndScrollView();

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			if (hasSelect)
			{
				GUI.color = highligh;
				EditorGUILayout.LabelField("dc元素统一处理：");
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("缩小depth间隔:", GUILayout.Width(110)))
				{
					changeDepthDic.Clear();
					foreach (var dc in dcList)
					{
						if (dc.isSelect)
						{
							var data = preAllWidgetDic[dc];
							ShrinkDepth(data);
						}
					}
				}
				EditorGUILayout.LabelField("", GUILayout.Width(50));
				EditorGUILayout.LabelField("层级delta+", GUILayout.Width(70));
				preCallAddDepth = EditorGUILayout.IntField(preCallAddDepth, GUILayout.Width(50));
				if (GUILayout.Button("确定:", GUILayout.Width(50)))
				{
					changeDepthDic.Clear();
					foreach (var dc in dcList)
					{
						if (dc.isSelect)
						{
							var data = preAllWidgetDic[dc];
							AddDeltaDepth(data,preCallAddDepth);
						}
					}
				}
				if (singleSelect)
				{
					EditorGUILayout.LabelField("", GUILayout.Width(50));
					EditorGUILayout.LabelField("层级Start =", GUILayout.Width(70));
					preCallStartDepth = EditorGUILayout.IntField(preCallStartDepth, GUILayout.Width(50));
					if (GUILayout.Button("确定:", GUILayout.Width(50)))
					{
						changeDepthDic.Clear();
						foreach (var dc in dcList)
						{
							if (dc.isSelect)
							{
								var data = preAllWidgetDic[dc];
								AddDeltaDepth(data, preCallStartDepth - data.DepthStart);
							}
						}
					}
				}
				
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				GUI.color = Color.white;
			}
		}
	}
	private bool showScroll = false;
	private Vector2 scrollPos = Vector2.zero;
	private bool depthChangeExpand = false;
	public void DrawChange()
	{
		depthChangeExpand = EditorGUILayout.Foldout(depthChangeExpand, "depth 变化 count:"+ changeDepthDic.Count.ToString());
		if (depthChangeExpand && changeDepthDic.Count > 0)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("图集:", GUILayout.Width(150));
			EditorGUILayout.LabelField("transform:", GUILayout.Width(110));
			EditorGUILayout.LabelField("depth:", GUILayout.Width(110));
			EditorGUILayout.EndHorizontal();

			float height = changeDepthDic.Count * 30 + 10;
			showScroll = height > 400 ? true : false;
			height = height > 400 ? 400 : height;
			if (showScroll)
				scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(900), GUILayout.Height(height));
			foreach (var v in changeDepthDic)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(v.Value.dcName, GUILayout.Width(150));
				EditorGUILayout.ObjectField(v.Value.widget, typeof(UIWidget), true, GUILayout.Width(110));
				EditorGUILayout.LabelField(v.Value.depthStr + v.Value.flag);
				EditorGUILayout.EndHorizontal();
			}

			if (showScroll)
				EditorGUILayout.EndScrollView();
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

		return GetPath(root, t.parent, path, false);
	}
	class SimpleDC
	{
		public bool isEmpty;
		public UIDrawCall dc;
		public UIPanel manager;
		public bool isExpand; //绘制用是否展开
		public bool isSelect;//绘制用
		public bool isUseScroll;//绘制用
		public Vector2 scrollPos;//绘制用
		public string name;
		public SimpleDC(bool isEmpty, UIDrawCall dc = null,UIPanel panel = null)
		{
			this.isEmpty = isEmpty;
			this.dc = dc;
			if (dc != null)
			{
				if (dc.name != "")
				{
					name = dc.name;
				}
				else
				{
					if (dc.dynamicMaterial != null)
						name = dc.dynamicMaterial.name;
				}
				manager = dc.manager;
			}
			else
			{
				name = "空";
				manager = panel;
			}
		}
	}
	class DepthChangeData
	{
		public int originDepth;
		public UIWidget widget;
		public string dcName;
		public string flag;
		public string depthStr;
		private int mLastDepth;
		public DepthChangeData(UIWidget widget, int originDepth, int lastDepth)
		{
			this.widget = widget;
			this.originDepth = originDepth;
			this.lastDepth = lastDepth;
			depthStr = originDepth + " -> " + lastDepth ;
		}
		public int lastDepth
		{
			set
			{
				mLastDepth = value;
				depthStr += " -> " + value;
			}
			get { return mLastDepth; }
		}
	}
	class DrawCallData
	{
		public SimpleDC simple;
		public List<UIWidget> widgets = new List<UIWidget>();
		private int mDepthStart;
		private int mDepthEnd;

		private bool isStartDirty = true;
		private bool isEndDirty = true;
		public DrawCallData Clone()
		{
			DrawCallData d = new DrawCallData();
			d.simple = simple;
			foreach (var w in widgets)
			{
				d.widgets.Add(w);
			}
			d.SetDirty(true);
			return d;
		}
		public bool Isvalid() {
			return widgets.Count > 0;
		}
		public void Sort()
		{
			widgets.Sort((a, b) =>
			{
				return a.depth - b.depth;
			});
		}
		public void AddW(UIWidget w)
		{
			widgets.Add(w);
			SetDirty(true);
		}
		public void RemoveW(UIWidget w)
		{
			widgets.Remove(w);
			SetDirty(true);
		}
		public void SetDirty(bool isDirty)
		{
			isStartDirty = isDirty;
			isEndDirty = isDirty;
		}
		public int DepthStart
		{
			get
			{
				if (isStartDirty) {
					isStartDirty = false;
					int depth = int.MaxValue;
					foreach (var w in widgets)
					{
						if (w.depth < depth)
						{
							depth = w.depth;
						}
					}
					mDepthStart = depth;
					
				}
				return mDepthStart;
			}
		}
		public int DepthEnd
		{
			get
			{
				if (isEndDirty) {
					isEndDirty = false;
					int depth = int.MinValue;
					foreach (var w in widgets)
					{
						if (w.depth > depth)
						{
							depth = w.depth;
						}
					}
					mDepthEnd = depth;
				}
				return mDepthEnd;
			}
		}
	}
}
