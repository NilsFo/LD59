using UnityEngine;

public class OrbitViz3D : MonoBehaviour
{
    public Orbit orbit;

    public Transform orbitStartViz;
    public Transform orbitViz;
    public Transform orbitPreviewViz;

    public bool isPreview;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateOrbit();
        orbitStartViz.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateOrbit();
    }

    void UpdateOrbit()
    {
        if (isPreview)
        {
            orbitViz.gameObject.SetActive(false);
            orbitPreviewViz.gameObject.SetActive(true);
        }
        else
        {
            orbitViz.gameObject.SetActive(true);
            orbitPreviewViz.gameObject.SetActive(false);
        }
        transform.rotation = Quaternion.LookRotation(orbit.OrbitStart, orbit.OrbitAxis);
        transform.localScale = Vector3.one * orbit.height;
        orbitStartViz.transform.position = orbit.OrbitStart * orbit.height;
    }
}