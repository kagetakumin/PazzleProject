using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 初期化シーン読み込み
/// </summary>
public class InitializeSceneLoader
{
    /// <summary>
    /// 初期化処理（シーン読み込み前に実行）
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        int loadSceneCount = 0;
        var activeScene = SceneManager.GetActiveScene();
        for (int i = 0; i < SceneLoadManager.FixSceneTypeList.Count; ++i)
        {
            var loadSceneAsyncOperation = SceneManager.LoadSceneAsync(SceneName.NameList[SceneLoadManager.FixSceneTypeList[i]], i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive);
            loadSceneAsyncOperation.completed += _ =>
            {
                ++loadSceneCount;
                if (loadSceneCount < SceneLoadManager.FixSceneTypeList.Count) { return; }
            };
        }
    }
}
