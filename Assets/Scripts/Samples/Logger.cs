using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Logger : MonoBehaviour
{

    private static Logger instance;

    [SerializeField] // serialized for debug purpouse
    List<string> log = new List<string>();


    [SerializeField]
    TMP_Text results;


    [SerializeField] // serialized for debug purpouse
    bool forcePopulate; 

    [SerializeField]
    int maxRows = 10;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        Application.logMessageReceived += Application_logMessageReceived;
    }

    private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        AddNewRow(condition);
    }

    public void AddNewRow(string row)
    {
        log.Add(row);
        forcePopulate = true;
    }

    public void RemoveRow(string row) { }
    public void Clear() { }

    void Populate()
    {
        while (log.Count > maxRows)
        {
            log.RemoveAt(0);
        }

        string res = string.Empty;
        foreach (var row in log)
        {
            res += row + "\r\n";
        }

        if (results != null)
            results.text = res;
    }


    void Update()
    {
        if (forcePopulate)
        {
            Populate ();
            forcePopulate = false;
        }
        
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= Application_logMessageReceived;
    }
}
