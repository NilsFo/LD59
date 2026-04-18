using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCamera : MonoBehaviour
{
    private bool scrollUp, scrollRight, scrollDown, scrollLeft;

    public Transform cam;
    public Transform focusRotate;

    // public float cameraSpeedZoomFactor = 0.5f;

    public AnimationCurve cameraAccelerationCurve;
    public float cameraAcceleration = 6f;

    public Transform zoomMax;
    public Transform zoomMin;
    public float zoomSpeed = 0.5f;
    public AnimationCurve zoomCurve;

    public float rotationSpeed = 0.5f;

    private Vector2 scrollVector = new Vector2();
    private Vector2 keyInputVector;
    private bool cameraRotateButton; // right mouse 
    private Vector2 mouseDelta;
    private float zoomLevelCurrent = 0;
    private float zoomLevelTarget = 0;
    private float cameraSmoothedZoom;

    private Vector3 eulerAnglesPitch;


    // Use this for initialization
    void Start()
    {
        eulerAnglesPitch = focusRotate.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();
        CameraRotate();
        CameraZoom();
    }

    private void CameraRotate()
    {
        if (cameraRotateButton)
        {
            eulerAnglesPitch.x -= mouseDelta.y * rotationSpeed;
            eulerAnglesPitch.x = Mathf.Clamp(eulerAnglesPitch.x, -85f, 85f);
            focusRotate.eulerAngles = eulerAnglesPitch;

            eulerAnglesPitch.y += mouseDelta.x * rotationSpeed;
            focusRotate.eulerAngles = eulerAnglesPitch;
        }
    }

    private void CameraZoom()
    {
        cameraSmoothedZoom += (zoomLevelTarget - cameraSmoothedZoom) * cameraAcceleration * Time.unscaledDeltaTime;
        float z = cameraAccelerationCurve.Evaluate(Mathf.Abs(cameraSmoothedZoom)) * Mathf.Sign(cameraSmoothedZoom);

        zoomLevelCurrent += z * zoomSpeed;
        zoomLevelCurrent = Mathf.Clamp(zoomLevelCurrent, 0, 1);

        Vector3 newPosition = Vector3.Lerp(zoomMin.localPosition, zoomMax.localPosition, zoomLevelTarget);
        cam.localPosition = Vector3.MoveTowards(cam.localPosition, newPosition, Time.unscaledDeltaTime * zoomSpeed);
    }

    public void ReadInput()
    {
        Mouse mouse = Mouse.current;
        cameraRotateButton = mouse.rightButton.isPressed;

        if (mouse.scroll.value.y > 0)
        {
            zoomLevelTarget += -0.1f;
        }
        else if (mouse.scroll.value.y < 0)
        {
            zoomLevelTarget += 0.1f;
        }

        zoomLevelTarget = Math.Clamp(zoomLevelTarget, 0, 1);
        mouseDelta = mouse.delta.value;
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Handles.color = Color.white;
        Handles.DrawLine(zoomMin.position, zoomMax.position);

        if (Application.isPlaying)
        {
            Handles.Label(cam.position, "Zoom level: " + zoomLevelCurrent + "/" + zoomLevelTarget);
            Handles.DrawWireCube(zoomMin.transform.position, Vector3.one * 0.01f);
            Handles.DrawWireCube(zoomMax.transform.position, Vector3.one * 0.01f);
        }
    }


#endif
}