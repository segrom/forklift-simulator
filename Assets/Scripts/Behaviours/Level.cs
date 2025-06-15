using System;
using System.Linq;
using System.Threading.Tasks;
using Abstractions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Behaviours
{
	public class Level: MonoBehaviour, ILevel
	{
		public Vector3 SpawnPoint => _spawnPoint.position;
		public Vector3[] LoadingPoints => _loadingPoints.Select(x=>x.position).ToArray();
		public Vector3[] UnloadingPoints => _unloadingPoints.Select(x=>x.position).ToArray();
		
		[SerializeField] private Transform _spawnPoint;
		[SerializeField] private Transform[] _loadingPoints;
		[SerializeField] private Transform[] _unloadingPoints;
		
		[Inject] DiContainer _container;

		private GameObject _cargoPrefab;
		private GameObject _triggerPrefab;
		
		private GameObject _currentCargo;
		private UnloadTrigger _currentTrigger;
		
		public async UniTask BeginGameLoop()
		{
			if (_cargoPrefab == null) 
				_cargoPrefab = await Resources.LoadAsync<GameObject>("Prefabs/Cargo").ToUniTask() as GameObject;
			
			if (_triggerPrefab == null) 
				_triggerPrefab = await Resources.LoadAsync<GameObject>("Prefabs/UnloadTrigger").ToUniTask() as GameObject;
			
			await UniTask.Delay(100);

			NewDelivery();
		}

		private void NewDelivery()
		{
			var unloadPoint = _unloadingPoints[Random.Range(0, _loadingPoints.Length)];
			var scale = Random.Range(2f, 8f);
			
			_currentTrigger = _container.InstantiatePrefabForComponent<UnloadTrigger>(_triggerPrefab, 
				unloadPoint.transform.position, Quaternion.identity,null);
			_currentTrigger.transform.localScale = new Vector3(scale, _currentTrigger.transform.localScale.y, scale);
			_currentTrigger.OnUnload += HandleUnload;
			
			var loadPoint = _loadingPoints[Random.Range(0, _loadingPoints.Length)];
			_currentCargo = _container.InstantiatePrefab(_cargoPrefab, loadPoint.transform.position, Quaternion.identity,null);
			CargoShow(loadPoint.transform.position).Forget(Debug.LogException);
		}

		private void HandleUnload()
		{
			HandleUnloadInternal().Forget(Debug.LogException);
		}

		private async UniTask HandleUnloadInternal()
		{
			await UniTask.Delay(100);
			await CargoHide(_currentCargo.transform.position + new Vector3(0, 20f, 0));
			
			var unloadPoint = _unloadingPoints[Random.Range(0, _loadingPoints.Length)];
			var scale = Random.Range(2f, 8f);
			_currentTrigger.transform.position = unloadPoint.transform.position;
			_currentTrigger.transform.localScale = new Vector3(scale, _currentTrigger.transform.localScale.y, scale);
			
			var loadPoint = _loadingPoints[Random.Range(0, _loadingPoints.Length)];
			_currentCargo.transform.position = loadPoint.transform.position;
			
			await CargoShow(loadPoint.transform.position);
		}

		private async UniTask CargoShow(Vector3 targetPosition)
		{
			var rb = _currentCargo.GetComponent<Rigidbody>();
			rb.isKinematic = true;
			_currentCargo.transform.position = targetPosition + new Vector3(0, 20f, 0);
			var seq = DOTween.Sequence();
			
			seq.Append(_currentCargo.transform.DOMove(targetPosition, 5f).SetEase(Ease.OutQuad));
			seq.Join(_currentCargo.transform.DOLocalRotate(new Vector3(0, 900, 0), 5f).SetEase(Ease.OutQuad));
			await seq.Play().ToUniTask();
			rb.isKinematic = false;
		}
		
		private async UniTask CargoHide(Vector3 targetPosition)
		{
			var rb = _currentCargo.GetComponent<Rigidbody>();
			rb.isKinematic = true;
			var seq = DOTween.Sequence();
			
			seq.Append(_currentCargo.transform.DOMove(targetPosition, 5f).SetEase(Ease.OutQuad));
			seq.Join(_currentCargo.transform.DOLocalRotate(new Vector3(0, 900, 0), 5f).SetEase(Ease.OutQuad));
			await seq.Play().ToUniTask();
		}

		private void OnDestroy()
		{
			if (_currentTrigger != null)
			{
				_currentTrigger.OnUnload -= HandleUnload;
				Destroy(_currentTrigger);
			}
			
			if(_currentCargo != null) 
				Destroy(_currentCargo);
		}
	}
}