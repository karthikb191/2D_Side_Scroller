using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    [SerializeField]
    private Vector3 offset;

    PlayerMovement player;
    private Vector3 initialPosition;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        initialPosition = gameObject.transform.position;
        GameManager.Instance.ResetGameEvent += ResetCamera;
    }
    private void LateUpdate()
    {
        if (player.Health <= 0)
            return;
        gameObject.transform.position = offset + new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
    }
    void ResetCamera()
    {
        gameObject.transform.position = initialPosition;
    }
}
