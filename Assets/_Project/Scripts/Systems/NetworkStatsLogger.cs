using System;
using System.IO;
using System.Text;
using UnityEngine;

public class NetworkStatsLogger
{
    private string _csvFilePath;
    private bool _fileCreated = false;
    private StringBuilder _stringBuilder = new StringBuilder();

    public NetworkStatsLogger(string fileName = "NetworkStats")
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string directory = System.IO.Path.Combine("D:\\", "NetworkLogs");

        // Create directory if it doesn't exist
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _csvFilePath = System.IO.Path.Combine(directory, $"{fileName}_{timestamp}.csv");
        CreateCsvFile();
    }

    private void CreateCsvFile()
    {
        try
        {
            // Create file with headers
            _stringBuilder.Clear();
            _stringBuilder.AppendLine("Timestamp,Ping(ms),ReceiveRate(KBps),SendRate(KBps)");
            File.WriteAllText(_csvFilePath, _stringBuilder.ToString());
            _fileCreated = true;
            Debug.Log($"Network stats CSV file created at: {_csvFilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create CSV file: {e.Message}");
            _fileCreated = false;
        }
    }

    private int _pendingEntries = 0;
    private const int BatchSize = 10; // Write after accumulating this many entries

    public void LogStats(int ping, float receiveRateKBps, float sendRateKBps)
    {
        if (!_fileCreated) return;

        try
        {
            // Create the CSV line and append to the StringBuilder
            string csvLine =
                $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff},{ping},{receiveRateKBps.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)},{sendRateKBps.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}";
            _stringBuilder.AppendLine(csvLine);

            _pendingEntries++;

            // Write to file when we've accumulated enough entries or force write on application quit
            if (_pendingEntries >= BatchSize)
            {
                FlushToFile();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to log stats: {e.Message}");
        }
    }

    public void FlushToFile()
    {
        if (!_fileCreated || _pendingEntries == 0) return;

        try
        {
            File.AppendAllText(_csvFilePath, _stringBuilder.ToString());
            _stringBuilder.Clear();
            _pendingEntries = 0;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to append stats to CSV file: {e.Message}");
        }
    }
}