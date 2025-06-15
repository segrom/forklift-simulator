using Enums;
using R3;

namespace Abstractions
{
	public interface IForkLift
	{
		ReadOnlyReactiveProperty<bool> IsEngineOn { get; }
		ReadOnlyReactiveProperty<TransmissionGearType> TransmissionGear { get; }
		ReadOnlyReactiveProperty<float> Fuel { get; }
		float MaxFuel { get; }
	}
}