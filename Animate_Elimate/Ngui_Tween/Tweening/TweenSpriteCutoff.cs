using UnityEngine;

/// <summary>
/// Tween the Sprite's cutoff.
/// </summary>
public class TweenSpriteCutoff : UITweener
{
    public float CutOffWidthFrom;
    public float CutOffWidthTo;
    public float CutOffHeightFrom;
    public float CutOffHeightTo;

    private bool _cached;
    private UIBasicSprite _sprite;

    private void Cache()
    {
        _cached=true;
        _sprite=this.GetComponent<UIBasicSprite>();
    }

    public float CutOffWidthValue
    {
        set
        {
            if(!_cached)
            {
                this.Cache();
            }
            _sprite.CutOffSquareWidth=value;
        }
        get
        {
            if(!_cached)
            {
                this.Cache();
            }
            return _sprite.CutOffSquareWidth;
        }
    }
    public float CutOffHeightValue
    {
        set
        {
            if(!_cached)
            {
                this.Cache();
            }
            _sprite.CutOffSquareHeight=value;
        }
        get
        {
            if(!_cached)
            {
                this.Cache();
            }
            return _sprite.CutOffSquareHeight;
        }
    }

    protected override void OnUpdate(float factor,
                                     bool isFinished)
    {
        this.CutOffWidthValue=Mathf.Lerp(CutOffWidthFrom,
                                         CutOffWidthTo,
                                         factor);
        this.CutOffHeightValue=Mathf.Lerp(CutOffHeightFrom,
                                          CutOffHeightTo,
                                          factor);
    }

}