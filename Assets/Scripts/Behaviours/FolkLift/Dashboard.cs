using TMPro;
using UnityEngine;

namespace Behaviours.FolkLift
{
	public class Dashboard: MonoBehaviour
	{
		[SerializeField] TMP_Text _leftBar;
		[SerializeField] TMP_Text _rightBar;
		[SerializeField] ForkLiftBase _forkLift;
		
		private void Update()
		{
			var fuel = _forkLift.Fuel.CurrentValue;
			var isOn = _forkLift.IsEngineOn.CurrentValue;
			var gear = _forkLift.TransmissionGear.CurrentValue;
			_leftBar.text = $"Fuel {fuel:F2}L\nEngine {(isOn ? "ON" : "OFF")}";
			_rightBar.text = $"Gear {gear}";
		}
	}
}