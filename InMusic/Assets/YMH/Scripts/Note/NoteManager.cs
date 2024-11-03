using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Note
{
    public int bar;
    public int channel;
    public List<int> noteData;

    public override string ToString()
    {
        return string.Format("Bar: {0}, Channel: {1}, NoteData: {2}", bar, channel, noteData);
    }
}

public class NoteManager : MonoBehaviour
{
}