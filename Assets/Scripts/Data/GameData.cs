using System;

[Serializable]
public class GameData
{
    public int Level;

    public int FiftyLifeline;

    public int SwitchLifeline;

    public int AudienceLifeline;


    public int Score;

    public GameData ()
    {
        this.Level = -2;
        this.FiftyLifeline = 0;
        this.SwitchLifeline = 0;
        this.AudienceLifeline = 0;
        this.Score = 0;
    }
}
