namespace OWML.Common
{
    /// <summary>
    /// The types of events that can be subscribed to.
    /// </summary>
    public enum Events
    {
        /// <summary>Called before the Awake() method.</summary>
        BeforeAwake = 0,

        /// <summary>Called after the Awake() method.</summary>
        AfterAwake = 1,

        /// <summary>Called before the Start() method.</summary>
        BeforeStart = 2,

        /// <summary>Called after the Start() method.</summary>
        AfterStart = 3,

        /// <summary>Called before the OnEnable() method.</summary>
        BeforeEnable = 4,

        /// <summary>Called after the OnEnable() method.</summary>
        AfterEnable = 5,

        /// <summary>Called before the OnDisable() method.</summary>
        BeforeDisable = 6,

        /// <summary>Called after the OnDisable() method.</summary>
        AfterDisable = 7
    }
}
