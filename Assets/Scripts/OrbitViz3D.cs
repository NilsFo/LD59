using UnityEngine;

public class OrbitViz3D : MonoBehaviour
{
    public Orbit orbit;

    public Transform orbitStartViz;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateOrbit();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateOrbit();
    }

    void UpdateOrbit()
    {
        transform.rotation = Quaternion.LookRotation(orbit.OrbitStart, orbit.OrbitAxis);
        orbitStartViz.transform.position = orbit.OrbitStart * orbit.height;
    }
}
