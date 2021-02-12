using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン遷移管理
/// </summary>
public class SceneTransitionManager : SingletonMonoBehaviour<SceneTransitionManager>
{
    /// <summary>
    /// シーン遷移
    /// </summary>
    /// <param name="toSceneType">遷移先のシーンタイプ</param>
    /// <param name="isLoadAdditive">true:追加読み込み, false:他のシーンを解放して読み込み</param>
    /// <param name="onCompleteLoad">読み込み完了時</param>
    /// <param name="onCompleteTransition">遷移完了時（フェード終了時）</param>
    public void Transition(SceneName.Type toSceneType, bool isAdditive = false, Action onCompleteLoad = null, Action onCompleteTransition = null)
    {
        // フェードアウト後のシーン読み込み処理を設定
        Action loadSceneAction = () =>
        {

            // シーン読み込み処理
            SceneLoadManager.Instance.LoadSceneAsync(toSceneType, () =>
            {
                // シーン読み込み完了時処理を実行
                onCompleteLoad?.Invoke();

                // フェードイン開始
                CommonFade.StartFadeIn(onCompleteFade: onCompleteTransition);
            });
        };
        if (isAdditive)
        {
            SceneLoadManager.Instance.UnloadAllScene();
        }

        // フェードアウト開始
        CommonFade.StartFadeOut(onCompleteFade: () =>
        {
            // シーン読み込み処理を実行
            loadSceneAction();
        });
    }
}