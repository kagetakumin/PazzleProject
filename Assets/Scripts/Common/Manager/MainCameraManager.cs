using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraManager : SingletonMonoBehaviour<MainCameraManager>
{
    [SerializeField]
    private Camera _mainCamera = null;

    public Camera MainCamera => _mainCamera;
}
