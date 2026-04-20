using UnityEngine;

public class SelfRotator : MonoBehaviour
{
    public float rotationSpeed = 50;
    public Vector3 axis = Vector3.forward;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var q = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, axis);
        transform.localRotation *= q;
    }
}