using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIInvisibleWidget : UIWidget
{
    private Texture2D mTex;
    private Material mMat;

    protected override void Awake()
    {
        base.Awake();
        height = 0;
        width = 0;
        mTex = new Texture2D(0, 0);
        mChanged = false;
        fillGeometry = false;

        geometry.verts.Add(new Vector3(1f, 0, 0));
        geometry.verts.Add(new Vector3(2f, 0, 0));
        geometry.verts.Add(new Vector3(3f, 0, 0));
        geometry.verts.Add(new Vector3(4f, 0, 0));
        geometry.ApplyTransform(Matrix4x4.identity);
        geometry.uvs.Add(Vector2.zero);
        geometry.uvs.Add(Vector2.up);
        geometry.uvs.Add(Vector2.one);
        geometry.uvs.Add(Vector2.right);
        geometry.cols.Add(Color.white);
        geometry.cols.Add(Color.white);
        geometry.cols.Add(Color.white);
        geometry.cols.Add(Color.white);
    }

    public override Texture mainTexture
    {
        get
        {
            return mTex;
        }
        set
        {

        }
    }

    public override Material material
    {
        get
        {
            return mMat;
        }
        set
        {

        }
    }
}