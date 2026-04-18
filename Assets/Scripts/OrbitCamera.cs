using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class OrbitCamera : MonoBehaviour {

    bool scrollUp, scrollRight, scrollDown, scrollLeft;


    public Transform cam;
    public Transform focusRotate;

    public float cameraSpeedZoomFactor = 0.5f;

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

    private float dZoom = 0f;

    private float zoomLevel = 0;
    private float cameraSmoothedZoom;

    
    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        ReadInput();
        CameraRotate();
        CameraZoom();
    }
    private void CameraRotate()
    {
        if(cameraRotateButton)
        {
            Vector3 eulerAnglesPitch = focusRotate.eulerAngles;
            eulerAnglesPitch.x -= mouseDelta.y * rotationSpeed;
            eulerAnglesPitch.x = Mathf.Clamp(eulerAnglesPitch.x, 10f, 85f);
            focusRotate.eulerAngles = eulerAnglesPitch;

            Vector3 eulerAnglesYaw = transform.eulerAngles;
            eulerAnglesYaw.y += mouseDelta.x * rotationSpeed;
            transform.eulerAngles = eulerAnglesYaw;
        }
    }
    private void CameraZoom() {
        cameraSmoothedZoom += (dZoom - cameraSmoothedZoom) * cameraAcceleration * Time.deltaTime;
        float z = cameraAccelerationCurve.Evaluate(Mathf.Abs(cameraSmoothedZoom)) * Mathf.Sign(cameraSmoothedZoom);

        zoomLevel += z * zoomSpeed;
        zoomLevel = Mathf.Clamp(zoomLevel, 0, 1);
        
        cam.position = Vector3.Lerp(zoomMin.position, zoomMax.position, zoomCurve.Evaluate(zoomLevel));
        
    }

    public void ReadInput()
    {
        var mouse = Mouse.current;
        cameraRotateButton = mouse.rightButton.isPressed;

        if (mouse.scroll.value.y > 0)
        {
            dZoom = 1f;
        }
        else if (mouse.scroll.value.y < 0)
        {
            dZoom = 1f;
        }
    }

}
