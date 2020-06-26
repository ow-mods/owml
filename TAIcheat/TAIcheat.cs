using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using OWML.ModHelper.Input;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Collections.Generic;
using Harmony;

namespace TAIcheat
{
    public static class MyExtensions
    {
        public static void TAIcheat_SetTranslationalThrust(this JetpackThrusterModel jet, float newacc)
        {
            float oldtrst = jet.GetMaxTranslationalThrust();
            float oldbst = jet.GetBoostMaxThrust();
            FieldInfo fiboost = typeof(JetpackThrusterModel).GetField("_boostThrust", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo fithrust = typeof(JetpackThrusterModel).GetField("_maxTranslationalThrust", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fiboost != null && fithrust != null)
            {
                fiboost.SetValue(jet, (object)(oldbst * (newacc / oldtrst)));
                fithrust.SetValue(jet, (object)(newacc));
            }
        }
    }
    public class DebugInput : ModBehaviour
    {
        private RelativeLocationData relconstr(Vector3 body_position, Quaternion body_rotation, Vector3 body_velocity, OWRigidbody relativeBody, Transform relativeTransform = null)
        {
            if (relativeTransform == null)
            {
                relativeTransform = relativeBody.transform;
            }
            RelativeLocationData res = new RelativeLocationData(Locator.GetPlayerBody(), relativeBody);
            res.localPosition = relativeTransform.InverseTransformPoint(body_position);
            res.localRotation = Quaternion.Inverse(relativeTransform.rotation) * body_rotation;
            res.localRelativeVelocity = relativeTransform.InverseTransformDirection(body_velocity - relativeBody.GetPointVelocity(body_position));
            return res;
        }

        public static IModConsole console;
        public static IModLogger logger;

        private void Start()
        {
            ModHelper.Console.WriteLine("TAICheat ready!");
        }

        public override void Configure(IModConfig config)
        {
            console = ModHelper.Console;
            logger = ModHelper.Logger;
            if (inputs != null)
            {
                foreach (string key in inputs.Keys)
                {
                    ModHelper.Input.UnregisterCombination(inputs[key]);
                }
            }
            inputs = new Dictionary<string, IModInputCombination>();
            foreach (string name in config.Settings.Keys)
            {
                if (config.GetSettingsValue<string>(name) != null)
                {
                    var combination = ModHelper.Input.RegisterCombination(this, name, config.GetSettingsValue<string>(name));
                    inputs.Add(name, combination);
                }
            }
        }

        Dictionary<string, IModInputCombination> inputs;

        private void LateUpdate()
        {
            if (_playerController != null)
                this._gForce = this._playerController.GetNormalAccelerationScalar();
        }

