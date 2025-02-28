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
                byte[] data = new byte[8];
                int bytes = stream.Read(data, 0, data.Length);

                // Convert the byte array to a string
                string message = Encoding.UTF8.GetString(data, 0, bytes);

                // Split the message into angle and trigger state
                string packet = message.Split(';')[0];
                string[] parts = packet.Split(',');
                
                if (parts.Length == 3)
                {
                    //Debug.Log(parts[1] + " " + parts[2]);
                    int.TryParse(parts[1], out int angle); int.TryParse(parts[2], out int triggerPressed);
                    //{
                        // Output the angle and trigger state
                        Debug.Log("Received Angle: " + angle + ", Trigger State: " + triggerPressed);
                    //}
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
