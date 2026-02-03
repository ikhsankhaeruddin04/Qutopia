using System.Collections.Generic;
using System.IO;
using SQLite4Unity3d;
using UnityEngine;
using System.Linq;

public class DatabaseService
{
    // private SQLiteConnection _connection;

    // public DatabaseService(string DatabaseName)
    // {
    //     string dbPath = Path.Combine(Application.streamingAssetsPath, DatabaseName);
    //     _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
    // }

    // public List<QuestionData> GetQuestions(string difficulty, int stage)
    // {
    //     return _connection.Table<QuestionData>()
    //         .Where(q => q.difficulty == difficulty && q.stage == stage)
    //         .ToList();
    // }
}

// [System.Serializable]
// public class QuestionData
// {
//     [PrimaryKey, AutoIncrement]
//     public int id { get; set; }
//     public string question { get; set; }
//     public string optionA { get; set; }
//     public string optionB { get; set; }
//     public string optionC { get; set; }
//     public string optionD { get; set; }
//     public int correctAnswerIndex { get; set; }
//     public string difficulty { get; set; }
//     public int stage { get; set; }
// }
