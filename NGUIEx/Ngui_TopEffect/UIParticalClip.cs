using UnityEngine;
using System.Collections;

public class UIParticalClip : DisableItem
{

    private UIPanel mPanel;
    private Renderer[] mSenderers;
    private bool mRefresh = false;
    void Start()
    {
        mRefresh = false;
        mPanel = gameObject.GetComponentInParent<UIPanel>();
        mSenderers = gameObject.GetComponentsInChildren<Renderer>();

        if (mPanel != null)
        {
            mPanel.onClipMove += SetClip;
        }
    }

    void Update()
    {
        if (mRefresh == false)
        {
            mRefresh = true;
            SetClip(mPanel);
        }
    }

    void SetClip(UIPanel panel)
    {
        if (panel == null)
        {
            return;
        }
        if (mSenderers == null)
        {
            return;
        }
        Vector2 soft = panel.clipSoftness;

        var panelWorldCorners = panel.worldCorners;
        var leftBottom = panelWorldCorners[0];
        var rightTop = panelWorldCorners[2];
        var center = Vector3.Lerp(leftBottom, rightTop, 0.5f);
        var z = rightTop.x - leftBottom.x;
        var w = rightTop.y - leftBottom.y;
        var cr = new Vector4(center.x, center.y, z * 0.5f, w * 0.5f);

        Vector2 sharpness = new Vector2(1000.0f, 1000.0f);
        //if (soft.x > 0f) sharpness.x = cr.z / soft.x;
        //if (soft.y > 0f) sharpness.y = cr.w / soft.y;

        foreach (Renderer render in mSenderers)
        {
            if (render != null)
            {
                render.material.SetVector("_ClipRange0", new Vector4(-cr.x / cr.z, -cr.y / cr.w, 1f / cr.z, 1f / cr.w));
                render.material.SetVector("_ClipArgs0", new Vector2(sharpness.x, sharpness.y));
            }
        }
    }
}
