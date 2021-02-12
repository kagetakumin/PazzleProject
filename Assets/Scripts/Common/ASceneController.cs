using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class ASceneController : MonoBehaviour
{
    [SerializeField]
    protected SceneName.Type _sceneType = 0;

    /// <summary>
    /// シーンタイプ
    /// </summary>
    public SceneName.Type SceneType => _sceneType;

    /// <summary>
    /// MonobehaviourのAwake実行時
    /// </summary>
    protected virtual void OnAwakeScene() { }

    /// <summary>
    /// MonobehaviourのStart実行時
    /// </summary>
    protected virtual void OnStartScene() { }

    /// <summary>
    /// シーンの初期化処理（読み込み完了時）
    /// </summary>
    public virtual void SceneInitialize() { }

    /// <summary>
    /// シーンの終了処理（削除直前処理）
    /// </summary>
    public virtual void SceneFinalize() { }

    /// <summary>
    /// このシーンへの遷移完了時（フェード等を含む）処理
    /// </summary>
    public virtual void OnCompleteSceneTransition() { }

    private void Awake()
    {
        OnAwakeScene();
    }

    private void Start()
    {
        if (!SceneLoadManager.Instance.IsLoadedScene(_sceneType))
        {
            SceneLoadManager.Instance.RegisterLoadedScene(_sceneType, this);
        }

        OnStartScene();
    }
}
