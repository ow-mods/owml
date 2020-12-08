using System.Collections.Generic;
using OWML.Common.Interfaces;
using UnityEngine;

namespace OWML.ModHelper
{
    public class ModBehaviour : MonoBehaviour, IModBehaviour
    {
        public IModHelper ModHelper { get; private set; }

        public object Api { get; private set; }

        public void Init(IModHelper modHelper)
        {
            ModHelper = modHelper;
            Configure(modHelper.Config);
            DontDestroyOnLoad(gameObject);
            Api = GetApi();
        }

        public virtual void Configure(IModConfig config)
        {
        }

        public virtual object GetApi() => null;

        public IList<IModBehaviour> GetDependants()
        {
            return ModHelper.Interaction.GetDependants(ModHelper.Manifest.UniqueName);
        }

        public IList<IModBehaviour> GetDependencies()
        {
            return ModHelper.Interaction.GetDependencies(ModHelper.Manifest.UniqueName);
        }
    }
}
