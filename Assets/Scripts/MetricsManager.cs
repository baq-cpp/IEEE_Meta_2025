using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MetricsManager : MonoBehaviour
{
    public static MetricsManager Instance { get; private set; }

    [Serializable]
    public class LabSessionData
    {
        public string sessionId;
        public string labId;
        public string startTimeUtc;
        public string endTimeUtc;
        public double durationSeconds;
        public bool completed;
    }

    private LabSessionData currentSession;
    private DateTime startTime;
    private string pendingLab;
    private bool sessionActive;

    private void Awake()
    {
        // Only allow one MetricsManager to exist.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keep this object when changing scenes.
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called before SelectionManager loads the lab.
    public void PrepareLab(string labName)
    {
        pendingLab = labName;

        Debug.Log("Preparing metric session for: " + pendingLab);
    }

    //  Called after scene loads
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (string.IsNullOrEmpty(pendingLab))
            return;

        if (scene.name == pendingLab)
        {
            StartLab(scene.name);
            pendingLab = "";
        }
    }

    private void StartLab(string labName)
    {
        if (sessionActive)
        {
            Debug.LogWarning("A lab session is already active.");
            return;
        }

        startTime = DateTime.UtcNow;

        currentSession = new LabSessionData
        {
            sessionId = Guid.NewGuid().ToString(),
            labId = labName,
            startTimeUtc = startTime.ToString("o"),
            endTimeUtc = "",
            durationSeconds = 0,
            completed = false
        };

        sessionActive = true;

        Debug.Log(
            "Started timing " + labName +
            " at " + currentSession.startTimeUtc
        );
    }

    // Called by End button
    public void EndLab()
    {
        if (!sessionActive)
        {
            Debug.LogWarning("There is no active lab session.");
            return;
        }

        DateTime endTime = DateTime.UtcNow;

        currentSession.endTimeUtc = endTime.ToString("o");
        currentSession.durationSeconds =
            Math.Round((endTime - startTime).TotalSeconds,0);
        currentSession.completed = true;

        SaveSession();

        sessionActive = false;

        Debug.Log(
            "Completed " + currentSession.labId +
            " in " + currentSession.durationSeconds +
            " seconds."
        );
    }

    private void SaveSession()
    {
        string json = JsonUtility.ToJson(currentSession, true);

        string metricsFolder = Path.Combine(
            Application.persistentDataPath,
            "Metrics"
        );

        Directory.CreateDirectory(metricsFolder);

        // Timestamp prevents previous sessions from being overwritten
        string fileName =
            "LabSession_" +
            DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff") +
            ".json";

        string filePath = Path.Combine(metricsFolder, fileName);

        File.WriteAllText(filePath, json);

        Debug.Log("JSON saved to: " + filePath);
    }
}
