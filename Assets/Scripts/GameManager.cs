using System;
using Abstractions;
using Behaviours;
using Behaviours.FolkLift;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class GameManager : MonoBehaviour, IGameManager, IInitializable
{
    [Inject] DiContainer _container;
    [Inject] ISplashScreen _splashScreen;
    
    private Level _currentLevel;
    private ForkLiftBase _forkLift;
    
    private MainActions _input;
    
    public void Initialize()
    {
        Debug.Log("Game Manager Started");
        InitializeInternal().Forget(Debug.LogException);
        
        _input = new MainActions();
        _input.UI.Exit.performed += HandleExit;
        Application.focusChanged += HandleFocusChange;
        Cursor.visible = false;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
    }

    private void HandleFocusChange(bool hasFocus)
    {
        Cursor.visible = !hasFocus;
    }

    private async UniTask InitializeInternal()
    {
        var prefab = await Resources.LoadAsync<ForkLiftBase>("Prefabs/ForkLift").ToUniTask();
        if (prefab is not ForkLiftBase forkLiftPrefab) throw new NullReferenceException();
        _forkLift = _container.InstantiatePrefabForComponent<ForkLiftBase>(forkLiftPrefab, parentTransform: null);

        await LoadLevel("Level1");
        await UniTask.Delay(100);
        _splashScreen.Hide();
        _input.Enable();
    }

    private async UniTask LoadLevel(string levelName)
    {
        if (_currentLevel != null) Destroy(_currentLevel);
        
        var obj = await Resources.LoadAsync<Level>("Prefabs/Levels/" + levelName).ToUniTask();
        if (obj is not Level level) throw new Exception("Level not found");
        _currentLevel = _container.InstantiatePrefabForComponent<Level>(level, parentTransform: null);
        
        _forkLift.transform.position = _currentLevel.SpawnPoint;
        
        Resources.UnloadUnusedAssets();

        _currentLevel.BeginGameLoop().Forget(Debug.LogException);
    }

    private void HandleExit(InputAction.CallbackContext _)
    {
        Debug.Log("Game Manager Exit");
        Application.Quit();
    }
    
    private void OnDestroy()
    {
        _input.Disable();
        _input.UI.Exit.performed -= HandleExit;
        Application.focusChanged -= HandleFocusChange;
        Cursor.visible = true;
    }
}
