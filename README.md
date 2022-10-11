# GyroInputTest

![GIF](https://user-images.githubusercontent.com/343936/194891664-4f126bee-0af8-44d0-818f-c9b73204de85.gif)

**GyroInputTest** is an extension for [Unity Input System] that adds gyroscope input support for DualShock 4 gamepads.

[Unity Input System]: https://docs.unity3d.com/Packages/com.unity.inputsystem@latest

### Known issues

- The constants for gyroscopic data from DualShock 4 are undocumented and
  unknown. I put plausible values just by guessing. It seems to work but may
  contain small errors.
- I implemented gyroscope drift compensation for the pitch/bank angles. It does
  nothing about the yaw angle; It may drift constantly.

### Acknowledgements

The initial idea is based on [SG4YK's implementation].

[SG4YK's implementation]: https://blog.sg4yk.com/dual_shock_motion_in_unity_en.html
