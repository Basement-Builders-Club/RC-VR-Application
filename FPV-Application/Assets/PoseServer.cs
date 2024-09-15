using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class PoseServer : MonoBehaviour
{
    private TcpListener server;
    private TcpClient client;
    private NetworkStream stream;

    public GameObject wheel;
    private bool rightTriggerPressed;
    private bool leftTriggerPressed;
    private PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Forward.performed += OnRightTriggerAction;
        inputActions.Player.Forward.canceled += OnRightTriggerAction;
        inputActions.Player.Backward.performed += OnLeftTriggerAction;
        inputActions.Player.Backward.canceled += OnLeftTriggerAction;
    }

    void OnDisable()
    {
        inputActions.Player.Forward.performed -= OnRightTriggerAction;
        inputActions.Player.Forward.canceled -= OnRightTriggerAction;
        inputActions.Player.Backward.performed -= OnLeftTriggerAction;
        inputActions.Player.Backward.canceled -= OnLeftTriggerAction;
        inputActions.Disable();
    }

    private void OnRightTriggerAction(InputAction.CallbackContext context)
    {
        rightTriggerPressed = context.ReadValueAsButton();
    }

    private void OnLeftTriggerAction(InputAction.CallbackContext context)
    {
        leftTriggerPressed = context.ReadValueAsButton();
    }

    void Start()
    {
        // Start the server on port 8080
        server = new TcpListener(IPAddress.Any, 8080);
        server.Start();
        Debug.Log("Server started on port 8080");

        // Accept a client connection asynchronously
        server.BeginAcceptTcpClient(new AsyncCallback(OnClientConnected), null);
    }

    void OnClientConnected(IAsyncResult ar)
    {
        client = server.EndAcceptTcpClient(ar);
        stream = client.GetStream();
        Debug.Log("Client connected");
    }

    void Update()
    {
        if (client != null && client.Connected)
        {
            // Get the wheel's rotation about the z axis
            int angle = (int) wheel.transform.eulerAngles.z;

            // Create the message to send
            int triggerVal = 1;
            if (rightTriggerPressed)
            {
                triggerVal += 1;
            }
            if (leftTriggerPressed)
            {
                triggerVal -= 1;
            }

            string message = $"~,{angle},{triggerVal};";

            // Convert the message to a byte array
            byte[] messageData = Encoding.UTF8.GetBytes(message);

            // Send the message data to the client
            stream.Write(messageData, 0, messageData.Length);
            Debug.Log("Sent Message: " + message);
        }
    }

    void OnApplicationQuit()
    {
        // Clean up the server and client when the application quits
        stream?.Close();
        client?.Close();
        server?.Stop();
    }
}
