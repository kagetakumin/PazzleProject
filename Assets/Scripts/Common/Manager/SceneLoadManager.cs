using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン読み込み管理
/// </summary>
public class SceneLoadManager : SingletonMonoBehaviour<SceneLoadManager>
{
    public static readonly List<SceneName.Type> FixSceneTypeList = new List<SceneName.Type>()
    {
        SceneName.Type.Base,
    };

    private bool isLoadedNotFixScene = false;
    private Dictionary<SceneName.Type, ASceneController> LoadedSceneList { get; set; } = new Dictionary<SceneName.Type, ASceneController>();
    private List<SceneName.Type> ActiveSceneList
    {
        get
        {
            return LoadedSceneList.Where(x => x.Value.gameObject.activeSelf)
                .Select(x => x.Key)
                .ToList();
        }
    }

    /// <summary>
    /// 固定シーンの読み込み済みかの判定
    /// </summary>
    public bool IsLoadedNotFixScene
    {
        get
        {
            if (isLoadedNotFixScene) { return true; }

            foreach(var sceneType in FixSceneTypeList)
            {
                if (!IsLoadedScene(sceneType)) { return false; }
            }

            isLoadedNotFixScene = true;
            return true;
        }
    }

    /// <summary>
    /// シーン読み込み完了時
    /// </summary>
    /// <param name="sceneType">読み込まれたシーン</param>
    private void OnCompleteLoadScene(SceneName.Type sceneType, Action onCompletedLoad)
    {
        // 読み込まれたシーンを登録
        RegisterLoadedScene(sceneType);

        // 読み込まれたシーンをアクティブ化
        ActivateScene(sceneType, true);

        // 完了時処理が設定されていれば実行
        onCompletedLoad?.Invoke();
    }

    /// <summary>
    /// シーン解放完了時
    /// </summary>
    /// <param name="sceneType">解放されたシーン</param>
    private void OnCompleteUnloadScene(SceneName.Type sceneType, Action onCompleteUnload)
    {
        // 解放したシーンをリストから削除
        LoadedSceneList.Remove(sceneType);

        // 完了時処理が設定されていれば実行
        onCompleteUnload?.Invoke();
    }

    /// <summary>
    /// シーン読み込み
    /// </summary>
    /// <param name="sceneType">読み込むシーンタイプ</param>
    /// <param name="isAdditive">true:追加読み込み, false:単発で読み込み</param>
    /// <param name="onCompleteLoad">読み込み完了時処理</param>
    public void LoadSceneAsync(SceneName.Type sceneType, Action onCompleteLoad = null)
    {
        // 既にシーン読み込みされている場合は完了時処理を実行して終了
        if (LoadedSceneList.ContainsKey(sceneType))
        {
            OnCompleteLoadScene(sceneType, onCompleteLoad);
            return;
        }

        // 非同期でシーン読み込みを実行
        var operation = SceneManager.LoadSceneAsync(SceneName.NameList[sceneType], LoadSceneMode.Additive);
        operation.completed += _ =>
        {
            // 完了時処理を実行
            OnCompleteLoadScene(sceneType, onCompleteLoad);
        };
    }

    /// <summary>
    /// シーン読み込み
    /// </summary>
    /// <param name="sceneType">読み込むシーンタイプ</param>
    /// <param name="isAdditive">true:追加読み込み, false:単発で読み込み</param>
    /// <param name="onCompleteLoad">読み込み完了時処理</param>
    public void LoadScene(SceneName.Type sceneType, Action onCompleteLoad = null)
    {
        // 既にシーン読み込みされている場合は完了時処理を実行して終了
        if (LoadedSceneList.ContainsKey(sceneType))
        {
            OnCompleteLoadScene(sceneType, onCompleteLoad);
            return;
        }

        // シーン読み込みを実行
        SceneManager.LoadScene(SceneName.NameList[sceneType], LoadSceneMode.Additive);

        // 完了時処理を実行
        OnCompleteLoadScene(sceneType, onCompleteLoad);
    }

