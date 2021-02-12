using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シングルトンMonobehaviour
/// （アプリ起動中利用する共通機能のスーパークラス）
/// </summary>
/// <typeparam name="T">クラス</typeparam>
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance = null;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(T)) as T;

                if (instance == null)
                {
                    Debug.LogErrorFormat("[ERROR] Not found object : {0}", typeof(T));
                }
            }

            return instance;
        }
    }

    /// <summary>
    /// 生成時
    /// </summary>
    private void Awake()
    {
        // 最初のオブジェクトであればインスタンスを登録
        if (instance == null)
        {
            instance = this as T;
        }

        // 2つ目以降のオブジェクトであれば削除
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
}
