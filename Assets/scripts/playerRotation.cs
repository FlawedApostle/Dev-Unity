using UnityEngine;
/// <summary>
///  Player Rotation keeps the logic seperate - Rigid Body and the camera Controller
///  This avoids physics conflicts and respects the rigidBody constraints
///  essentially i had cemera stutter when I tried to use the camera in the camera
///  based off the rigidbody. for seme reason the pyhsics were fighting
///  after a long ass time of re search i found that if i modulate (seperate) 
///  the logic it should work. 
///  So here we are getting ONLY the rigid Body of the capsule/character
///  the fixedUpdate() is taking the cameraLook rotation every frame without missing
/// where as Update() will not work and itll stutter again
/// using a quaternion we rotae on the y axis. rotating hte rigid body by the quaternion
/// </summary>

public class PlayerRotation : MonoBehaviour
{
    public CameraLook cameraLook;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float mouseX = cameraLook.mouseXInput;
        Quaternion deltaRotation = Quaternion.Euler(0f, mouseX, 0f);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }
}
