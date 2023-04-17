using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollyTrack : MonoBehaviour
{
    [SerializeField] private DollyNode[] nodes;
    [SerializeField] private float defaultSpeed, blendSpeed;

    private Quaternion originalRot;
    private Vector3 originalPos;
    private int nodePointer;
    private float originalSpeed, speed, distance, journeyProgress;

    private void Start()
    {
        speed = defaultSpeed;

        originalPos = transform.position;
        originalRot = transform.rotation;
        originalSpeed = speed;

        distance = (nodes[nodePointer].node.position - transform.position).magnitude;
    }

    private void Update()
    {
        //Update journey progress based on speed and distance
        journeyProgress += (speed * Time.deltaTime) / distance;

        //Lerp position, rotation, and speed for a smoother transition
        transform.position = Vector3.Lerp(transform.position, Vector3.Lerp(originalPos, nodes[nodePointer].node.position, journeyProgress), blendSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Lerp(originalRot, nodes[nodePointer].node.rotation, journeyProgress), blendSpeed * Time.deltaTime);
        speed = Mathf.Lerp(speed, Mathf.Lerp(originalSpeed, nodes[nodePointer].speed, journeyProgress), blendSpeed * Time.deltaTime);

        //Once a node has been reached, switch to next node
        if (journeyProgress >= 1)
        {
            originalPos = nodes[nodePointer].node.position;
            originalRot = nodes[nodePointer].node.rotation;
            originalSpeed = nodes[nodePointer].speed;
            nodePointer = GameplayManager.instance.NextPointer(nodePointer, nodes.Length);

            distance = (nodes[nodePointer].node.position - transform.position).magnitude;
            journeyProgress = 0;
        }
    }
}

[System.Serializable]
public class DollyNode
{
    public Transform node;
    public float speed;
}
