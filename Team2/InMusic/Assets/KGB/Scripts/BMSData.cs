using System.Collections.Generic;
using UnityEngine;

public class BMSData
{
    public BMSHeader header { get; set; } = new BMSHeader();
    public BMSWAV wavInfo { get; set; } = new BMSWAV();
    public List<BMSNoteData> notes { get; set; } = new List<BMSNoteData>();
}

public class BMSHeader
{
    public int player { get; set; }
    public string genre { get; set; }
    public string title { get; set; }
    public string artist { get; set; }
    public float bpm { get; set; }
    public int playLevel { get; set; }
    public int rank { get; set; }
    public int LNType { get; set; }
}

public class BMSNoteData
{
    public int measure { get; set; }
    public int channel { get; set; }
    public string noteString { get; set; }
}

public class BMSWAV
{
    public string wav;
}