using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Panel navigation class
/// </summary>
public class PanelNavigation : SingletonBehaviour<PanelNavigation> {

    [Header("General")]
    public int startIndex = 0;
    public int maxHistorySize = 10;
    public bool enableEscapeKey = true;
    public ScreenOrientation defaultOrientation = ScreenOrientation.Portrait;

    [Header("Automatic")]
    public bool autoDetectPanels = false;
    public bool defaultIncludeHistory = true;

    [System.Serializable]
    public class PanelEntry
    {
        public string name;
        public GameObject panel;
        public ScreenOrientation orientation = ScreenOrientation.Portrait;
        public bool includeInHistory = true;
    }

    [Header("Panels")]
    public PanelEntry[] panelEntries;
    
    //Privates
    private Stack<int> indexHistory;
    private int currentIndex = 0;

    void Start() {

        Screen.orientation = defaultOrientation;
        if (startIndex > panelEntries.Length - 1)
        {
            Debug.LogWarningFormat("Starting panel index {0} is out of bounds!", startIndex);
            return;
        }
        indexHistory = new Stack<int>();
        indexHistory.Push(startIndex);
        goToPanel(startIndex);

        if (autoDetectPanels) autoDetect();
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) goToPrevious();
    }

    /// <summary>
    /// Mostly ment for quick testing, it will detect objects that contain 
    /// panel within their name and will add them to the panel entries.
    /// So you can use their object names to navigate them quickly.
    /// </summary>
    private void autoDetect()
    {
        List<PanelEntry> entries = new List<PanelEntry>();
        foreach (Transform child in transform)
        {
            if (child.name.ToLower().Contains("panel"))
            {
                PanelEntry entry = new PanelEntry
                {
                    name = child.name,
                    panel = child.gameObject,
                    orientation = defaultOrientation,
                    includeInHistory = true,

                };
                entries.Add(entry);
            }
        }
        panelEntries = entries.ToArray();
    }

    /// <summary>
    /// Go to the previous panel in history
    /// </summary>
    public void goToPrevious()
    {
        if (indexHistory.Count > 0)
        {
            int previousIndex = indexHistory.Count > 1 ? indexHistory.ElementAt(indexHistory.Count - 2) : 0;
            goToPanel(previousIndex);
            applyPanelSettings();
            indexHistory.Pop();
        }
    }

    /// <summary>
    /// Go to panel by name
    /// </summary>
    /// <param name="name"></param>
    public void goToPanel(string name)
    {
        for(int i = 0; i < panelEntries.Length; i++)
        {
            if(panelEntries[i].name == name) goToPanel(i);
            else panelEntries[i].panel.SetActive(false);
        }
    }

    /// <summary>
    /// Get the current panel entry
    /// </summary>
    /// <returns></returns>
    public PanelEntry currentPanel()
    {
        return panelEntries[currentIndex];
    }

    /// <summary>
    /// Apply current panel orientation
    /// </summary>
    private void applyPanelSettings()
    {
        PanelEntry entry = currentPanel();
        Screen.orientation = entry.orientation;
    }

    /// <summary>
    /// Go to panel by index
    /// </summary>
    /// <param name="index"></param>
    public void goToPanel(int index, bool disableOthers = true)
    {
        //Debug.Log(string.Format("Active panel at index {0}", index));

        int previousIndex = currentIndex;
        currentIndex = index;

        if(disableOthers) disableAllPanels();
        panelEntries[index].panel.SetActive(true);

        applyPanelSettings();
        if (currentIndex != previousIndex && indexHistory.Count < maxHistorySize) indexHistory.Push(previousIndex);
    }

    /// <summary>
    /// Disable all the panels
    /// </summary>
    private void disableAllPanels()
    {
        for (int i = 0; i < panelEntries.Length; i++)
        {
            panelEntries[i].panel.SetActive(false);
        }
    }
}
