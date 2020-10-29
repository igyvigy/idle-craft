// using InputSamples.Controls;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadInputManager : MonoBehaviour
{
    private GamepadControls playerControls;
    public Vector2 MovementValue { get; private set; }
    public Vector2 LookValue { get; private set; }
    public Vector2 ThirdPersonLookValue { get; private set; }
    public bool StartButtonValue { get; private set; }
    public bool SelectButtonValue { get; private set; }
    public bool JumpValue { get; private set; }
    public bool AttackRValue { get; private set; }
    public bool KickLValue { get; private set; }
    public bool KickRValue { get; private set; }
    public float CameraUpDownValue { get; private set; }
    public float ZoomValue { get; private set; }
    public float CameraFrontBackValue { get; private set; }
    public bool NextTargetValue { get; private set; }
    public bool PrevTargetValue { get; private set; }
    public bool DestroyValue { get; private set; }
    public bool BuildValue { get; private set; }
    public bool LightAttackValue { get; private set; }
    private delegate void AxisClosure(Vector2 prop);
    private delegate void FloatClosure(float prop);
    private delegate void BoolClosure(bool prop);
    private void BindBool(UnityEngine.InputSystem.InputAction action, BoolClosure prop)
    {
        action.performed += context => prop(context.ReadValue<float>() >= InputSystem.settings.defaultButtonPressPoint);
        action.canceled += context => prop(false);
    }

    private void BindFloat(UnityEngine.InputSystem.InputAction action, FloatClosure prop)
    {
        action.performed += context => prop(context.ReadValue<float>());
        action.canceled += context => prop(0);
    }
    private static void BindAxis(UnityEngine.InputSystem.InputAction action, AxisClosure prop)
    {
        action.performed += context => prop(context.ReadValue<Vector2>());
        action.canceled += context => prop(Vector2.zero);
    }

    protected virtual void Awake()
    {
        playerControls = new GamepadControls();

        BindAxis(playerControls.gameplay.Movement, axis => MovementValue = axis);
        BindAxis(playerControls.gameplay.Look, axis => LookValue = axis);
        BindAxis(playerControls.gameplay.ThirdPersonLook, axis => ThirdPersonLookValue = axis);

        BindFloat(playerControls.gameplay.CameraForwardBack, f => CameraFrontBackValue = f);
        BindFloat(playerControls.gameplay.CameraUpDown, f => CameraUpDownValue = f);

        BindFloat(playerControls.gameplay.Zoom, f => ZoomValue = f);

        BindBool(playerControls.gameplay.Start, b => StartButtonValue = b);
        BindBool(playerControls.gameplay.Select, b => SelectButtonValue = b);

        BindBool(playerControls.gameplay.LightAttack, b => LightAttackValue = b);

        BindBool(playerControls.gameplay.Jump, b => JumpValue = b);
        BindBool(playerControls.gameplay.AttackR, b => AttackRValue = b);
        BindBool(playerControls.gameplay.KickL, b => KickLValue = b);
        BindBool(playerControls.gameplay.KickR, b => KickRValue = b);

        BindBool(playerControls.gameplay.NextTarget, b => NextTargetValue = b);
        BindBool(playerControls.gameplay.PrevTarget, b => PrevTargetValue = b);
        BindBool(playerControls.gameplay.Destroy, b => DestroyValue = b);
        BindBool(playerControls.gameplay.Build, b => BuildValue = b);
    }

    protected virtual void OnEnable()
    {
        playerControls?.Enable();
    }

    protected virtual void OnDisable()
    {
        playerControls?.Disable();
    }
}
