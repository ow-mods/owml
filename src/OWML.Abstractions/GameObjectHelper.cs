using System;
using OWML.Common;
using UnityEngine;

namespace OWML.Abstractions
{
	public class GameObjectHelper : IGameObjectHelper
	{
		public TInterface CreateAndAdd<TInterface, TBehaviour>(string name = null) =>
			(TInterface)(object)CreateAndAdd<TBehaviour>(name);

		public TBehaviour CreateAndAdd<TBehaviour>(string name = null) =>
			CreateAndAdd<TBehaviour>(typeof(TBehaviour), name);

		public TBehaviour CreateAndAdd<TBehaviour>(Type type, string name = null) =>
			(TBehaviour)(object)new GameObject(name).AddComponent(type);
	}
}
