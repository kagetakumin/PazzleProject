using UnityEditor;
/// <summary>
/// ベースシーン制御
/// </summary>
public class BaseSceneController : ASceneController
{
    protected override void OnStartScene()
    {
        base.OnStartScene();

        SceneTransitionManager.Instance.Transition(SceneType + 1);
    }

    protected override void OnAwakeScene()
    {
        base.OnAwakeScene();

        DontDestroyOnLoad(gameObject);
    }
}
