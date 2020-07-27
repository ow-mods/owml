using System;

namespace OWML.Common
{
    public interface IModUnityEvents
    {
        void FireOnNextUpdate(Action action);
    }
}