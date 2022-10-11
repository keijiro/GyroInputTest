using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

sealed class GyroInputTester : MonoBehaviour
{
    #region Static class members

    // Layout extension written in JSON
    const string LayoutJson = @"{
      ""name"": ""DualShock4GamepadHIDCustom"",
      ""extend"": ""DualShock4GamepadHID"",
      ""controls"": [
        {""name"":""gyro"", ""format"":""VC3S"", ""offset"":13,
         ""layout"":""Vector3"", ""processors"":""ScaleVector3(x=-1,y=-1,z=1)""},
        {""name"":""gyro/x"", ""format"":""SHRT"", ""offset"":0 },
        {""name"":""gyro/y"", ""format"":""SHRT"", ""offset"":2 },
        {""name"":""gyro/z"", ""format"":""SHRT"", ""offset"":4 },
        {""name"":""accel"", ""format"":""VC3S"", ""offset"":19,
         ""layout"":""Vector3"", ""processors"":""ScaleVector3(x=-1,y=-1,z=1)""},
        {""name"":""accel/x"", ""format"":""SHRT"", ""offset"":0 },
        {""name"":""accel/y"", ""format"":""SHRT"", ""offset"":2 },
        {""name"":""accel/z"", ""format"":""SHRT"", ""offset"":4 }
      ]}";

    #endregion

    #region Private members

    // Accumulation of gyro input
    Quaternion _accGyro = Quaternion.identity;

    // Accelerometer input control
    Vector3Control _accel;

    // Gyro vector data to rotation conversion
    Quaternion GyroInputToRotation(InputAction.CallbackContext ctx)
      => Quaternion.Euler(ctx.ReadValue<Vector3>() * Mathf.PI * 2);

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        // DS4 input layout extension
        InputSystem.RegisterLayoutOverride(LayoutJson);

        // Gyroscope input callback
        var action = new InputAction(binding: "<Gamepad>/gyro");
        action.performed += ctx => _accGyro *= GyroInputToRotation(ctx);
        action.Enable();

        // Accelerometer input control
        _accel = Gamepad.current.GetChildControl<Vector3Control>("accel");
    }

    void Update()
    {
        // Current status
        var rot = transform.localRotation;

        // Rotation from gyroscope
        rot *= _accGyro;
        _accGyro = Quaternion.identity;

        // Drift compensation using gravitational acceleration
        var gravity = _accel.ReadValue();
        var comp = Quaternion.FromToRotation(rot * gravity, -Vector3.up);

        // Compensation reduction
        comp.w *= 10.0f;
        comp = comp.normalized;

        // Update
        transform.localRotation = comp * rot;
    }

    #endregion
}
