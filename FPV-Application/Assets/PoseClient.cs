using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class PoseClient : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private Thread clientThread;

    void Start()
    {
        clientThread = new Thread(new ThreadStart(ConnectToServer));
        clientThread.IsBackground = true;
        clientThread.Start();
    }

    void ConnectToServer()
    {
        try
        {
            // Connect to the server on the same machine, port 8080
            client = new TcpClient("127.0.0.1", 8080);
            stream = client.GetStream();
            Debug.Log("Connected to server");

            while (true)
            {
                // Buffer to store the data
                byte[] data = new byte[256];
                int bytes = stream.Read(data, 0, data.Length);

                // Convert the byte array to a string
                string message = Encoding.UTF8.GetString(data, 0, bytes);

                // Split the message into angle and trigger state
                string[] parts = message.Split(',');
                if (parts.Length == 2)
                {
                    if (float.TryParse(parts[0], out float angle) && bool.TryParse(parts[1], out bool triggerPressed))
                    {
                        // Output the angle and trigger state
                        Debug.Log("Received Angle: " + angle + ", Trigger Pressed: " + triggerPressed);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Client Error: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        // Clean up
        clientThread.Abort();
        stream?.Close();
        client?.Close();
    }
}
