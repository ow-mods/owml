using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.Common
{
	public interface IPauseMenuManager
	{
		public Menu MakePauseListMenu(string title);

		public SubmitAction MakeSimpleButton(string name, Menu customMenu = null);

		public GameObject MakeMenuOpenButton(string name, Menu menuToOpen, Menu customMenu = null);

		public GameObject MakeSceneLoadButton(string name, SubmitActionLoadScene.LoadableScenes sceneToLoad, PopupMenu confirmPopup = null, Menu customMenu = null);
	}
}
