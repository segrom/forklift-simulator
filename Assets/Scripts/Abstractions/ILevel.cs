

using UnityEngine;

namespace Abstractions
{
	public interface ILevel
	{
		Vector3 SpawnPoint { get; }
		Vector3[] LoadingPoints { get; }
		Vector3[] UnloadingPoints { get; }
	}
}