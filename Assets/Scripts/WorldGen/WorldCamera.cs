﻿using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class WorldCamera : MonoBehaviour {
    public bool clampRotation = true;
    [SerializeField] private float minZoom = 10f, zoomSensitivity = 10f, smoothing = 0.1f, keyboardSensitivity = 10f, panSensitivity = 1f, rotateSensitivity = 1f;

    [SerializeField, ReadOnly] private float zoom;

    private const int MouseButtonPan = 2, MouseButtonRotate = 0;

    [ReadOnly] public Vector3 target;

    private Vector3 Position => target - transform.forward * zoom;

    [HideInInspector] public new Camera camera;

    private void Awake() {
        camera = GetComponent<Camera>();
    }

    public void Set(Vector3 position) {
        transform.eulerAngles = new Vector3(90, 0, 0);
        zoom = position.y;

        target = new Vector3(position.x, 0, position.z);
    }

    private void Move(float x, float z) {
        var movement = Time.deltaTime * new Vector3(x, 0, z);

        target += transform.right * movement.x;

        var movementZ = transform.forward * movement.z;
        var magnitude = movementZ.magnitude;
        movementZ.y = 0;

        target += magnitude * movementZ.normalized;

        target.x = Mathf.Clamp(target.x, 0, GameController.World.width);
        target.z = Mathf.Clamp(target.z, 0, GameController.World.height);

        transform.position = Position;
    }

    private void Rotate(float xAngle, float yAngle) {
        transform.RotateAround(target, Vector3.up, xAngle * Time.deltaTime);
        transform.RotateAround(target, transform.right, yAngle * Time.deltaTime);

        var clampedRotation = transform.eulerAngles;

        if (clampedRotation.x > 180f) clampedRotation.x = 0;

        if (clampRotation) transform.eulerAngles = clampedRotation;

        transform.position = Position;
    }

    private void Zoom(float amount) {
        zoom = Mathf.Clamp(zoom + amount, minZoom, GameController.World.LongSide);
    }

    private void Update() {
        if (GameController.World == null) return;

        var input = Input.GetAxis("Mouse ScrollWheel");

        if (input < 0) Zoom(zoom / 50 * zoomSensitivity);
        else if (input > 0) Zoom(-(zoom / 50 * zoomSensitivity));

        Move(keyboardSensitivity * Input.GetAxis("Horizontal"), keyboardSensitivity * Input.GetAxis("Vertical"));

        if (Input.GetMouseButton(MouseButtonPan)) {
            var x = -Input.GetAxisRaw("Mouse X");
            var y = -Input.GetAxisRaw("Mouse Y");

            if (Math.Abs(x) > 0.01f || Math.Abs(y) > 0.01f) Move(x * panSensitivity * (zoom / 100), y * panSensitivity * (zoom / 100));
        } else if (Input.GetMouseButton(MouseButtonRotate)) {
            var x = Input.GetAxisRaw("Mouse X");
            var y = -Input.GetAxisRaw("Mouse Y");

            if (Math.Abs(x) > 0.01f || Math.Abs(y) > 0.01f) Rotate(x * rotateSensitivity, y * rotateSensitivity);
        } else {
            transform.position = (transform.position - Position).magnitude > 0.001f
                ? Vector3.Lerp(transform.position, Position, smoothing)
                : Position;
        }
    }
}
