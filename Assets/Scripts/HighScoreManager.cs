using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class HighScoreManager : MonoBehaviour
{
    private static List<HighScore> highScores;

    public string fileName;

    private const int MAX_SCORES = 10;
    private SaveFile sf;

    // Start is called before the first frame update
    void Start()
    {
        sf = new SaveFile(fileName);
        //ResetHighScores();
        highScores = sf.LoadHighScores();
    }

    private void ResetHighScores()
    {
        highScores = new List<HighScore>();

        // create an empty high scores table
        for (int i = 0; i < MAX_SCORES; i++)
        {
            AddNewHighScore("No Name", 0, new DateTime(2020, 7, 9));
        }
    }

    // check if the score made is in the top 10
    public bool IsHighScore(int score)
    {
        foreach (HighScore h in highScores)
        {
            if (score > h.Score) return true;
        }

        return false;
    }

    public String GetNames()
    {
        String text = "";

        foreach (HighScore h in highScores)
        {
            text += h.Name + "\n";
        }

        return text;
    }

    public String GetScores()
    {
        String text = "";

        foreach (HighScore h in highScores)
        {
            text += h.Score + "\n";
        }

        return text;
    }

    public String GetDates()
    {
        String text = "";

        foreach (HighScore h in highScores)
        {
            text += h.Date.ToString("MM/dd/yyyy") + "\n";
        }

        return text;
    }

    private List<HighScore> GetSortedHighScores()
    {
        return highScores.OrderByDescending(h => h.Score).ToList();
    }

    public void AddNewHighScore(String name, int score, DateTime date)
    {
        if (highScores.Count >= MAX_SCORES) // if the high score list is full
            highScores.RemoveAt(highScores.Count - 1); // remove the lowest high score

        highScores.Add(new HighScore(name, score, date)); // add the new high score

        highScores = GetSortedHighScores(); // sort the high scores into order

        sf.SaveHighScores(highScores); // saves the highscores to file
    }

}

[System.Serializable]
public struct HighScore
{
    private string name;
    private int score;
    private DateTime date;

    public HighScore(string name, int score, DateTime date)
    {
        this.name = name;
        this.score = score;
        this.date = date;
    }

    public string Name
    {
        get { return name; }
    }

    public int Score
    {
        get { return score; }
    }

    public DateTime Date
    {
        get { return date; }
    }

}

[System.Serializable]
public class SaveFile
{
    private String filename;

    public SaveFile(String filename)
    {
        this.filename = filename;
    }

    public void SaveHighScores(List<HighScore> highScores)
    {
        //Create a new data container
        //SaveFile sf = new SaveFile();

        // add the high scores to the file
        //foreach (HighScore score in HighScoreManager.getSortedHighScores())
        //sf.Add(score);

        //Create a file (if one doesn't exist) to store the save data
        BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Create(Application.persistentDataPath + filename);
        FileStream file = File.Create(filename);

        //Write the data container to the actual file on the hard drive
        bf.Serialize(file, highScores);

        //Close the file
        file.Close();

        //Let everyone know!
        //Debug.Log("Save HighScore: " + Application.persistentDataPath + filename);
        Debug.Log("High Score saved to " + filename);
    }

    public List<HighScore> LoadHighScores()
    {
        //Make sure there is a save file
        if (!File.Exists(filename))
        {
            Debug.LogError("No highscores file!");
            return null;
        }

        //Open the save file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(filename, FileMode.Open);

        //Read the data container from the save file
        List<HighScore> highScores = (List<HighScore>)bf.Deserialize(file);

        //Close the file
        file.Close();

        //Let everyone know!
        Debug.Log("Highscores loaded from: " + filename);

        return highScores;
    }

}