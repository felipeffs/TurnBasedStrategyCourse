using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"There's more than one InputManager! {transform} - {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Vector2 GetMouseScreenPosition()
    {
        return Input.mousePosition;
    }

    public bool IsMouseButtonDown()
    {
        return Input.GetMouseButton(0);
    }

    public Vector2 GetCameraMoveVector()
    {
        Vector2 inputMoveDirection = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDirection.y += 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDirection.y -= 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDirection.x -= 1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDirection.x += 1f;
        }

        return inputMoveDirection;
    }

    public float GetCameraRotateAmount()
    {
        float rotationAmount = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            rotationAmount = 1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotationAmount = -1f;
        }

        return rotationAmount;
    }

    public float GetCameraZoomAmount()
    {
        float zoomAmount = 0f;

        if (Input.mouseScrollDelta.y > 0)
        {
            zoomAmount = -1f;
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            zoomAmount = 1f;
        }

        return zoomAmount;
    }
}
