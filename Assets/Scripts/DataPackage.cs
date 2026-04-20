using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class DataPackage : MonoBehaviour
{
    public List<GameObject> dataPath;
    private int targetIndex = 0;
    public float speed = 1f;

    private void Start()
    {
        if (dataPath.Count == 0)
        {
            Destroy(gameObject);
            return;
        }
        
        transform.position = dataPath[0].transform.position;
        transform.LookAt(dataPath[0].transform, Vector3.up);
    }

    private void Update()
    {
        var curTarget = dataPath[targetIndex].transform;
        transform.position = Vector3.MoveTowards(transform.position, curTarget.position, speed * Time.deltaTime);
        transform.LookAt(curTarget, Vector3.up);
        if (Vector3.SqrMagnitude(transform.position - curTarget.position) < 0.01f)
        {
            targetIndex++;
            if (targetIndex >= dataPath.Count)
            {
                Destroy(gameObject);
            }
        }
    }
}