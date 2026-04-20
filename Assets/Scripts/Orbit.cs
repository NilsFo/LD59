using System;
using Unity.VisualScripting;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public const float LeoValue = 1.3f;
    public const float MeoValue = 1.7f;
    public const float GeoValue = 2.5f;

    public enum OrbitState : Int32
    {
        LEO = 0,
        MEO = 1,
        GEO = 2
    }

    public Vector3 OrbitAxis { get; private set; } = Vector3.up;
    public Vector3 OrbitStart { get; private set; } = Vector3.left;
    public float height = 1.1f;
    
    public float rotationSpeed = 10.0f;
    public float rotationSpeedMeo = 5.0f;
    public float rotationSpeedGeo = 0.0f;

    public float ascendingSpeed = 10.0f;
    public float descendingSpeed = 10.0f;

    public OrbitState orbitState = OrbitState.LEO;
    public OrbitState targetOrbitState = OrbitState.LEO;
    public event Action<OrbitState> OnOrbitChanged;

    public OrbitViz3D orbitViz3D;
    
    public bool IsVisible { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        switch (orbitState)
        {
            case OrbitState.LEO:
                height = LeoValue;
                break;
            case OrbitState.MEO:
                height = MeoValue;
                break;
            case OrbitState.GEO:
                height = GeoValue;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        if(IsVisible)
            Show();
        else
            Hide();
    }

    public void SetFromIncEq(float inclination, float equator)
    {
        OrbitStart = Quaternion.AngleAxis(equator, Vector3.up) * Vector3.forward;
        OrbitAxis = Quaternion.AngleAxis(inclination, OrbitStart) * Vector3.up;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(OrbitAxis);
        // Debug.Log(OrbitStart);
        // Debug.Log(Vector3.Cross(OrbitAxis, OrbitStart));
        if (orbitState != targetOrbitState)
        {
            if (orbitState == OrbitState.LEO)
            {
                //Accend
                height += Time.deltaTime * ascendingSpeed;
                if (height >= MeoValue)
                {
                    orbitState = OrbitState.MEO;
                    height = MeoValue;
                    OnOrbitChanged?.Invoke(orbitState);
                }
                if (height >= GeoValue)
                {
                    orbitState = OrbitState.GEO;
                    height = GeoValue;
                    OnOrbitChanged?.Invoke(orbitState);
                }
            }
            else if (orbitState == OrbitState.MEO)
            {
                if (targetOrbitState == OrbitState.LEO)
                {
                    //Desent
                    height -= Time.deltaTime * descendingSpeed;
                    if (height <= LeoValue)
                    {
                        orbitState = OrbitState.LEO;
                        height = LeoValue;
                        OnOrbitChanged?.Invoke(orbitState);
                    }
                }
                else
                {
                    //Accend
                    height += Time.deltaTime * ascendingSpeed;
                    if (height >= GeoValue)
                    {
                        orbitState = OrbitState.GEO;
                        height = GeoValue;
                        OnOrbitChanged?.Invoke(orbitState);
                    }
                }
            }
            else //GEO
            {
                //Decent
                height -= Time.deltaTime * descendingSpeed;
                if (height <= MeoValue)
                {
                    orbitState = OrbitState.MEO;
                    height = MeoValue;
                    OnOrbitChanged?.Invoke(orbitState);
                }

                if (height <= LeoValue)
                {
                    orbitState = OrbitState.LEO;
                    height = LeoValue;
                    OnOrbitChanged?.Invoke(orbitState);
                }
            }
        }

        if (orbitState == OrbitState.GEO)
        {
            Hide();
        }
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
        Debug.DrawLine(Vector3.zero, start * 2, Color.blue);
        Debug.DrawLine(Vector3.zero, vec * 2, Color.green);
        Debug.DrawLine(Vector3.zero, newNormal * 2, Color.red);
        Debug.DrawLine(Vector3.zero, newStart * 2, Color.yellow);
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

    public bool SetLeo()
    {
        if (orbitState == targetOrbitState)
        {
            targetOrbitState = OrbitState.LEO;
            return true;
        }

        return false;
    }

    public bool SetMeo()
    {
        if (orbitState == targetOrbitState)
        {
            targetOrbitState = OrbitState.MEO;
            return true;
        }

        return false;
    }

    public bool SetGeo()
    {
        if (orbitState == targetOrbitState)
        {
            targetOrbitState = OrbitState.GEO;
            return true;
        }

        return false;
    }

    public void Show()
    {
        if(orbitState == OrbitState.GEO) return;
        IsVisible = true;
        orbitViz3D.gameObject.SetActive(true);
    }

    public void Hide()
    {
        IsVisible = false;
        orbitViz3D.gameObject.SetActive(false);
    }

    public float Speed
    {
        get
        {
            if (orbitState == OrbitState.GEO) return rotationSpeedGeo;
            if (orbitState == OrbitState.MEO) return rotationSpeedMeo;
            return rotationSpeed;
        }
    }
}