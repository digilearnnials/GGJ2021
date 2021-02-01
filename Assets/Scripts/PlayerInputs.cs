using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInputs : MonoBehaviour
{
    public UnityEvent<Vector3> MoveInput = new UnityEvent<Vector3>();

    private void Update()
    {
        MoveInput?.Invoke(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")));
    }
}
