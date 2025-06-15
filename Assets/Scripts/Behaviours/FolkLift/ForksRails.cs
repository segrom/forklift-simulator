using System;
using UnityEngine;

namespace Behaviours.FolkLift
{
	public class ForksRails: MonoBehaviour
	{
		[SerializeField] Transform _forks;
		[SerializeField] Transform _rails;
		[SerializeField] float _velocity;
		[Header("Bounds")]
		[SerializeField] float _forksMinLifting;
		[SerializeField] float _forksMaxLifting;
		[SerializeField] float _railsMinLifting;
		[SerializeField] float _railsMaxLifting;
		
		private float _currentLifting;
		
		public void SetLifting(float lifting)
		{
			_currentLifting = Mathf.Clamp(lifting, -1, 1);
		}

		private void FixedUpdate()
		{
			if (_currentLifting == 0) return;
			if (_currentLifting > 0)
			{
				if (_forks.localPosition.y < _forksMaxLifting)
				{
					_forks.localPosition += Vector3.up * (_currentLifting * _velocity * Time.fixedDeltaTime);
					return;
				}
				if (_rails.localPosition.y < _railsMaxLifting)
				{
					_rails.localPosition += Vector3.up * (_currentLifting * _velocity * Time.fixedDeltaTime);
					return;
				}
				return;
			}
			
			if (_rails.localPosition.y > _railsMinLifting)
			{
				_rails.localPosition += Vector3.up * (_currentLifting * _velocity * Time.fixedDeltaTime);
				return;
			}
			if (_forks.localPosition.y > _forksMinLifting)
			{
				_forks.localPosition += Vector3.up * (_currentLifting * _velocity * Time.fixedDeltaTime);
			}
		}
	}
}