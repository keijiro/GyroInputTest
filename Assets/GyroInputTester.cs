using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

sealed class GyroInputTester : MonoBehaviour
{
    Quaternion _accGyro = Quaternion.identity;

    Quaternion GyroInputToRotation(InputAction.CallbackContext ctx)
      => Quaternion.Euler(ctx.ReadValue<Vector3>() * Mathf.PI * 2);

    void Start()
    {
        // DS4 input layout extension
        InputSystem.RegisterLayoutOverride(@"
{""name"": ""DualShock4GamepadHIDCustom"",
 ""extend"": ""DualShock4GamepadHID"",
 ""controls"": [
   { ""name"": ""gyro"",    ""format"": ""VC3S"", ""offset"": 13, ""layout"": ""Vector3"" },
   { ""name"": ""gyro/x"",  ""format"": ""SHRT"", ""offset"": 0 },
   { ""name"": ""gyro/y"",  ""format"": ""SHRT"", ""offset"": 2 },
   { ""name"": ""gyro/z"",  ""format"": ""SHRT"", ""offset"": 4 },
   { ""name"": ""accel"",   ""format"": ""VC3S"", ""offset"": 19, ""layout"": ""Vector3"" },
   { ""name"": ""accel/x"", ""format"": ""SHRT"", ""offset"": 0 },
   { ""name"": ""accel/y"", ""format"": ""SHRT"", ""offset"": 2 },
   { ""name"": ""accel/z"", ""format"": ""SHRT"", ""offset"": 4 }
 ]}"
        );

        // Gyroscope input callback
        var action = new InputAction(binding: "<Gamepad>/gyro");
        action.performed += ctx => _accGyro *= GyroInputToRotation(ctx);
        action.Enable();
    }

    void Update()
    {
        // Current status
        var rot = transform.localRotation;
        var gyro = _accGyro;
        _accGyro = Quaternion.identity;

        // Delta rotation from gyroscope
        rot *= new Quaternion(-gyro.x, -gyro.y, gyro.z, gyro.w);

        // Gravity vector from accelerometer
        var accel = Gamepad.current.GetChildControl<Vector3Control>("accel");
        var gravity = Vector3.Scale(accel.ReadValue(), new Vector3(-1, -1, 1));

        // Compensation
        var comp = Quaternion.FromToRotation(rot * gravity, -Vector3.up);

        // Compensation weakening
        comp.w *= 10.0f;
        comp = comp.normalized;

        // Update
        transform.localRotation = comp * rot;
    }
}
