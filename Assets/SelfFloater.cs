using UnityEngine;

public class SelfFloater : MonoBehaviour
{
    public float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.localPosition;
        pos.y += speed * Time.unscaledDeltaTime;
        transform.localPosition = pos;
    }
}