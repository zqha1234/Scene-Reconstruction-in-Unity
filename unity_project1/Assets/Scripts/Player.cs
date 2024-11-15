using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private bool jumpKeyWasPressed;
    private float horizontalInput;
    private Rigidbody rb;
    private GameObject board;
    private Vector3 p1;
    private Vector3 p2;
    private float d_p1;
    private float d_p2;
    private readonly float boardSpeed = 2.0f;
    private int direction;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        board = GameObject.FindWithTag("Board");
        p1 = new Vector3((float)5.5, (float)2.3, 0);
        p2 = new Vector3(4, (float)2.3, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            jumpKeyWasPressed = true;
        }
        horizontalInput = Input.GetAxis("Horizontal");
        d_p1 = Vector3.Distance(board.transform.position, p1);
        d_p2 = Vector3.Distance(board.transform.position, p2);
        if (d_p1 < 0.01f)
        {
            direction = 1;
        }
        else if (d_p2 < 0.01f)
        {
            direction = 0;
        }
    }

    // FixedUpdate is called once per physic update
    private void FixedUpdate()
    {
        if (jumpKeyWasPressed)
        {
            rb.AddForce(Vector3.up * 4, ForceMode.VelocityChange);
            jumpKeyWasPressed = false;
        }
        rb.velocity = new Vector3(horizontalInput * 3, rb.velocity.y, 0);
        if (direction == 1)
        {
            board.transform.position = Vector3.MoveTowards(board.transform.position, p2, boardSpeed * Time.deltaTime);
        }
        else
        {
            board.transform.position = Vector3.MoveTowards(board.transform.position, p1, boardSpeed * Time.deltaTime);
        }
    }



}