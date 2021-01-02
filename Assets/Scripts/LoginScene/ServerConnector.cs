using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerConnector : MonoBehaviour
{
    public string serverIP = "127.0.0.1";
    public int serverPort = 5867;
    public uint timeBetweenConnectionAttemptInSeconds = 3;
    
    private float _lastConnectionAttemptTimestamp = 0;

    void Start()
    {
        
    }

    private void Update()
    {
        if (NetworkManager.ConnectedToServer) 
            return;
        
        if (Time.realtimeSinceStartup >= _lastConnectionAttemptTimestamp + timeBetweenConnectionAttemptInSeconds)
        {
            Debug.Log("Connecting to server...");
            if(NetworkManager.TryInitializeConnectionToServer(serverIP, serverPort) == true)
                Debug.Log("Connected to server!");
            else
                Debug.Log("Connection failed... Next attempt in " + timeBetweenConnectionAttemptInSeconds + " seconds!");
                
            _lastConnectionAttemptTimestamp = Time.realtimeSinceStartup;
        }
    }
}
