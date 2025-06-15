using Cysharp.Threading.Tasks;

namespace Abstractions
{
	public interface ISplashScreen
	{
		public UniTask Show(bool immediately = false);
		public UniTask Hide(bool immediately = false);
	}
}