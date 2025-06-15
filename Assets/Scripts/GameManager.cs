using System;
using Abstractions;
using Behaviours;
using Behaviours.FolkLift;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour, IGameManager, IInitializable
{
    [Inject] DiContainer _container;
    [Inject] ISplashScreen _splashScreen;
    
    private Level _currentLevel;
    private ForkLiftBase _forkLift;
    
    public void Initialize()
    {
        Debug.Log("Game Manager Started");
        InitializeInternal().Forget(Debug.LogException);
    }

    private async UniTask InitializeInternal()
    {
        var prefab = await Resources.LoadAsync<ForkLiftBase>("Prefabs/ForkLift").ToUniTask();
        if (prefab is not ForkLiftBase forkLiftPrefab) throw new NullReferenceException();
        _forkLift = _container.InstantiatePrefabForComponent<ForkLiftBase>(forkLiftPrefab, parentTransform: null);

        await LoadLevel("Level1");
        await UniTask.Delay(100);
        _splashScreen.Hide();
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
}
