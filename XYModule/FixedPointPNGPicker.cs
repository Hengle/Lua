// ************************************************
// Solution Name : XianJian-SoftStar
// File Name     : FixedPointPNGPicker.cs
//
// Created By gaoweiya@softstar.sh.cn
// 2015_04_29
// ************************************************

using System;
using SoftStar.GWY.RM;
using UnityEngine;

[RequireComponent(typeof(UITexture))]
public class FixedPointPNGPicker : DisableItem
{
    public string _preABName=string.Empty;
    private UITexture _textureContainer;
    public float _releaseDelay;
    public bool _asyncLoad;
    public int _requestID;
    private AssetLoadRequest.OnLoadAsset _onLoadAsset;
    /// <summary>
    /// =0 未设置；=1 texture；=2 material；未设置的情况下需要处理一些特殊问题
    /// </summary>
    byte _textureFlag;

    public enum MaterialUsage
    {
        //使用材质
        MATERIAL,
        //使用材质的主贴图
        MAIN_TEXTURE,
        //把材质的两个贴图分别赋值到已有材质对应的属性上
        TEXTURES,
    }

    public UITexture GetTextureContainer()
    {
        return _textureContainer;
    }

    public void Init(string abName,
                     bool makePixelPerfect,
                     float releaseDelay,
                     bool asyncLoad,
                     AssetLoadRequest.OnLoadAsset onLoadAsset,
                     MaterialUsage usage)
    {
        _releaseDelay = releaseDelay;
        _asyncLoad = asyncLoad;
        _onLoadAsset = onLoadAsset;
        SetTexture(abName,
                        makePixelPerfect,
                        usage);
    }

    public void Uninit()
    {
        if(string.IsNullOrEmpty(_preABName))
        {
            return;
        }
        ClearPre();
        _textureFlag = 0;
        _textureContainer =null;
        _preABName=string.Empty;
    }
	
    private void OnDestroy()
    {
        Uninit();
		_onLoadAsset = null;
	}

    private void ClearPre()
    {
        if(!string.IsNullOrEmpty(_preABName))
        {
            if (_textureContainer != null)
            {
                if(_textureFlag == 1)
                {
                    _textureContainer.mainTexture = null;
                }else if(_textureFlag == 2)
                {
                    _textureContainer.material = null;
                    _textureContainer.shader = null;
                }
            }

            if (_asyncLoad)
            {
                if (_requestID != 0)
                {
                    AssetManager.CancelLoadObjectRequest(_requestID, _preABName);
                    _requestID = 0;
                }                
                else
                {
                    AssetManager.ReleaseObjectDelay(_preABName, _releaseDelay);
                }
            }
            else
            {
                AssetManager.ReleaseObjectDelay(_preABName, _releaseDelay);
            }

            _preABName = string.Empty;
        }
    }

    private void SetTexture(string abName,
                            bool makePixelPerfect, 
                            MaterialUsage usage)
    {
        if(string.IsNullOrEmpty(abName))
        {
            ClearPre();
            return;
        }
        if (abName == _preABName)
        {
            return;
        }
        //abName=abName.ToLower();
        //if(string.Compare(abName,
        //                  _preABName,
        //                  StringComparison.InvariantCultureIgnoreCase)==0)
        //{
        //    return;
        //}
        //加载贴图不同的话，首先卸载一下旧的，以防止短时间内设置两次导致的加载异常
        //举例：原图路径=A，此时加载路径=B的图片，在B未加载完的时候，又设置加载路径=A的图片，这样由于_preABName==abName，会加载无效；
        //所以这里先清空一下 by:lml
        Uninit();
        EnableDisable();

        _textureContainer=this.GetComponent<UITexture>();
        if (_asyncLoad)
        {
            _preABName = abName; //这里_preABName需要先赋值，如果等加载回来再赋值(连续加载多个图片时会出问题)
            var assetType = AssetManager.AssetType.Object;
            //if (useOriginalMaterial)
            //{
            //    assetType = AssetManager.AssetType.Material;
            //}
            _requestID = AssetManager.LoadObjectAsyncSpec(0, abName, 0,
                (assetName, obj, loadRequestID, instanceID) =>
                {
                    _requestID = 0;
                    ApplyTexture(obj, makePixelPerfect, abName, assetName, loadRequestID, usage);
                }
            , assetType, false);
        }
        else
        {
            var obj = AssetManager.LoadObject(abName, 0);
            if (obj == null)
            {
                //if (GameLogger.IsEnable) GameLogger.Error("not exists." + abName);
                return;
            }
            ApplyTexture(obj, makePixelPerfect, abName, null, 0, usage);
        }
    }

    void ApplyTexture(UnityEngine.Object texture, bool makePixelPerfect, string abName, string assetName, int loadRequestID, MaterialUsage usage)
	{
		if (texture == null)
		{
			if (_onLoadAsset != null) 
				_onLoadAsset(assetName, null, loadRequestID, 0);
			return;
		}
        //ClearPre();
        if (texture is Texture)
        {
            _textureFlag = 1;
            _textureContainer.mainTexture = (Texture)texture;
        }
        else if (texture is Material)
        {
            Material mat = (Material)texture;
            if (usage == MaterialUsage.MAIN_TEXTURE)
            {
                _textureContainer.mainTexture = mat.mainTexture;
                _textureFlag = 1;
            }else if (usage==MaterialUsage.TEXTURES)
            {
                string mainTextureName = "_MainTex";
                _textureContainer.material.SetTexture(mainTextureName, mat.GetTexture(mainTextureName));
                string alphaTextuerName = "_MainTex_Alpha";
                _textureContainer.material.SetTexture(alphaTextuerName, mat.GetTexture(alphaTextuerName));
                _textureContainer.mainTexture = mat.mainTexture;
                _textureFlag = 1;
            }
            else if(usage==MaterialUsage.MATERIAL)
            {
                _textureContainer.material = mat;
                _textureFlag = 2;
            }
        }
        if (makePixelPerfect)
        {
            _textureContainer.MakePixelPerfect();
        }
        if (_onLoadAsset != null)
        {
            _onLoadAsset(assetName, texture, loadRequestID, 0);
        }
        _preABName = abName;
    }
}