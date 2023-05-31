using System.Collections.Generic;

public class MapData : IData
{
    readonly List<string> audio_sh1_list = new List<string>();
    readonly List<Question> question_list = new List<Question>();
    public int TotalStep { get; private set; } = 5;
    public int MaxPlayCnt { get; private set; } = 2;

    public List<Question> GetMapItems() 
    {
        return question_list;
    }

    public void Init()
    {
        GameData data = Game.Data.JSData.Data;

        if (Game.Data.JSData.Data == null) return;
        List<Activity_Stimulus> stimulus = data.Activity.Stimulus;
        for (int i = 0; i < stimulus.Count; i++)
        {
            Activity_Stimulus _stimulus = stimulus[i];
            audio_sh1_list.Add(_stimulus.Body.item.audio.sha1);
        }

        List<Activity_Questions> questions = data.Activity.Questions;
        for (int i = 0; i < questions.Count; i++)
        {
            Question q = new Question(questions[i]);
            question_list.Add(q);
        }
    }

    public void Dispose()
    {
        audio_sh1_list.Clear();
        question_list.Clear();
    }
}

public struct Table
{
    public int Row;
    public int Column;

    public Table(int r, int c) 
    {
        Row = r;
        Column = c;
    }
}

public struct QuestionItem 
{
    public Table Index;
    public string Name;

    public QuestionItem(string name, int row, int col) 
    {
        Name = name;
        Index = new Table(row, col);
    }
}

public struct Question
{
    public List<QuestionItem> item_list;
    public List<List<int>> Result;
    public List<Table> Tables;

    public Question(Activity_Questions question)
    {
        Result = new List<List<int>>();
        item_list = new List<QuestionItem>();
        Tables = new List<Table>();

        Questions_Body body = question.Body;

        for (int i = 0; i < body.options.Count; i++)
        {
            Questions_Options option = body.options[i];
            item_list.Add(new QuestionItem(option.image.sha1, option.rowIndex, option.colIndex));
        }

        List<List<string>> answers = body.answers;
        for (int i = 0; i < answers.Count; i++)
        {
            List<string> answer = answers[i];
            List<int> l = new List<int>();
            for (int j = 0; j < answer.Count; j++)
            {
                l.Add(int.Parse(answer[j]));
            }
            Result.Add(l);
        }

        for (int i = 0; i < body.tags.Count; i++)
        {
            var t = body.tags[i];
            Tables.Add(new Table(t.rows, t.cols));
        }
    }
}