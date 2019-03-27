using UnityEngine;

[RequireComponent(typeof(UIInvisibleWidget))]
public class UIInVisibleSort : MonoBehaviour
{
    public bool isTop = true;
    public int renderQueue_ReadOnly;

    private Renderer[] _renderers;
    private UIInvisibleWidget _widget;
    private int defaultDelayFrameCount = 5;
    private int delayFrameCount = 0;

    public void SortTop()
    {
        int maxDepth = _widget.depth;
        if (isTop)
        {
            UIPanel parentPanel = transform.GetComponentInParent<UIPanel>();
            if (parentPanel)
                maxDepth = parentPanel.GetMaxUIWightDepth() + 1;
        }
        Sort(maxDepth);
    }

    public void Sort(int depth)
    {
        _widget.depth = depth;
    }

    void Awake()
    {
        _widget = gameObject.GetComponent<UIInvisibleWidget>();
        _renderers = gameObject.GetComponentsInChildren<Renderer>();
    }

    void Start()
    {
        delayFrameCount = defaultDelayFrameCount;
        SortTop();
    }

    void Update()
    {
        delayFrameCount--;
        if (delayFrameCount == 0)
        {
            return;
        }

        if (_widget == null || _widget.drawCall == null || _renderers == null)
        {
            return;
        }

        renderQueue_ReadOnly = _widget.drawCall.renderQueue;
        foreach(Renderer render in _renderers)
        {
            //render.sortingOrder = 0;
            render.material.renderQueue = renderQueue_ReadOnly;
        }
    }

    [ContextMenu("ReExcute")]
    void ReExcute()
    {
        delayFrameCount = defaultDelayFrameCount;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        delayFrameCount = defaultDelayFrameCount;
        if (_widget)
		{
			_widget.drawCall.renderQueue = renderQueue_ReadOnly;
		}
    }
#endif

}
