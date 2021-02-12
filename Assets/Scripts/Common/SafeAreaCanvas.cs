using UnityEngine;

/// <summary>
/// セーフエリア
/// └RectTransformを端末のセーフエリアに合わせる
/// </summary>
public class SafeAreaCanvas : MonoBehaviour
{
    private const bool IsScreenRotatable = false;

    [SerializeField]
    private RectTransform _rectTransform = null;
    [SerializeField]
    private Canvas _canvas = null;

    /// <summary>
    /// 生成時（MonoBehaviour）
    /// </summary>
    private void Awake()
    {
        CheckRectTransform();
        CheckCanvas();

        Resize();
    }

    private void Start()
    {
        CheckMainCamera();
    }

    /// <summary>
    /// 更新時（MonoBehaviour）
    /// </summary>
    private void Update()
    {
        // セーフエリアが変更された場合にリサイズ
        if (IsScreenRotatable && _rectTransform.rect != Screen.safeArea)
        {
            Resize();
        }

        CheckMainCamera();
    }

    /// <summary>
    /// セーフエリアのサイズをリサイズ
    /// </summary>
    private void Resize()
    {
        var anchorMin = Screen.safeArea.position;
        var anchorMax = Screen.safeArea.position + Screen.safeArea.size;

        anchorMin.x = anchorMin.x / Screen.width;
        anchorMin.y = anchorMin.y / Screen.height;
        anchorMax.x = anchorMax.x / Screen.width;
        anchorMax.y = anchorMax.y / Screen.height;

        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;
    }

    /// <summary>
    /// RectTransformチェック
    /// </summary>
    private void CheckRectTransform()
    {
        if (_rectTransform != null) { return; }

        var rectTransform = GetComponentInChildren<RectTransform>();
        if (rectTransform != null)
        {
            _rectTransform = FindObjectOfType<RectTransform>();
            if (_rectTransform == null)
            {
                Debug.LogErrorFormat("[ERROR]Not found RectTransform | {0}", gameObject.scene.name);
            }
        }

        _rectTransform = rectTransform;
    }

    /// <summary>
    /// Canvasチェック
    /// </summary>
    private void CheckCanvas()
    {
        if (_canvas != null) { return; }

        var targetCanvas = GetComponent<Canvas>();
        if (targetCanvas != null)
        {
            targetCanvas = FindObjectOfType<Canvas>();
            if (targetCanvas == null)
            {
                Debug.LogErrorFormat("[ERROR]Not found Canvas | {0}", gameObject.scene.name);
            }
        }

        _canvas = targetCanvas;
    }

    /// <summary>
    /// メインカメラチェック
    /// </summary>
    private void CheckMainCamera()
    {
        if (_canvas == null)
        {
            Debug.LogErrorFormat("[ERROR]Not found Canvas | {0}", gameObject.scene.name);
            return;
        }
        if (_canvas.worldCamera != null) { return; }

        if (_canvas.worldCamera == null && MainCameraManager.Instance != null)
        {
            _canvas.worldCamera = MainCameraManager.Instance.MainCamera;
        }
    }
}