        private void OnGUI()
        {
            if (_playerController == null || _playerForceDetector == null)
            {
                this._playerForceDetector = Locator.GetPlayerForceDetector();
                this._playerController = Locator.GetPlayerController();
                if (_playerController == null || _playerForceDetector == null) return;
            }
            float num = 400f;
            if (GUIMode.IsHiddenMode() || PlayerState.UsingShipComputer())
            {
                return;
            }
            if (inputHUD == 1)
            {
                GUI.Label(new Rect(10f + num, 10f, 200f, 20f), "Time Scale: " + Mathf.Round(Time.timeScale * 100f) / 100f);
                GUI.Label(new Rect(10f + num, 25f, 200f, 20f), string.Concat(new object[]
                {
                "Time Remaining: ",
                Mathf.Floor(TimeLoop.GetSecondsRemaining() / 60f),
                ":",
                Mathf.Round(TimeLoop.GetSecondsRemaining() % 60f * 100f / 100f)
                }));
                GUI.Label(new Rect(10f + num, 40f, 200f, 20f), "Loop Count: " + TimeLoop.GetLoopCount());
                GUI.Label(new Rect(10f + num, 55f, 90f, 40f), "PauseFlags: ");
                GUI.Label(new Rect(100f + num, 55f, 50f, 40f), "MENU\n" + ((!OWTime.IsPaused(OWTime.PauseType.Menu)) ? "FALSE" : "TRUE "));
                GUI.Label(new Rect(150f + num, 55f, 50f, 40f), "LOAD\n" + ((!OWTime.IsPaused(OWTime.PauseType.Loading)) ? "FALSE" : "TRUE "));
                GUI.Label(new Rect(200f + num, 55f, 50f, 40f), "READ\n" + ((!OWTime.IsPaused(OWTime.PauseType.Reading)) ? "FALSE" : "TRUE "));
                GUI.Label(new Rect(250f + num, 55f, 50f, 40f), "SLP\n" + ((!OWTime.IsPaused(OWTime.PauseType.Sleeping)) ? "FALSE" : "TRUE "));
                GUI.Label(new Rect(300f + num, 55f, 50f, 40f), "INIT\n" + ((!OWTime.IsPaused(OWTime.PauseType.Initializing)) ? "FALSE" : "TRUE "));
                GUI.Label(new Rect(350f + num, 55f, 50f, 40f), "STRM\n" + ((!OWTime.IsPaused(OWTime.PauseType.Streaming)) ? "FALSE" : "TRUE "));
                GUI.Label(new Rect(400f + num, 55f, 50f, 40f), "SYS\n" + ((!OWTime.IsPaused(OWTime.PauseType.System)) ? "FALSE" : "TRUE "));
                GUI.Label(new Rect(10f + num, 85f, 200f, 20f), "Input Mode: " + OWInput.GetInputMode().ToString());
                this._inputModeArray = OWInput.GetInputModeStack();
                GUI.Label(new Rect(10f + num, 100f, 200f, 20f), "Input Mode Stack: ");
                int num2 = 150;
                int num3 = 0;
                while (num3 < this._inputModeArray.Length && this._inputModeArray[num3] != InputMode.None)
                {
                    GUI.Label(new Rect((float)num2 + num, 100f, 200f, 20f), this._inputModeArray[num3].ToString());
                    num2 += 75;
                    num3++;
                }
                GUI.Label(new Rect(10f + num, 115f, 300f, 20f), "Net Force Accel: " + Mathf.Round(this._playerForceDetector.GetForceAcceleration().magnitude * 100f) / 100f);
                GUI.Label(new Rect(210f + num, 115f, 300f, 20f), "G-Force: " + Mathf.Round(this._gForce * 100f) / 100f);
                GUI.Label(new Rect(10f + num, 130f, 200f, 20f), "Load Time: " + LoadTimeTracker.GetLatestLoadTime());
                if (DynamicResolutionManager.isEnabled)
                {
                    GUI.Label(new Rect(10f + num, 145f, 200f, 20f), "Resolution Scale: " + DynamicResolutionManager.currentResolutionScale);
                }
                GUI.Label(new Rect(10f + num, 160f, 200f, 20f), "Player Speed: " + (Locator.GetCenterOfTheUniverse().GetOffsetVelocity() + Locator.GetPlayerBody().GetVelocity()).magnitude.ToString());
                GUI.Label(new Rect(210f + num, 160f, 200f, 20f), "Player Accel: " + Locator.GetPlayerBody().GetAcceleration().magnitude.ToString());
                if (Locator.GetPlayerSuit().GetComponent<JetpackThrusterModel>())
                {
                    GUI.Label(new Rect(10f + num, 175f, 200f, 20f), string.Concat(new object[]
                    {
                    "Jetpack Max Accel: ",
                    Locator.GetPlayerSuit().GetComponent<JetpackThrusterModel>().GetMaxTranslationalThrust().ToString(),
                    "/",
                    Locator.GetPlayerSuit().GetComponent<JetpackThrusterModel>().GetBoostMaxThrust().ToString()
                    }));
                }
                if (Locator.GetShipBody().GetComponent<ShipThrusterModel>())
                {
                    GUI.Label(new Rect(210f + num, 175f, 200f, 20f), "Ship Max Accel: " + Locator.GetShipBody().GetComponent<ShipThrusterModel>().GetMaxTranslationalThrust().ToString());
                }
                GUI.Label(new Rect(10f + num, 190f, 400f, 20f), string.Concat(new object[]
                {
                "Inspector layer: ",
                rayMask,
                " ",
                LayerMask.LayerToName(rayMask)
                }));
                if (GetWarpOWRigidbody())
                {
                    GUI.Label(new Rect(10f + num, 205f, 400f, 20f), string.Concat(new string[]
                    {
                    "Warp Body: ",
                    GetWarpOWRigidbody().gameObject.name,
                    " layer: ",
                    GetWarpOWRigidbody().gameObject.layer.ToString(),
                    " ",
                    LayerMask.LayerToName(GetWarpOWRigidbody().gameObject.layer)
                    }));
                }
                if (hit.collider)
                {
                    GUI.Label(new Rect(10f + num, 220f, 400f, 20f), string.Concat(new string[]
                    {
                    "Latest hit layer: ",
                    hit.collider.gameObject.layer.ToString(),
                    " ",
                    LayerMask.LayerToName(hit.collider.gameObject.layer)
                    }));
                    GUI.Label(new Rect(10f + num, 235f, 600f, 20f), "Name: " + hit.collider.gameObject.name + " Distance: " + (hit.point - Locator.GetPlayerBody().transform.position).magnitude.ToString());
                }
                /*if (PadEZ.PadManager.GetActiveController()!=null)
				{
					GUI.Label(new Rect(10f + num, 250f, 600f, 20f), PadEZ.PadManager.GetActiveController().GetIndex().ToString() + " " + PadEZ.PadManager.GetActiveController().GetPadType().ToString() +" "+ UnityEngine.Input.GetJoystickNames()[PadEZ.PadManager.GetActiveController().GetIndex()]);
				}*/
            }
            if (inputHUD == 2)
            {
                GUI.Label(new Rect(10f, 10f, 300f, 2500f), ReadInputManager.ReadCommandInputs(false));
            }
            if (inputHUD == 3)
            {
                GUI.Label(new Rect(0f, 0f, 300f, 2500f), ReadInputManager.ReadCommandInputs(false));
                GUI.Label(new Rect(300f, 0f, 300f, 2500f), ReadInputManager.ReadCommandInputs(true));
            }
            if (inputHUD == 4)
            {
                GUI.Label(new Rect(0f, 0f, 500f, 2500f), ReadInputManager.ReadInputAxes());
                GUI.Label(new Rect(500f, 0f, 500f, 2500f), ReadInputManager.ReadRawInputManagerButtons());
            }
        }

