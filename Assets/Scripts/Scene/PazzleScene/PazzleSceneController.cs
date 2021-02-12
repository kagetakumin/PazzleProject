using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// パズルシーン制御
/// </summary>
public class PazzleSceneController : ASceneController
{
    private enum GameStatus
    {
        None,
        Initialize,
        GameStart,
        GeneratePiece,
        CheckBoard,
        WaitInput,
        MovePiece,
        GameEnd,
    }

    [SerializeField]
    private Button _newButton = null;

    [SerializeField]
    private Button _menuButton = null;

    [SerializeField]
    private Text _scoreText = null;

    [SerializeField]
    private Text _bestScoreText = null;

    [SerializeField]
    private List<PazzlePiece> _pieceList = null;

    private GameStatus CurrentStatus { get; set; } = GameStatus.None;
    private ReactiveProperty<int> ScoreValue { get; set; } = new ReactiveProperty<int>(0);
    private ReactiveProperty<int> BestScoreValue { get; set; } = new ReactiveProperty<int>(0);

    private void InitializePazzle()
    {
        CurrentStatus = GameStatus.Initialize;
        ScoreValue.Value = 0;
        BestScoreValue.Value = 0;

        foreach(var piece in _pieceList)
        {
            piece.Initialize();
            piece.gameObject.SetActive(false);
        }
    }

    private void ChangeGameStatus()
    {
        switch (CurrentStatus)
        {
            case GameStatus.None:
                break;
            case GameStatus.Initialize:
                break;
            case GameStatus.GameStart:
                break;
            case GameStatus.GeneratePiece:
                break;
            case GameStatus.CheckBoard:
                break;
            case GameStatus.WaitInput:
                break;
            case GameStatus.MovePiece:
                break;
            case GameStatus.GameEnd:
                break;
            default:
                break;
        }
    }

    private void OnClickNewButton()
    {
        InitializePazzle();
    }

    private void OnClickMenuButton()
    {
    }

    private void OnValueChangeScore(int value)
    {
        _scoreText.text = value.ToString("n");
    }

    private void OnValueChangeBestScore(int value)
    {
        _bestScoreText.text = value.ToString("n");
    }

    protected override void OnStartScene()
    {
        base.OnStartScene();

        _newButton?.onClick.RemoveAllListeners();
        _menuButton?.onClick.RemoveAllListeners();

        _newButton?.onClick.AddListener(OnClickNewButton);
        _menuButton?.onClick.AddListener(OnClickMenuButton);

        InitializePazzle();
    }

    public override void SceneInitialize()
    {
        base.SceneInitialize();

        ScoreValue.ObserveEveryValueChanged(score => score.Value).Subscribe(OnValueChangeScore);
        BestScoreValue.ObserveEveryValueChanged(score => score.Value).Subscribe(OnValueChangeBestScore);

        InitializePazzle();
    }
}
