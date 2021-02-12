using System;
using System.Threading;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 共通フェード処理
/// </summary>
public class CommonFade
{
    public const float DEFAULT_FADE_TIME = 1f;

    /// <summary>
    /// フェード中かどうか
    /// </summary>
    public static bool IsPlayingFadeAnimation
    {
        get
        {
            return CommonMaskManager.Instance.ImageAlpha > 0f && CommonMaskManager.Instance.ImageAlpha < 1f;
        }
    }

    /// <summary>
    /// フェードアウト開始
    /// </summary>
    /// <param name="toColor">フェード先の色</param>
    /// <param name="time">フェード時間</param>
    /// <param name="onCompleteFade">フェード完了時処理</param>
    public static void StartFadeOut(Color toColor, float time = DEFAULT_FADE_TIME, Action onCompleteFade = null)
    {
        // フェードが既に終了している場合は即時完了処理を実行
        if (CommonMaskManager.Instance.ImageAlpha >= 1f)
        {
            onCompleteFade();
            return;
        }
        // フェード実行中はキャンセル
        else if (IsPlayingFadeAnimation) { return; }

        // フェード先のアルファ値を設定
        toColor.a = 1f;

        // フェード開始
        StartFade(toColor, time, onCompleteFade);
    }

    /// <summary>
    /// フェードアウト開始
    /// </summary>
    /// <param name="time">フェード時間</param>
    /// <param name="onCompleteFade">フェード完了時処理</param>
    public static void StartFadeOut(float time = DEFAULT_FADE_TIME, Action onCompleteFade = null)
    {
        // フェード開始
        StartFadeOut(Color.black, time, onCompleteFade);
    }

    /// <summary>
    /// フェードイン開始
    /// </summary>
    /// <param name="time">フェード時間</param>
    /// <param name="onCompleteFade">フェード完了時処理</param>
    public static void StartFadeIn(Color toColor, float time = DEFAULT_FADE_TIME, Action onCompleteFade = null)
    {
        // フェードが既に終了している場合は即時完了処理を実行
        if (CommonMaskManager.Instance.ImageAlpha <= 0f)
        {
            onCompleteFade();
            return;
        }
        // フェード実行中はキャンセル
        else if (IsPlayingFadeAnimation) { return; }

        // フェード先の色を設定
        toColor.a = 0f;

        // フェード開始
        StartFade(toColor, time, onCompleteFade);
    }

    /// <summary>
    /// フェードイン開始
    /// </summary>
    /// <param name="time">フェード時間</param>
    /// <param name="onCompleteFade">フェード完了時処理</param>
    public static void StartFadeIn(float time = DEFAULT_FADE_TIME, Action onCompleteFade = null)
    {
        // フェード開始
        StartFadeIn(Color.black, time, onCompleteFade);
    }

    /// <summary>
    /// フェード開始
    /// </summary>
    /// <param name="toColor">フェード先の色設定</param>
    /// <param name="time">フェード時間</param>
    /// <param name="onCompleteFade">フェード完了時処理</param>
    public static void StartFade(Color toColor, float time = DEFAULT_FADE_TIME, Action onCompleteFade = null)
    {
        // 時間設定がなければ即座に色を変更して完了処理を実行
        if (time <= 0f)
        {
            CommonMaskManager.Instance.ImageColor = toColor;
            onCompleteFade?.Invoke();
            return;
        }

        // 計算用のパラメータを設定
        float value = 0f;
        float rate = 1f / time;
        var resultColor = CommonMaskManager.Instance.ImageColor;
        var fromColor = resultColor;

        // フェード実行
        Observable.EveryUpdate()
            .TakeWhile(_ => value < 1f)
            .Subscribe(_ =>
            {
                // フェード更新処理
                value += rate * Time.deltaTime;
                resultColor = Color.Lerp(fromColor, toColor, value);
                CommonMaskManager.Instance.ImageColor = resultColor;
            },
            () =>
            {
                // フェード完了処理
                CommonMaskManager.Instance.ImageColor = toColor;
                onCompleteFade?.Invoke();
            });
    }
}
