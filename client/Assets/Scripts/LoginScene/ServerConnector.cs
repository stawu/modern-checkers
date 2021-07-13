using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ServerConnector : MonoBehaviour
{
    public string serverIP = "127.0.0.1";
    public int serverPort = 5867;
    public uint timeBetweenConnectionAttemptInSeconds = 3;

    public UnityEvent onConnectedToServer;
    public UnityEvent onDisconnectedFromServer;
    
    private float _lastConnectionAttemptTimestamp = 0;
    private bool _connectedToServer = false;

    private void Update()
    {
        if (NetworkManager.ConnectedToServer) 
            return;

        if (_connectedToServer)
        {
            _connectedToServer = false;
            onDisconnectedFromServer.Invoke();
        }

        if (Time.realtimeSinceStartup >= _lastConnectionAttemptTimestamp + timeBetweenConnectionAttemptInSeconds)
        {
            Debug.Log("Connecting to server...");
            if (NetworkManager.TryInitializeConnectionToServer(serverIP, serverPort) == true)
            {
                Debug.Log("Connected to server!");
                onConnectedToServer.Invoke();
                _connectedToServer = true;
            }
            else
                Debug.Log("Connection failed... Next attempt in " + timeBetweenConnectionAttemptInSeconds + " seconds!");
                
            _lastConnectionAttemptTimestamp = Time.realtimeSinceStartup;
        }
    }
}
