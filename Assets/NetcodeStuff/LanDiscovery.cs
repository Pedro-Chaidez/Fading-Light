using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class LanDiscovery : MonoBehaviour
{
    public static LanDiscovery Instance { get; private set; }

    private UdpClient udpBroadcaster;
    private UdpClient udpListener;
    private IPEndPoint broadcastEndPoint;
    private bool isBroadcasting = false;
    private bool isListening = false;

    private const int DISCOVERY_PORT = 47777; 
    private const string BROADCAST_MESSAGE = "GHOST_LOBBY_OPEN";

    public Action<string> OnServerFound; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartBroadcasting()
    {
        StopBroadcasting();
        
        try 
        {
            udpBroadcaster = new UdpClient();
            udpBroadcaster.EnableBroadcast = true;
            broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, DISCOVERY_PORT);
            isBroadcasting = true;
            Debug.Log("LAN Discovery: Started Broadcasting...");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to start broadcast: {e.Message}");
        }
    }

    public void StopBroadcasting()
    {
        isBroadcasting = false;
        if (udpBroadcaster != null)
        {
            udpBroadcaster.Close();
            udpBroadcaster = null;
        }
    }

    private void Update()
    {
        if (isBroadcasting && udpBroadcaster != null)
        {
            if (Time.frameCount % 60 == 0)
            {
                byte[] data = Encoding.UTF8.GetBytes(BROADCAST_MESSAGE);
                udpBroadcaster.Send(data, data.Length, broadcastEndPoint);
            }
        }

        if (isListening && udpListener != null && udpListener.Available > 0)
        {
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] receivedBytes = udpListener.Receive(ref remoteEP);
                string message = Encoding.UTF8.GetString(receivedBytes);

                if (message == BROADCAST_MESSAGE)
                {
                    string serverIP = remoteEP.Address.ToString();
                    OnServerFound?.Invoke(serverIP);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error receiving LAN data: {e.Message}");
            }
        }
    }

    public void StartListening()
    {
        StopListening();

        try
        {
            udpListener = new UdpClient();
            udpListener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpListener.Client.Bind(new IPEndPoint(IPAddress.Any, DISCOVERY_PORT));
            udpListener.EnableBroadcast = true;
            isListening = true;
            Debug.Log("LAN Discovery: Started Listening...");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to start listening (Port might be busy): {e.Message}");
        }
    }

    public void StopListening()
    {
        isListening = false;
        if (udpListener != null)
        {
            udpListener.Close();
            udpListener = null;
        }
    }

    private void OnDestroy()
    {
        StopBroadcasting();
        StopListening();
    }
}