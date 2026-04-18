using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Vector3 OrbitAxis { get; private set; } = Vector3.up;
    public Vector3 OrbitStart { get; private set; } = Vector3.left;
    public float inclination = 45f;
    public float equator = 45f;
    public float height = 1.1f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        OrbitStart = (Quaternion.AngleAxis(equator, Vector3.up) * Vector3.forward);
        OrbitAxis =  Quaternion.AngleAxis(inclination, OrbitStart) * Vector3.up;
        Debug.Log(OrbitAxis);
        Debug.Log(OrbitStart);
        Debug.Log(Vector3.Cross(OrbitAxis, OrbitStart));
    }
    
    // Omega ist die rotation um den orbit in radianten zwsichen 0 und 2*pi, startet am äquator
    public Vector3 GetOrbitPosition(float omega)
    {
        return Quaternion.AngleAxis(omega, OrbitAxis) * OrbitStart.normalized * height;
    }
}
