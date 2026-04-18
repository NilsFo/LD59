using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Vector3 OrbitAxis { get; private set; } = Vector3.up;
    public Vector3 OrbitStart { get; private set; } = Vector3.left;
    public float height = 1.1f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
    }

    public void SetFromIncEq(float inclination, float equator)
    {
        OrbitStart = Quaternion.AngleAxis(equator, Vector3.up) * Vector3.forward;
        OrbitAxis =  Quaternion.AngleAxis(inclination, OrbitStart) * Vector3.up;

    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(OrbitAxis);
        // Debug.Log(OrbitStart);
        // Debug.Log(Vector3.Cross(OrbitAxis, OrbitStart));
    }
    
    // Omega ist die rotation um den orbit in radianten zwsichen 0 und 2*pi, startet am äquator
    public Vector3 GetOrbitPosition(float omega)
    {
        return Quaternion.AngleAxis(omega, OrbitAxis) * OrbitStart.normalized * height;
    }

    public float SetNewOrbit(Vector3 start, Vector3 vec)
    {
        if (Vector3.Dot(start, vec) == 0)
        {
            return 0;
        }
        start.Normalize();
        vec.Normalize();
        Vector3 newNormal = Vector3.Cross(start, vec);
        Vector3 newStart = Vector3.Cross(newNormal, Vector3.up);
        Debug.DrawLine(Vector3.zero, start*2, Color.blue);
        Debug.DrawLine(Vector3.zero, vec*2, Color.green);
        Debug.DrawLine(Vector3.zero, newNormal*2, Color.red);
        Debug.DrawLine(Vector3.zero, newStart*2, Color.yellow);
        // inclination = Vector3.Angle(newNormal, Vector3.Cross(newStart, Vector3.up));
        // equator = Vector3.SignedAngle(Vector3.forward, newStart, Vector3.up);
        OrbitStart = newStart;
        OrbitAxis = newNormal;
        float newOmega = Vector3.SignedAngle(OrbitStart, start, OrbitAxis);
        return newOmega;
    }

    public void SetFromOrbit(Orbit orbit)
    {
        OrbitAxis = orbit.OrbitAxis;
        OrbitStart = orbit.OrbitStart;
    }

    public Vector2 OrbitPosToEquirect(float omega)
    {
        Vector2 pos = new Vector2();
        var orbitPos = GetOrbitPosition(omega);
        //pos.x = orbitPos

        return pos;
    }
}
