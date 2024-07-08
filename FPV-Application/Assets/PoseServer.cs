using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class PoseServer : MonoBehaviour
{
    private TcpListener server;
    private TcpClient client;
    private NetworkStream stream;

    public GameObject targetObject; // The object whose angle you want to send

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
            // Get the object's angle (e.g., rotation around the Z-axis)
            float angle = targetObject.transform.eulerAngles.z;

            // Convert the angle to a byte array
            byte[] angleData = Encoding.UTF8.GetBytes(angle.ToString());

            // Send the angle data to the client
            stream.Write(angleData, 0, angleData.Length);
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
