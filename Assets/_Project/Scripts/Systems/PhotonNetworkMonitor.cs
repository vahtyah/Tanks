using System;
    using ExitGames.Client.Photon;
    using Photon.Pun;
    using UnityEngine;
    
    public class PhotonNetworkMonitor : PersistentSingletonPunCallbacks<PhotonNetworkMonitor>
    {
        [Serializable]
        public class NetworkStats
        {
            public float ReceiveRateKBps;
            public float SendRateKBps;
            public int Ping;
            public string ConnectionQuality;
        }
    
        public enum MonitorMode
        {
            Debug,
            UI,
            Both
        }

        [SerializeField] private bool isDisabled = false;
        
    
        [Header("Monitor Settings")] 
        [SerializeField]
        private MonitorMode monitorMode = MonitorMode.Debug;
    
        [SerializeField] private float updateInterval = 1f;
        [SerializeField] private bool enableDetailedLogging = false;
        [SerializeField] private bool  logToCSV = false;
        
    
        [Header("Network Thresholds")] [SerializeField]
        private int goodPingThreshold = 50;
    
        [SerializeField] private int poorPingThreshold = 150;
    
        [Header("UI References")] [SerializeField]
        private TMPro.TextMeshProUGUI statsText;
    
        private float _timer;
        private NetworkStats _currentStats = new NetworkStats();
        private long _lastBytesIn;
        private long _lastBytesOut;

        private NetworkStatsLogger statsLogger;
    
        public event Action<NetworkStats> OnNetworkStatsUpdated;
    
        private void Start()
        {
            // Enable traffic statistics tracking in Photon
            if (PhotonNetwork.NetworkingClient?.LoadBalancingPeer != null)
            {
                PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsEnabled = true;
            }

            if (logToCSV)
            {
                statsLogger = new NetworkStatsLogger();
            }
        }
    
        private void Update()
        {
            if (!PhotonNetwork.IsConnectedAndReady || isDisabled) return;
    
            _timer += Time.deltaTime;
            if (_timer < updateInterval) return;
            _timer = 0;
    
            UpdateNetworkStats();
    
            switch (monitorMode)
            {
                case MonitorMode.Debug:
                    LogNetworkStats();
                    break;
                case MonitorMode.UI:
                    UpdateUI();
                    break;
                case MonitorMode.Both:
                    LogNetworkStats();
                    UpdateUI();
                    break;
            }
            
            if (logToCSV && statsLogger != null)
            {
                statsLogger.LogStats(_currentStats.Ping, _currentStats.ReceiveRateKBps, _currentStats.SendRateKBps);
            }
    
            OnNetworkStatsUpdated?.Invoke(_currentStats);
        }
    
        private void UpdateNetworkStats()
        {
            var client = PhotonNetwork.NetworkingClient;
            var peer = client.LoadBalancingPeer;
    
            // Calculate basic rates
            long bytesIn = peer.BytesIn;
            long bytesOut = peer.BytesOut;
    
            long bytesInDelta = bytesIn - _lastBytesIn;
            long bytesOutDelta = bytesOut - _lastBytesOut;
    
            _currentStats.ReceiveRateKBps = bytesInDelta / (updateInterval * 1024f);
            _currentStats.SendRateKBps = bytesOutDelta / (updateInterval * 1024f);
            _currentStats.Ping = PhotonNetwork.GetPing();
    
            // Determine connection quality
            if (_currentStats.Ping < goodPingThreshold)
            {
                _currentStats.ConnectionQuality = "Excellent";
            }
            else if (_currentStats.Ping < poorPingThreshold)
            {
                _currentStats.ConnectionQuality = "Good";
            }
            else if (_currentStats.Ping < poorPingThreshold * 2)
            {
                _currentStats.ConnectionQuality = "Fair";
            }
            else
            {
                _currentStats.ConnectionQuality = "Poor";
            }
    
            // Store current values for next calculation
            _lastBytesIn = bytesIn;
            _lastBytesOut = bytesOut;
        }
    
        private void LogNetworkStats()
        {
            string baseLog = $"Network: {_currentStats.ConnectionQuality} | " +
                             $"Ping: {_currentStats.Ping}ms | " +
                             $"In: {_currentStats.ReceiveRateKBps:F2} KB/s | " +
                             $"Out: {_currentStats.SendRateKBps:F2} KB/s";
    
            if (enableDetailedLogging)
            {
                string detailedLog = baseLog + 
                                     $"\nRoom Players: {PhotonNetwork.CurrentRoom?.PlayerCount ?? 0}";
                Debug.Log(detailedLog);
            }
            else
            {
                Debug.Log(baseLog);
            }
        }
    
        private void UpdateUI()
        {
            if (statsText == null) return;
    
            statsText.text = $"Network: {_currentStats.ConnectionQuality}\n" +
                             $"Ping: {_currentStats.Ping}ms\n" +
                             $"Download: {_currentStats.ReceiveRateKBps:F2} KB/s\n" +
                             $"Upload: {_currentStats.SendRateKBps:F2} KB/s";
        }
    
        public override void OnConnectedToMaster()
        {
            Debug.Log("Network monitor: Connected to Photon Master Server");
            
            // Enable traffic statistics when connected
            if (PhotonNetwork.NetworkingClient?.LoadBalancingPeer != null)
            {
                PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsEnabled = true;
            }
        }
    
        public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
        {
            Debug.LogWarning($"Network monitor: Disconnected from Photon (Reason: {cause})");
        }
    }