using Abstractions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Behaviours.UI
{
	public class MainScreen : MonoBehaviour, ISplashScreen
	{
		private const float SPLASH_SCREEN_ANIMATION_DURATION = 5f;
		
		[SerializeField] private Image _splashScreenImage;
		
		public async UniTask Show(bool immediately = false)
		{
			await _splashScreenImage
				.DOFade(1f, immediately 
					? 0 
					: SPLASH_SCREEN_ANIMATION_DURATION)
				.ToUniTask();
		}

		public async UniTask Hide(bool immediately = false)
		{
			await _splashScreenImage
				.DOFade(0f, immediately 
					? 0 
					: SPLASH_SCREEN_ANIMATION_DURATION)
				.ToUniTask();
		}
	}
}