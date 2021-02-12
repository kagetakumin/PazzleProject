using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonMaskManager : SingletonMonoBehaviour<CommonMaskManager>
{
    [SerializeField]
    private Image _maskImage = null;

    public Color ImageColor
    {
        get { return _maskImage.color; }
        set
        {
            if (_maskImage == null) { return; }
            _maskImage.color = value;
        }
    }

    public float ImageAlpha
    {
        get { return _maskImage.color.a; }
        set
        {
            if (_maskImage == null) { return; }
            var imageColor = _maskImage.color;
            imageColor.a = value;
            _maskImage.color = imageColor;
        }
    }
}
