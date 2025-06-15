using System;
using R3;
using UnityEngine;

namespace Behaviours
{
	public class UnloadTrigger : MonoBehaviour
	{
		[SerializeField] float _unloadDelay = 3f;
		public event Action OnUnload;
		
		private float _timerValue;
		private IDisposable _currentTimer;

		private bool _isPlayerIn;
		
		public void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				_isPlayerIn = true;
				return;
			}
			
			if (!other.CompareTag("Cargo") || _currentTimer != null) return;

			_currentTimer = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
			{
				if (_isPlayerIn)
				{
					Debug.Log("Forks need to be out of trigger.");
					return;
				}
				
				_timerValue++;
				Debug.Log($"Unload timer: {_timerValue}");
				
				if (!(_timerValue >= _unloadDelay)) return;
				
				OnUnload?.Invoke();
				_currentTimer?.Dispose();
				_currentTimer = null;
			});
		}

		public void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				_isPlayerIn = false;
				return;
			}

			if (!other.CompareTag("Cargo") || _currentTimer == null) return;
			_currentTimer.Dispose();
			_currentTimer = null;
		}
	}
}