        private ForceDetector _playerForceDetector;
        private PlayerCharacterController _playerController;
        private InputMode[] _inputModeArray;
        private MeshRenderer[] _thrusterArrowRenderers;
        private float _gForce;
        private PlayerSpacesuit playersuit;

        private void FixedUpdate()
        {
            if (this._gotoWarpPointNextFrame)
            {
                this._gotoWarpPointNextFrame = false;
                Locator.GetPlayerBody().MoveToRelativeLocation(_relativeData[relIndex], _relativeBody[relIndex], null);
            }
            if (this._engageLudicrousSpeed)
            {
                this._engageLudicrousSpeed = false;
                Locator.GetShipBody().AddVelocityChange(Locator.GetShipBody().transform.forward * this.ludicrousMult * 25000f);
            }
        }

        private void Update()
        {
            this.shiftPressed = global::Input.GetKey(KeyCode.LeftShift) || global::Input.GetKey(KeyCode.RightShift);
            this.ctrlPressed = global::Input.GetKey(KeyCode.LeftControl) || global::Input.GetKey(KeyCode.RightControl);
            this.altPressed = global::Input.GetKey(KeyCode.LeftAlt) || global::Input.GetKey(KeyCode.RightAlt);
            if (global::Input.GetKeyDown(KeyCode.BackQuote))
            {
                cheatsOn = !cheatsOn;
                if (cheatsOn)
                    AudioSource.PlayClipAtPoint(Locator.GetAudioManager().GetAudioClipArray(global::AudioType.NomaiPowerOn)[0], Locator.GetActiveCamera().transform.position);
                else
                    AudioSource.PlayClipAtPoint(Locator.GetAudioManager().GetAudioClipArray(global::AudioType.NomaiPowerOff)[0], Locator.GetActiveCamera().transform.position);
            }

            if (playersuit == null)
                playersuit = Locator.GetPlayerSuit();
            if (playersuit == null)
                return;
            if (!playersuit.enabled)
                playersuit = Locator.GetPlayerSuit();
            if (playersuit == null || !playersuit.enabled)
                return;
            if (cheatsOn)
            {
                /*if (global::Input.GetKeyDown(DebugKeyCode.destroyTimeline))
				{
					Debug.Log("Try DestroyTimeline (Requires NomaiExperimentBlackHole)");
					GlobalMessenger.FireEvent("DebugTimelineDestroyed");
				}
				if (global::Input.GetKeyDown(DebugKeyCode.uiTestAndSuicide))
				{
					Locator.GetPlayerTransform().GetComponent<PlayerResources>().SetDebugKillResources(true);
				}
				if (global::Input.GetKeyUp(DebugKeyCode.uiTestAndSuicide))
				{
					Locator.GetPlayerTransform().GetComponent<PlayerResources>().SetDebugKillResources(false);
				}*/
                if (ModHelper.Input.IsNewlyPressed(inputs["(tele) Save probe's pos"]))
                {
                    if (Locator.GetProbe().GetAnchor().IsAnchored())
                    {
                        Transform transform = Locator.GetProbe().transform;
                        _relativeBody[relIndex] = transform.parent.GetAttachedOWRigidbody(false);
                        _relativeData[relIndex] = new RelativeLocationData(Locator.GetProbe().GetAnchor().GetAttachedOWRigidbody(), _relativeBody[relIndex], null);
                        this.COn = true;
                    }
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["(tele) Save RayCast pos"]))
                {
                    OWCamera activeCamera = Locator.GetActiveCamera();
                    Vector3 position = new Vector3((float)(activeCamera.pixelWidth - 1) / 2f, (float)(activeCamera.pixelHeight - 1) / 2f);
                    if (!Physics.Raycast(activeCamera.ScreenPointToRay(position), out hit, float.PositiveInfinity, OWLayerMask.BuildPhysicalMask().value))
                    {
                        foreach (RaycastHit raycastHit in Physics.RaycastAll(activeCamera.ScreenPointToRay(position), float.PositiveInfinity, OWLayerMask.BuildPhysicalMask().value | 524288))
                        {
                            hit = raycastHit;
                            if (raycastHit.collider.GetAttachedOWRigidbody(false))
                            {
                                break;
                            }
                        }
                        if (!hit.collider.GetAttachedOWRigidbody(false))
                        {
                            foreach (RaycastHit raycastHit2 in Physics.RaycastAll(activeCamera.ScreenPointToRay(position)))
                            {
                                hit = raycastHit2;
                                if (raycastHit2.collider.GetAttachedOWRigidbody(false))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    if (hit.collider.GetAttachedOWRigidbody(false))
                    {
                        _hasSetWarpPoint[relIndex] = true;
                        _relativeBody[relIndex] = hit.rigidbody.GetAttachedOWRigidbody(false);
                        _relativeData[relIndex] = relconstr(hit.point, Quaternion.FromToRotation(Locator.GetPlayerBody().transform.up, hit.normal) * Locator.GetPlayerBody().transform.rotation, _relativeBody[relIndex].GetPointVelocity(hit.point), _relativeBody[relIndex], null);
                        this.COn = true;
                    }
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["(tele) Save player's pos"]) && Locator.GetPlayerSectorDetector().GetLastEnteredSector() != null)
                {
                    _hasSetWarpPoint[relIndex] = true;
                    _relativeBody[relIndex] = Locator.GetPlayerSectorDetector().GetLastEnteredSector().GetOWRigidbody();
                    _relativeData[relIndex] = new RelativeLocationData(Locator.GetPlayerBody(), _relativeBody[relIndex], null);
                    this.COn = true;
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["(insp) Cycle through layers"]))
                {
                    rayMask++;
                    rayMask %= 32;
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["(insp) RayCast"]))
                {
                    OWCamera activeCamera2 = Locator.GetActiveCamera();
                    Vector3 position2 = new Vector3((float)(activeCamera2.pixelWidth - 1) / 2f, (float)(activeCamera2.pixelHeight - 1) / 2f);
                    if (!Physics.Raycast(activeCamera2.ScreenPointToRay(position2), out hit, float.PositiveInfinity, 1 << rayMask) && hit.collider.GetAttachedOWRigidbody(false))
                    {
                        _hasSetWarpPoint[relIndex] = true;
                        _relativeBody[relIndex] = hit.rigidbody.GetAttachedOWRigidbody(false);
                        _relativeData[relIndex] = relconstr(hit.point, Quaternion.FromToRotation(Locator.GetPlayerBody().transform.up, hit.normal) * Locator.GetPlayerBody().transform.rotation, _relativeBody[relIndex].GetPointVelocity(hit.point), _relativeBody[relIndex], null);
                        this.COn = true;
                    }
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["(tele) Cycle through pos"]))
                {
                    this.COn = true;
                    relIndex++;
                    relIndex %= 10;
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["(tele) Tele to saved pos"]) && _hasSetWarpPoint[relIndex])
                {
                    this.COn = true;
                    this._gotoWarpPointNextFrame = true;
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["Learn launchcode"]))
                {
                    if (PlayerData.IsLoaded())
                    {
                        PlayerData.LearnLaunchCodes();
                        this.COn = true;
                    }
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["Toggle spacesuit"]))
                {
                    if (!Locator.GetPlayerSuit().IsWearingSuit(true))
                    {
                        Locator.GetPlayerSuit().SuitUp(false, false);
                        this.COn = true;
                    }
                    else
                    {
                        Locator.GetPlayerSuit().RemoveSuit(false);
                        this.COff = true;
                    }
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["Toggle HUD"]))
                {
                    hiddenHUD = !hiddenHUD;
                    if (hiddenHUD)
                    {
                        oldmode = (int)typeof(GUIMode).GetAnyField("_renderMode").GetValue(null);
                        typeof(GUIMode).GetAnyField("_renderMode").SetValue(null, 7);
                        this.COn = true;
                    }
                    else
                    {
                        typeof(GUIMode).GetAnyField("_renderMode").SetValue(null, oldmode);
                        this.COff = true;
                    }
                    GlobalMessenger.FireEvent("OnChangeGUIMode");
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["Toggle helmet"]) && Locator.GetPlayerSuit() && Locator.GetPlayerSuit().IsWearingSuit(true))
                {
                    if (Locator.GetPlayerSuit().IsWearingHelmet())
                    {
                        Locator.GetPlayerSuit().RemoveHelmet();
                        this.COff = true;
                    }
                    else
                    {
                        Locator.GetPlayerSuit().PutOnHelmet();
                        this.COn = true;
                    }
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["Destroy ship"]) && Locator.GetShipTransform())
                {
                    UnityEngine.Object.Destroy(Locator.GetShipTransform().gameObject);
                    this.COn = true;
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["Damage ship"]) && Locator.GetShipTransform())
                {
                    ShipComponent[] componentsInChildren = Locator.GetShipTransform().GetComponentsInChildren<ShipComponent>();
                    for (int k = 0; k < componentsInChildren.Length; k++)
                    {
                        componentsInChildren[k].SetDamaged(true);
                    }
                    this.COn = true;
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["Fuel+heal"]))
                {
                    Locator.GetPlayerTransform().GetComponent<PlayerResources>().DebugRefillResources();
                    if (Locator.GetShipTransform())
                    {
                        ShipComponent[] componentsInChildren2 = Locator.GetShipTransform().GetComponentsInChildren<ShipComponent>();
                        for (int l = 0; l < componentsInChildren2.Length; l++)
                        {
                            componentsInChildren2[l].SetDamaged(false);
                        }
                    }
                    this.COn = true;
                }
                if (ModHelper.Input.IsPressed(inputs["Increase Ludicrous Speed"]))
                {
                    this.ludicrousMult *= 2f;
                    this.COn = true;
                }
                if (ModHelper.Input.IsPressed(inputs["Decrease Ludicrous Speed"]))
                {
                    this.ludicrousMult /= 2f;
                    this.COff = true;
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["Engage Ludicrous Speed"]))
                {
                    this._engageLudicrousSpeed = true;
                    AudioSource.PlayClipAtPoint(Locator.GetAudioManager().GetAudioClipArray(global::AudioType.ToolProbeLaunch)[0], Locator.GetPlayerBody().transform.position);
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["Toggle superjetpack"]) && Locator.GetPlayerSuit().GetComponent<JetpackThrusterModel>())
                {
                    if (!this.wasBoosted)
                    {
                        this.jetpackStanard = Locator.GetPlayerSuit().GetComponent<JetpackThrusterModel>().GetMaxTranslationalThrust();
                        Locator.GetPlayerSuit().GetComponent<JetpackThrusterModel>().TAIcheat_SetTranslationalThrust(50f);
                    }
                    else
                    {
                        Locator.GetPlayerSuit().GetComponent<JetpackThrusterModel>().TAIcheat_SetTranslationalThrust(this.jetpackStanard);
                    }
                    this.wasBoosted = !this.wasBoosted;
                    if (this.wasBoosted)
                    {
                        this.COn = true;
                    }
                    else
                    {
                        this.COff = true;
                    }
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["Toggle PowerOverwhelming"]))
                {
                    Locator.GetPlayerTransform().GetComponent<PlayerResources>().ToggleInvincibility();
                    Locator.GetDeathManager().ToggleInvincibility();
                    Transform shipTransform = Locator.GetShipTransform();
                    if (shipTransform)
                    {
                        shipTransform.GetComponentInChildren<ShipDamageController>().ToggleInvincibility();
                        invincible = !invincible;
                    }
                    if (invincible)
                    {
                        this.COn = true;
                    }
                    else
                    {
                        this.COff = true;
                    }
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["Learn all frequencies"]))
                {
                    if (PlayerData.KnowsMultipleFrequencies())
                    {
                        PlayerData.ForgetFrequency(SignalFrequency.Quantum);
                        PlayerData.ForgetFrequency(SignalFrequency.EscapePod);
                        PlayerData.ForgetFrequency(SignalFrequency.Statue);
                        PlayerData.ForgetFrequency(SignalFrequency.WarpCore);
                        PlayerData.ForgetFrequency(SignalFrequency.HideAndSeek);
                        this.COff = true;
                    }
                    else
                    {
                        this.COn = true;
                        for (int m = 10; m < 16; m++)
                        {
                            PlayerData.LearnSignal((SignalName)m);
                        }
                        PlayerData.LearnFrequency(SignalFrequency.Quantum);
                        for (int n = 20; n < 26; n++)
                        {
                            PlayerData.LearnSignal((SignalName)n);
                        }
                        PlayerData.LearnFrequency(SignalFrequency.EscapePod);
                        for (int num = 30; num < 33; num++)
                        {
                            PlayerData.LearnSignal((SignalName)num);
                        }
                        PlayerData.LearnFrequency(SignalFrequency.Statue);
                        PlayerData.LearnFrequency(SignalFrequency.WarpCore);
                        for (int num2 = 40; num2 < 50; num2++)
                        {
                            PlayerData.LearnSignal((SignalName)num2);
                        }
                        PlayerData.LearnFrequency(SignalFrequency.HideAndSeek);
                        for (int num3 = 60; num3 < 63; num3++)
                        {
                            PlayerData.LearnSignal((SignalName)num3);
                        }
                    }
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["Reveal all facts"]))
                {
                    Locator.GetShipLogManager().RevealAllFacts(this._revealRumorsOnly);
                    this._revealRumorsOnly = false;
                    this.COn = true;
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["Cycle DebugHUD"]))
                {
                    inputHUD++;
                    inputHUD %= 5;
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["Toggle player collision extra"]))
                {
                    if (Locator.GetPlayerBody().GetRequiredComponent<Rigidbody>().detectCollisions)
                    {
                        Locator.GetPlayerBody().DisableCollisionDetection();
                        this.COn = true;
                    }
                    else
                    {
                        Locator.GetPlayerBody().EnableCollisionDetection();
                        this.COff = true;
                    }
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["Toggle player collision"]))
                {
                    foreach (Collider collider in Locator.GetPlayerBody().GetComponentsInChildren<Collider>())
                    {
                        if (!collider.isTrigger)
                        {
                            collider.enabled = !collider.enabled;
                            if (collider.enabled)
                            {
                                this.COff = true;
                            }
                            else
                            {
                                this.COn = true;
                            }
                        }
                    }
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["Toggle ship collision extra"]))
                {
                    if (Locator.GetShipBody().GetRequiredComponent<Rigidbody>().detectCollisions)
                    {
                        Locator.GetShipBody().DisableCollisionDetection();
                        this.COn = true;
                    }
                    else
                    {
                        Locator.GetShipBody().EnableCollisionDetection();
                        this.COff = true;
                    }
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["Toggle ship collision"]))
                {
                    foreach (Collider collider2 in Locator.GetShipTransform().GetComponentsInChildren<Collider>())
                    {
                        if (!collider2.isTrigger)
                        {
                            collider2.enabled = !collider2.enabled;
                            if (collider2.enabled)
                            {
                                this.COff = true;
                            }
                            else
                            {
                                this.COn = true;
                            }
                        }
                    }
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["Disable nearby Anglerfishes"]))
                {
                    foreach (AnglerfishController anglerfishController in UnityEngine.Object.FindObjectsOfType<AnglerfishController>())
                    {
                        anglerfishController.gameObject.SetActive(!anglerfishController.gameObject.activeInHierarchy);
                        if (anglerfishController.gameObject.activeInHierarchy)
                        {
                            this.COff = true;
                        }
                        else
                        {
                            this.COn = true;
                        }
                    }
                }
                if (ModHelper.Input.IsNewlyPressed(inputs["Toggle nearby Anglerfishes AI"]))
                {

                    foreach (AnglerfishController anglerfishController in UnityEngine.Object.FindObjectsOfType<AnglerfishController>())
                    {
                        anglerfishController.enabled = !anglerfishController.enabled;
                        if (anglerfishController.enabled)
                        {
                            this.COff = true;
                        }
                        else
                        {
                            this.COn = true;
                        }
                    }
                }
            }
            if (ModHelper.Input.IsNewlyPressed(inputs["Break all BH fragments"]))
            {
                this.COn = true;
                FragmentIntegrity[] array2 = UnityEngine.Object.FindObjectsOfType<FragmentIntegrity>();
                for (int j = 0; j < array2.Length; j++)
                {
                    array2[j].AddDamage(10000f);
                }
            }
            if (ModHelper.Input.IsNewlyPressed(inputs["Trigger Supernova"]))
            {
                this.COn = true;
                GlobalMessenger.FireEvent("TriggerSupernova");
            }
            if (ModHelper.Input.IsNewlyPressed(inputs["Debug Vessel Warp"]))
            {
                if (PlayerData.GetWarpedToTheEye()) PlayerData.SaveEyeCompletion();
                else GlobalMessenger.FireEvent("DebugWarpVessel");
                this.COn = true;
            }
            if (Mathf.Abs(Time.timeScale - 1f) < 1e-4 || Mathf.Abs(Time.timeScale - 10f) < 1e-4)
            {
                if (ModHelper.Input.IsNewlyPressed(inputs["Timelapse"]))
                {
                    Time.timeScale = 10f;
                }
                else if (Mathf.Abs(Time.timeScale - 1f) > 1e-4 && !ModHelper.Input.IsPressed(inputs["Timelapse"]))
                {
                    Time.timeScale = 1f;
                }
            }

            if (this.COn)
            {
                AudioClip[] audioClipArray = Locator.GetAudioManager().GetAudioClipArray(global::AudioType.Menu_Confirm);
                AudioSource.PlayClipAtPoint(audioClipArray[UnityEngine.Random.Range(0, audioClipArray.Length)], Locator.GetActiveCamera().transform.position);
                this.COn = false;
            }
            if (this.COff)
            {
                AudioClip[] audioClipArray2 = Locator.GetAudioManager().GetAudioClipArray(global::AudioType.Menu_Cancel);
                AudioSource.PlayClipAtPoint(audioClipArray2[UnityEngine.Random.Range(0, audioClipArray2.Length)], Locator.GetActiveCamera().transform.position);
                this.COff = false;
            }
        }

        public OWRigidbody GetWarpOWRigidbody()
        {
            return _relativeBody[relIndex];
        }

        private bool altPressed, shiftPressed, ctrlPressed;
        private bool cheatsOn;
        private bool COff, COn, CMOn, CMOff;

        private bool _revealRumorsOnly = true;

        private bool _engageLudicrousSpeed;
        private float ludicrousMult = 1f;

        private bool invincible = false;

        private bool wasBoosted;
        private float jetpackStanard = 6f;


        private bool _gotoWarpPointNextFrame;
        private RelativeLocationData[] _relativeData = new RelativeLocationData[10];
        private OWRigidbody[] _relativeBody = new OWRigidbody[10];
        private bool[] _hasSetWarpPoint = new bool[10];
        private int relIndex;
        private RaycastHit hit;
        private int rayMask;

        private int oldmode;
        private bool hiddenHUD;
        private int inputHUD = 0;
    }
}
