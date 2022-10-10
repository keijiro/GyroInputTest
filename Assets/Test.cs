using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

sealed class Test : MonoBehaviour
{
    Quaternion _accGyro;

    void Start()
    {
        InputSystem.RegisterLayoutOverride(@"
{""name"": ""DualShock4GamepadHID2"",
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

        _accGyro = Quaternion.identity;

        var action = new InputAction(type: InputActionType.Value, binding: "<Gamepad>/gyro");

        action.performed += ctx =>
          { var rot = Quaternion.Euler(ctx.ReadValue<Vector3>() * Mathf.PI * 2);
            _accGyro *= rot; };

        action.Enable();
    }

    void Update()
    {
        var accel = Gamepad.current.GetChildControl<Vector3Control>("accel");
        var gravity = Vector3.Scale(accel.ReadValue(), new Vector3(1, -1, -1));

        var a_rot = Quaternion.FromToRotation(new Vector3(0, -1, 0), gravity.normalized);

        var q = new Quaternion(-_accGyro.x, -_accGyro.y, _accGyro.z, _accGyro.w);
        var g_rot = transform.localRotation * q;

        transform.localRotation = Quaternion.Slerp(g_rot, a_rot, Time.deltaTime * 5);

        _accGyro = Quaternion.identity;
    }
}
