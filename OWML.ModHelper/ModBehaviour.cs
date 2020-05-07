using OWML.Common;
using OWML.ModHelper.Events;
using System.Collections.Generic;
using UnityEngine;

namespace OWML.ModHelper
{
    public class ModBehaviour : MonoBehaviour, IModBehaviour
    {
        public IModHelper ModHelper { get; private set; }

        public void Init(IModHelper modHelper)
        {
            ModHelper = modHelper;
            Configure(modHelper.Config);
            DontDestroyOnLoad(gameObject);
        }

        public virtual void Configure(IModConfig config)
        {
        }

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
