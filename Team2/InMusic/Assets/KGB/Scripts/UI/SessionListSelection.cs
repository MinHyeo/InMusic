using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SessionListSelection
{
    private List<SessionListEntry> entries = new List<SessionListEntry>();
    private int selectedIndex = -1;

    private Transform scrollContentParent;
    private ScrollRect scrollRect;

    public SessionListSelection(Transform scrollContentParent)
    {
        this.scrollContentParent = scrollContentParent;
        scrollRect = scrollContentParent.GetComponentInParent<ScrollRect>();
    }

    public void SetEntries(List<SessionListEntry> newEntries)
    {
        entries = newEntries;
        selectedIndex = entries.Count > 0 ? 0 : -1;
        UpdateSelectionUI();
        ScrollToSelected();
    }

    public void ClearEntries()
    {
        entries.Clear();
        selectedIndex = -1;
    }

    public void SelectEntry(SessionListEntry entry)
    {
        int index = entries.IndexOf(entry);
        if (index >= 0)
        {
            selectedIndex = index;
            UpdateSelectionUI();
            ScrollToSelected();
        }
    }

    public void MoveSelectionUp()
    {
        if (entries.Count == 0) return;
        selectedIndex = (selectedIndex - 1 + entries.Count) % entries.Count;
        UpdateSelectionUI();
        ScrollToSelected();
    }

    public void MoveSelectionDown()
    {
        if (entries.Count == 0) return;
        selectedIndex = (selectedIndex + 1) % entries.Count;
        UpdateSelectionUI();
        ScrollToSelected();
    }

    public SessionListEntry GetSelectedEntry()
    {
        if (selectedIndex < 0 || selectedIndex >= entries.Count) return null;
        return entries[selectedIndex];
    }

    public void UpdateSelectionUI()
    {
        for (int i = 0; i < entries.Count; i++)
        {
            entries[i].SetSelected(i == selectedIndex);
        }
    }

    private void ScrollToSelected()
    {
        if (selectedIndex < 0 || selectedIndex >= entries.Count || scrollRect == null)
            return;

        RectTransform selectedRect = entries[selectedIndex].GetComponent<RectTransform>();
        RectTransform contentRect = scrollContentParent.GetComponent<RectTransform>();

        Vector3[] contentCorners = new Vector3[4];
        contentRect.GetWorldCorners(contentCorners);

        Vector3[] itemCorners = new Vector3[4];
        selectedRect.GetWorldCorners(itemCorners);

        float contentHeight = contentCorners[1].y - contentCorners[0].y;
        float itemPosY = itemCorners[0].y - contentCorners[0].y;

        float normalizedPosition = 1 - (itemPosY / contentHeight);

        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(normalizedPosition);
    }
}