    /// <summary>
    /// シーン解放
    /// </summary>
    /// <param name="sceneType">解放するシーンタイプ</param>
    /// <param name="onCompleteUnload">解放完了時処理</param>
    public void UnloadScene(SceneName.Type sceneType, Action onCompleteUnload = null)
    {
        // 読み込まれていないシーンであれば処理を終了
        if (!LoadedSceneList.ContainsKey(sceneType))
        {
            OnCompleteUnloadScene(sceneType, onCompleteUnload);
            return;
        }

        // シーン終了処理
        LoadedSceneList[sceneType].SceneFinalize();
        LoadedSceneList[sceneType].gameObject.SetActive(false);

        // 非同期でシーン解放
        var operation = SceneManager.UnloadSceneAsync(SceneName.NameList[sceneType]);
        operation.completed += _ =>
        {
            OnCompleteUnloadScene(sceneType, onCompleteUnload);
        };
    }

    /// <summary>
    /// シーンのアクティブ切り替え
    /// </summary>
    /// <param name="sceneType">対象のシーン</param>
    /// <param name="isActive">true:アクティブ, false:非アクティブ</param>
    public void ActivateScene(SceneName.Type sceneType, bool isActive)
    {
        // 読み込まれていないシーンであれば処理を終了
        if (!LoadedSceneList.ContainsKey(sceneType))
        {
            return;
        }

        // 非アクティブ化時は直前に終了処理
        if (!isActive && LoadedSceneList[sceneType].gameObject.activeSelf)
        {
            LoadedSceneList[sceneType].SceneFinalize();
        }

        LoadedSceneList[sceneType].gameObject.SetActive(isActive);

        // アクティブ化時は直後に初期化処理
        if (isActive && !LoadedSceneList[sceneType].gameObject.activeSelf)
        {
            LoadedSceneList[sceneType].SceneInitialize();
        }
    }

    /// <summary>
    /// すべてのシーンを解放
    /// </summary>
    /// <param name="onCompleteAll"></param>
    public void UnloadAllScene(GameObject gameObject = null, Action onCompleteAll = null)
    {
        var sceneListBuffer = new Dictionary<SceneName.Type, ASceneController>(LoadedSceneList);
        var unloadingCount = new ReactiveProperty<int>(sceneListBuffer.Count);

        // 解放中のシーンを監視し完了時に指定の処理を実行
        var disposable = unloadingCount.ObserveEveryValueChanged(count => count.Value)
            .First(value => value <= 0)
            .Subscribe(_ => onCompleteAll?.Invoke())
            .AddTo(gameObject);

        foreach (var scene in sceneListBuffer)
        {
            // 固定シーンは解放せずスキップ
            if (FixSceneTypeList.Contains(scene.Key)) { continue; }

            // シーン解放
            UnloadScene(scene.Key, () => unloadingCount.Value--);
        }
    }

    /// <summary>
    /// シーンをリストに登録
    /// </summary>
    /// <param name="sceneType">シーンタイプ</param>
    /// <returns>true:追加成功, false:追加失敗</returns>
    public bool RegisterLoadedScene(SceneName.Type sceneType, ASceneController sceneController = null)
    {
        if (LoadedSceneList.ContainsKey(sceneType)) { return false; }

        if (sceneController == null)
        {
            var scene = SceneManager.GetSceneByName(SceneName.NameList[sceneType]);
            if (scene == null)
            {
                Debug.LogError($"[ERROR]Not loaded : {SceneName.NameList[sceneType]}Scene");
                return false;
            }

            var rootObjects = scene.GetRootGameObjects();
            if (rootObjects == null)
            {
                Debug.LogError($"[ERROR]Have no root objects　: {SceneName.NameList[sceneType]}Scene");
                return false;
            }
            sceneController = rootObjects[0].GetComponent<ASceneController>();
            if (sceneController == null)
            {
                Debug.LogError($"[ERROR]Have no scene controller　: {SceneName.NameList[sceneType]}Scene");
                return false;
            }
        }

        LoadedSceneList.Add(sceneType, sceneController);
        return true;
    }

    /// <summary>
    /// シーンがアクティブかチェック
    /// </summary>
    /// <param name="sceneType">対象のシーンタイプ</param>
    /// <returns>true:アクティブ, false:非アクティブ</returns>
    public bool IsActiveScene(SceneName.Type sceneType)
    {
        if (!LoadedSceneList.ContainsKey(sceneType)) { return false; }

        return ActiveSceneList.Contains(sceneType);
    }

    /// <summary>
    /// シーンが読み込み済みかチェック
    /// </summary>
    /// <param name="sceneType">対象のシーンタイプ</param>
    /// <returns>true:読み込み済み, false:未読み込み</returns>
    public bool IsLoadedScene(SceneName.Type sceneType)
    {
        return LoadedSceneList.ContainsKey(sceneType);
    }
}
