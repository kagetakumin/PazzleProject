using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PazzlePiece : MonoBehaviour
{
    [SerializeField]
    private Image _backgroundImage = null;
    [SerializeField]
    private Text _valueText = null;
    [SerializeField]
    private Color _textColorLight = Color.white;
    [SerializeField]
    private Color _textColorDark = Color.black;

    private int Value { get; set; } = 0;

    private int ViewValue
    {
        get
        {
            return 1 << Value;
        }
    }

    public int PosX { get; set; } = 0;
    public int PosY { get; set; } = 0;

    public Color BackgroundColor
    {
        get { return _backgroundImage.color; }
        set
        {
            _backgroundImage.color = value;
            var gray = value.grayscale;
            if (gray > 0.5f && _valueText.color != _textColorDark)
            {
                _valueText.color = _textColorDark;
            }
            else if (_valueText.color != _textColorLight)
            {
                _valueText.color = _textColorLight;
            }
        }
    }

    public int Score
    {
        get { return Value; }
        set
        {
            Value = value;
            _valueText.text = ViewValue.ToString();
            gameObject.SetActive(Value > 0);
        }
    }

    public void Initialize()
    {
        BackgroundColor = Color.white;
        Score = 0;
    }
}
