using UnityEngine;

public class SelfRotator : MonoBehaviour
{
    public float rotationSpeed = 50;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rot.y += rotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(rot);
    }
}