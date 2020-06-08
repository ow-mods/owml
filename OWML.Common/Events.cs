namespace OWML.Common
{
    /// <summary>
    /// The types of events that can be subscribed to.
    /// </summary>
    public enum Events
    {
        /// <summary>Called before MonoBehaviour.Awake().</summary>
        BeforeAwake = 0,

        /// <summary>Called after MonoBehaviour.Awake().</summary>
        AfterAwake = 1,

        /// <summary>Called before MonoBehaviour.Start().</summary>
        BeforeStart = 2,

        /// <summary>Called after MonoBehaviour.Start().</summary>
        AfterStart = 3,

        /// <summary>Called before MonoBehaviour.OnEnable().</summary>
        BeforeEnable = 4,

        /// <summary>Called after MonoBehaviour.OnEnable().</summary>
        AfterEnable = 5,

        /// <summary>Called before MonoBehaviour.OnDisable().</summary>
        BeforeDisable = 6,

        /// <summary>Called after MonoBehaviour.OnDisable().</summary>
        AfterDisable = 7
    }
}
