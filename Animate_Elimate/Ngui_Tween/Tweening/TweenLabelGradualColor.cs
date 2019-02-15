//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using System;
using UnityEngine;

/// <summary>
///   Tween the object's color.
/// </summary>
#pragma warning disable 0618,0219,0414
[AddComponentMenu("NGUI/Tween/Tween Label Gradual Color")]
public class TweenLabelGradualColor : UITweener
{
    public Color from1=Color.white;
    public Color from2=Color.white;
    private bool mCached;
    private UILabel mLabel;
    public Color to1=Color.white;
    public Color to2=Color.white;
    [Obsolete("Use 'value' instead")]

    /// <summary>
    ///   Tween's current value.
    /// </summary>
    public Color gradientTop
    {
        get
        {
            if(!mCached)
            {
                this.Cache();
            }
            if(mLabel!=null)
            {
                return mLabel.gradientTop;
            }
            return Color.black;
        }
        set
        {
            if(!mCached)
            {
                this.Cache();
            }
            if(mLabel!=null)
            {
                mLabel.gradientTop=value;
            }
        }
    }
    public Color gradientBottom
    {
        get
        {
            if(!mCached)
            {
                this.Cache();
            }
            if(mLabel!=null)
            {
                return mLabel.gradientBottom;
            }
            return Color.black;
        }
        set
        {
            if(!mCached)
            {
                this.Cache();
            }
            if(mLabel!=null)
            {
                mLabel.gradientBottom=value;
            }
        }
    }

    private void Cache()
    {
        mCached=true;
        mLabel=this.GetComponent<UILabel>();
        if(mLabel!=null) {}
    }

    /// <summary>
    ///   Tween the value.
    /// </summary>
    protected override void OnUpdate(float factor,
                                     bool isFinished)
    {
        this.gradientTop=Color.Lerp(from1,
                                    from2,
                                    factor);
        this.gradientBottom=Color.Lerp(to1,
                                       to2,
                                       factor);
        mLabel.MarkAsChanged();
    }

    /// <summary>
    ///   Start the tweening operation.
    /// </summary>
    public static TweenLabelGradualColor Begin(GameObject go,
                                               float duration,
                                               Color from,
                                               Color to)
    {
#if UNITY_EDITOR
        if(!Application.isPlaying)
        {
            return null;
        }
#endif
        var comp=Begin<TweenLabelGradualColor>(go,
                                               duration);
        comp.from1=comp.gradientTop;
        comp.to1 = comp.gradientBottom;
        comp.from2 = from;
        comp.to2 = to;
        if(duration<=0f)
        {
            comp.Sample(1f,
                        true);
            comp.enabled=false;
        }
        return comp;
    }

    [ContextMenu("Set 'From' to current value")]
    public override void SetStartToCurrentValue()
    {
        from1=this.gradientTop;
        to1=this.gradientBottom;
    }

    [ContextMenu("Set 'To' to current value")]
    public override void SetEndToCurrentValue()
    {
        from2=this.gradientTop;
        to2=this.gradientBottom;
    }

    [ContextMenu("Assume value of 'From'")]
    private void SetCurrentValueToStart()
    {
        this.gradientTop=from1;
        this.gradientBottom=to1;
    }

    [ContextMenu("Assume value of 'To'")]
    private void SetCurrentValueToEnd()
    {
        this.gradientTop=from2;
        this.gradientBottom=to2;
    }
}