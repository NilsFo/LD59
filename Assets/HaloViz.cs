using UnityEngine;

[ExecuteAlways]
public class HaloViz : MonoBehaviour
{
    public float angle = 45f;

    public float height = 1;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float xz = Mathf.Tan(angle * Mathf.Deg2Rad) * height;
        float y = height;
        transform.localScale = new Vector3(xz, y, xz);
    }
}
