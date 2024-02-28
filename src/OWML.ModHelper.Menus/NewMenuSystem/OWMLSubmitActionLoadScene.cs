using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus.NewMenuSystem
{
	internal class OWMLSubmitActionLoadScene : SubmitActionConfirm
	{
		public Text _loadingText;

		private SubmitActionLoadScene.LoadableScenes _sceneToLoad;
		private StringBuilder _nowLoadingSB;
		private bool _receivedSubmitAction;
		private bool _waitingOnStreaming;

		public void SetSceneToLoad(SubmitActionLoadScene.LoadableScenes scene)
			=> _sceneToLoad = scene;

		private void Update()
		{
			if (!_receivedSubmitAction 
			    || (LoadManager.GetLoadingScene() != OWScene.SolarSystem && LoadManager.GetLoadingScene() != OWScene.EyeOfTheUniverse) 
			    || _loadingText == null)
			{
				return;
			}

			var loadProgress = LoadManager.GetAsyncLoadProgress();
			loadProgress = loadProgress < 0.1f
				? Mathf.InverseLerp(0f, 0.1f, loadProgress) * 0.9f
				: 0.9f + (Mathf.InverseLerp(0.1f, 1f, loadProgress) * 0.1f);

			ResetStringBuilder();
			_nowLoadingSB.Append(UITextLibrary.GetString(UITextType.LoadingMessage));
			_nowLoadingSB.Append(loadProgress.ToString("P0"));
			_loadingText.text = _nowLoadingSB.ToString();

			if (_waitingOnStreaming && LoadManager.IsAsyncLoadComplete())
			{
				LoadManager.EnableAsyncLoadTransition();
				_waitingOnStreaming = false;
			}
		}

		private void ResetStringBuilder()
		{
			if (_nowLoadingSB == null)
			{
				_nowLoadingSB = new StringBuilder();
			}
			else
			{
				_nowLoadingSB.Length = 0;
			}
		}

		public override void ConfirmSubmit()
		{
			base.ConfirmSubmit();
			switch (_sceneToLoad)
			{
				case SubmitActionLoadScene.LoadableScenes.GAME:
					LoadManager.LoadSceneAsync(OWScene.SolarSystem, false, LoadManager.FadeType.ToBlack, 1f, false);
					ResetStringBuilder();
					_waitingOnStreaming = true;
					break;
				case SubmitActionLoadScene.LoadableScenes.EYE:
					LoadManager.LoadSceneAsync(OWScene.EyeOfTheUniverse, true, LoadManager.FadeType.ToBlack, 1f, false);
					ResetStringBuilder();
					break;
				case SubmitActionLoadScene.LoadableScenes.TITLE:
					LoadManager.LoadScene(OWScene.TitleScreen, LoadManager.FadeType.ToBlack, 2f, true);
					break;
				case SubmitActionLoadScene.LoadableScenes.CREDITS:
					LoadManager.LoadScene(OWScene.Credits_Fast, LoadManager.FadeType.ToBlack, 1f, false);
					break;
			}

			_receivedSubmitAction = true;
			Locator.GetMenuInputModule().DisableInputs();
		}

		public override void SetUpPopupMenu()
		{
			_receivedSubmitAction = true;
			base.SetUpPopupMenu();
		}

		public override void CleanupPopup()
		{
			if (_listenersAttached)
			{
				_listenersAttached = false;
				_confirmPopup.OnPopupCancel -= CancelSubmit;
				_confirmPopup.OnPopupConfirm -= ConfirmSubmit;
				_confirmPopup.OnForceClosed -= OnPopupForceClosed;
			}
		}
	}
}
