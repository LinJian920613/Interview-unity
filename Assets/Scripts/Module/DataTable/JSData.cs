using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JSData : IData
{
    const string JSON_FILE_NAME = "data.json";
    public GameData Data;
    readonly List<GameData> m_datas = new List<GameData>();

    public void Init()
    {
        string path = Application.streamingAssetsPath + "/" + JSON_FILE_NAME;
        string readData = File.ReadAllText(path);
        JsonData jsonData = JsonMapper.ToObject(readData); //将json信息解析成对象
        foreach (JsonData js in jsonData)
        {
            GameData data = new GameData();
            data.Serialize(js);
            m_datas.Add(data);
        }
        Data = m_datas[0];
    }

    public void Dispose()
    {
        Data = null;
    }
}

public interface ISerialize
{
    void Serialize(JsonData jsData);
}

public class GameData : ISerialize
{
    public string ProductName;
    public string BookName;
    public string UnitName;
    public string LessonName;
    public Activity Activity = new Activity();
    public string Owner;
    public string Key;
    public string CreatedStamp;
    public string LastUpdatedStamp;
    public int State;

    public void Serialize(JsonData jsData)
    {
        ProductName = jsData.ToString("ProductName");
        BookName = jsData.ToString("BookName");
        UnitName = jsData.ToString("UnitName");
        LessonName = jsData.ToString("LessonName");
        Activity.Serialize(jsData["Activity"]);
        Owner = jsData.ToString("Owner");
        Key = jsData.ToString("Key");
        CreatedStamp = jsData.ToString("CreatedStamp");
        LastUpdatedStamp = jsData.ToString("LastUpdatedStamp");
        State = jsData.ToInt("State");
    }
}

public class Activity : ISerialize
{
    public List<Activity_Stimulus> Stimulus = new List<Activity_Stimulus>();
    public List<Activity_Questions> Questions = new List<Activity_Questions>();
    public string Title;
    public string Key;
    public Activity_Body Body = new Activity_Body();
    public string Tags;
    public string Type;
    public string activityQuestionMd5;
    public string part;
    public int ContentId;
    public string ContentRevision;

    public void Serialize(JsonData jsData)
    {
        foreach (JsonData js in jsData["Stimulus"])
        {
            Activity_Stimulus stimulus = new Activity_Stimulus();
            stimulus.Serialize(js);
            Stimulus.Add(stimulus);
        }
        foreach (JsonData js in jsData["Questions"])
        {
            Activity_Questions questions = new Activity_Questions();
            questions.Serialize(js);
            Questions.Add(questions);
        }
        Title = jsData.ToString("Title");
        Key = jsData.ToString("Key");
        Body.Serialize(jsData["Body"]);
        Tags = jsData.ToString("Tags");
        Type = jsData.ToString("Type");
        activityQuestionMd5 = jsData.ToString("activityQuestionMd5");
        part = jsData.ToString("part");
        ContentId = jsData.ToInt("ContentId");
        ContentRevision = jsData.ToString("ContentRevision");
    }
}

public class Activity_Body : ISerialize
{
    public List<Activity_Body_Mappings_element> mappings = new List<Activity_Body_Mappings_element>();
    public Activity_Body_Tags tags = new Activity_Body_Tags();

    public void Serialize(JsonData jsData)
    {
        foreach (JsonData js in jsData["mappings"])
        {
            Activity_Body_Mappings_element map_element = new Activity_Body_Mappings_element();
            map_element.Serialize(js);
            mappings.Add(map_element);
        }
        tags.Serialize(jsData["tags"]);
    }
}

public class Activity_Body_Mappings_element : ISerialize
{
    public string s;
    public string q;
    public string anchor;

    public void Serialize(JsonData jsData)
    {
        s = jsData.ToString("s");
        q = jsData.ToString("q");
        anchor = jsData.ToString("anchor");
    }
}

public class Activity_Body_Tags : ISerialize
{
    public List<string> primaryskillset = new List<string>();
    public string skillType;

    public void Serialize(JsonData jsData)
    {
        foreach (JsonData js in jsData["primary-skill-set"])
        {
            primaryskillset.Add(js.ToString());
        }
        skillType = jsData.ToString("skillType");
    }
}

public class Activity_Stimulus : ISerialize
{
    public string Key;
    public Stimulus_Body Body = new Stimulus_Body();
    public string Tags;
    public string Type;
    public string stimulusOfQuestion;
    public double questionAnchor;
    public double answerAnchor;
    public bool isForModeling;

    public void Serialize(JsonData jsData)
    {
        Key = jsData.ToString("Key");
        Body.Serialize(jsData["Body"]);
        Tags = jsData.ToString("Tags");
        Type = jsData.ToString("Type");
        stimulusOfQuestion = jsData.ToString("stimulusOfQuestion");
        questionAnchor = jsData.ToDouble("questionAnchor");
        answerAnchor = jsData.ToDouble("answerAnchor");
        isForModeling = jsData.ToBool("isForModeling");
    }
}

public class Stimulus_Body : ISerialize
{
    public Stimulus_Body_Item item = new Stimulus_Body_Item();
    public string version;
    public string tags;
    public string skillSet;
    public string tests;
    public string options;
    public string layoutMode;
    public string answers;
    public string mode;
    public string background;
    public string asrEngine;
    public int hintTime;

    public void Serialize(JsonData jsData)
    {
        item.Serialize(jsData["item"]);
        version = jsData.ToString("version");
        tags = jsData.ToString("tags");
        skillSet = jsData.ToString("skillSet");
        tests = jsData.ToString("tests");
        options = jsData.ToString("options");
        layoutMode = jsData.ToString("layoutMode");
        answers = jsData.ToString("answers");
        mode = jsData.ToString("mode");
        background = jsData.ToString("background");
        asrEngine = jsData.ToString("asrEngine");
        hintTime = jsData.ToInt("hintTime");
    }
}

public class Stimulus_Body_Item : ISerialize
{
    public string type;
    public string id;
    public string text;
    public string prompt;
    public string hideText;
    public string image;
    public Stimulus_Body_Item_Audio audio = new Stimulus_Body_Item_Audio();
    public string pdf;
    public string video;
    public string audioLocal;
    public string academic;
    public string showMode;
    public string subtitles;
    public int rowIndex;
    public int colIndex;
    public bool lockedPosition;
    public string expected;
    public string speaker;
    public string table;
    public int startRow;
    public int endRow;
    public int startCol;
    public int endCol;
    public string cells;

    public void Serialize(JsonData jsData)
    {
        type = jsData.ToString("type");
        id = jsData.ToString("id");
        text = jsData.ToString("text");
        prompt = jsData.ToString("prompt");
        hideText = jsData.ToString("hideText");
        image = jsData.ToString("image");
        audio.Serialize(jsData["audio"]);
        pdf = jsData.ToString("pdf");
        video = jsData.ToString("video");
        audioLocal = jsData.ToString("audioLocal");
        academic = jsData.ToString("academic");
        showMode = jsData.ToString("showMode");
        subtitles = jsData.ToString("subtitles");
        rowIndex = jsData.ToInt("rowIndex");
        colIndex = jsData.ToInt("colIndex");
        lockedPosition = jsData.ToBool("lockedPosition");
        expected = jsData.ToString("expected");
        speaker = jsData.ToString("speaker");
        table = jsData.ToString("table");
        startRow = jsData.ToInt("startRow");
        endRow = jsData.ToInt("endRow");
        startCol = jsData.ToInt("startCol");
        endCol = jsData.ToInt("endCol");
        cells = jsData.ToString("cells");
    }
}

public class Stimulus_Body_Item_Audio : ISerialize
{
    public string id;
    public string url;
    public int size;
    public string sha1;
    public string mimeType;
    public int width;
    public int height;
    public string language;
    public string title;
    public int duration;
    public string thumbnails;

    public void Serialize(JsonData jsData)
    {
        id = jsData.ToString("id");
        url = jsData.ToString("url");
        size = jsData.ToInt("size");
        sha1 = jsData.ToString("sha1");
        mimeType = jsData.ToString("mimeType");
        width = jsData.ToInt("width");
        height = jsData.ToInt("height");
        language = jsData.ToString("language");
        title = jsData.ToString("title");
        duration = jsData.ToInt("duration");
        thumbnails = jsData.ToString("thumbnails");
    }
}

public class Activity_Questions : ISerialize
{
    public string Key;
    public Questions_Body Body = new Questions_Body();
    public string Tags;
    public string Type;
    public Questions_StimulusOfQuestion stimulusOfQuestion = new Questions_StimulusOfQuestion();
    public double questionAnchor;
    public double answerAnchor;
    public bool isForModeling;

    public void Serialize(JsonData jsData)
    {
        Key = jsData.ToString("Key");
        Body.Serialize(jsData["Body"]);
        Tags = jsData.ToString("Tags");
        Type = jsData.ToString("Type");
        stimulusOfQuestion.Serialize(jsData["stimulusOfQuestion"]);
        questionAnchor = jsData.ToDouble("questionAnchor");
        answerAnchor = jsData.ToDouble("answerAnchor");
        isForModeling = jsData.ToBool("isForModeling");
    }
}

public class Questions_StimulusOfQuestion : ISerialize
{
    public string Key;
    public Questions_StimulusOfQuestion_Body Body = new Questions_StimulusOfQuestion_Body();
    public string Tags;
    public string Type;
    public string stimulusOfQuestion;
    public double questionAnchor;
    public double answerAnchor;
    public bool isForModeling;

    public void Serialize(JsonData jsData)
    {
        Key = jsData.ToString("Key");
        Body.Serialize(jsData["Body"]);
        Tags = jsData.ToString("Tags");
        Type = jsData.ToString("Type");
        stimulusOfQuestion = jsData.ToString("stimulusOfQuestion");
        questionAnchor = jsData.ToDouble("questionAnchor");
        answerAnchor = jsData.ToDouble("answerAnchor");
        isForModeling = jsData.ToBool("isForModeling");
    }
}

public class Questions_StimulusOfQuestion_Body : ISerialize
{
    public Questions_StimulusOfQuestion_Item item = new Questions_StimulusOfQuestion_Item();
    public string version;
    public string tags;
    public string skillSet;
    public string tests;
    public string options;
    public string layoutMode;
    public string answers;
    public string mode;
    public string background;
    public string asrEngine;
    public int hintTime;

    public void Serialize(JsonData jsData)
    {
        item.Serialize(jsData["item"]);
        version = jsData.ToString("version");
        tags = jsData.ToString("tags");
        skillSet = jsData.ToString("skillSet");
        tests = jsData.ToString("tests");
        options = jsData.ToString("options");
        layoutMode = jsData.ToString("layoutMode");
        answers = jsData.ToString("answers");
        mode = jsData.ToString("mode");
        background = jsData.ToString("background");
        asrEngine = jsData.ToString("asrEngine");
        hintTime = jsData.ToInt("hintTime");
    }
}

public class Questions_StimulusOfQuestion_Item : ISerialize
{
    public string type;
    public string id;
    public string text;
    public string prompt;
    public string hideText;
    public string image;
    public Questions_StimulusOfQuestion_Item_Audio audio = new Questions_StimulusOfQuestion_Item_Audio();
    public string pdf;
    public string video;
    public string audioLocal;
    public string academic;
    public string showMode;
    public string subtitles;
    public int rowIndex;
    public int colIndex;
    public bool lockedPosition;
    public string expected;
    public string speaker;
    public string table;
    public int startRow;
    public int endRow;
    public int startCol;
    public int endCol;
    public string cells;

    public void Serialize(JsonData jsData)
    {
        type = jsData.ToString("type");
        id = jsData.ToString("id");
        text = jsData.ToString("text");
        prompt = jsData.ToString("prompt");
        hideText = jsData.ToString("hideText");
        image = jsData.ToString("image");
        audio.Serialize(jsData["audio"]);
        pdf = jsData.ToString("pdf");
        video = jsData.ToString("video");
        audioLocal = jsData.ToString("audioLocal");
        academic = jsData.ToString("academic");
        showMode = jsData.ToString("showMode");
        subtitles = jsData.ToString("subtitles");
        rowIndex = jsData.ToInt("rowIndex");
        colIndex = jsData.ToInt("colIndex");
        lockedPosition = jsData.ToBool("lockedPosition");
        expected = jsData.ToString("expected");
        speaker = jsData.ToString("speaker");
        table = jsData.ToString("table");
        startRow = jsData.ToInt("startRow");
        endRow = jsData.ToInt("endRow");
        startCol = jsData.ToInt("startCol");
        endCol = jsData.ToInt("endCol");
        cells = jsData.ToString("cells");
    }
}

public class Questions_StimulusOfQuestion_Item_Audio : ISerialize
{
    public string id;
    public string url;
    public int size;
    public string sha1;
    public string mimeType;
    public int width;
    public int height;
    public string language;
    public string title;
    public int duration;
    public string thumbnails;

    public void Serialize(JsonData jsData)
    {
        id = jsData.ToString("id");
        url = jsData.ToString("url");
        size = jsData.ToInt("size");
        sha1 = jsData.ToString("sha1");
        mimeType = jsData.ToString("mimeType");
        width = jsData.ToInt("width");
        height = jsData.ToInt("height");
        language = jsData.ToString("language");
        title = jsData.ToString("title");
        duration = jsData.ToInt("duration");
        thumbnails = jsData.ToString("thumbnails");
    }
}

public class Questions_Body : ISerialize
{
    public string item;
    public string version;
    public List<Questions_Body_Tags> tags = new List<Questions_Body_Tags>();
    public string skillSet;
    public List<object> tests;
    public List<Questions_Options> options = new List<Questions_Options>();
    public string layoutMode;
    public List<List<string>> answers = new List<List<string>>();
    public string mode;
    public string background;
    public string asrEngine;
    public int hintTime;

    public void Serialize(JsonData jsData)
    {
        item = jsData.ToString("item");
        version = jsData.ToString("version");
        foreach (JsonData js in jsData["tags"])
        {
            Questions_Body_Tags tag = new Questions_Body_Tags();
            tag.Serialize(js);
            tags.Add(tag);
        }
        skillSet = jsData.ToString("skillSet");
        foreach (JsonData js in jsData["tests"])
        {
            tests.Add(js.ToObject());
        }
        foreach (JsonData js in jsData["options"])
        {
            Questions_Options option = new Questions_Options();
            option.Serialize(js);
            options.Add(option);
        }
        layoutMode = jsData.ToString("layoutMode");
        foreach (JsonData js in jsData["answers"])
        {
            List<string> l = new List<string>();
            foreach (var _js in js) l.Add(_js.ToString());
            answers.Add(l);
        }
        mode = jsData.ToString("mode");
        background = jsData.ToString("background");
        asrEngine = jsData.ToString("asrEngine");
        hintTime = jsData.ToInt("hintTime");
    }
}

public class Questions_Body_Tags : ISerialize
{
    public string compassTags;
    public string subSkillSet;
    public string vocabulary;
    public int rows;
    public int cols;
    public string primaryskillset;

    public void Serialize(JsonData jsData)
    {
        compassTags = jsData.ToString("compassTags");
        subSkillSet = jsData.ToString("subSkillSet");
        vocabulary = jsData.ToString("vocabulary");
        rows = jsData.ToInt("rows");
        cols = jsData.ToInt("cols");
        primaryskillset = jsData.ToString("primary-skill-set");
    }
}

public class Questions_Options : ISerialize
{
    public string type;
    public string id;
    public string text;
    public string prompt;
    public string hideText;
    public Options_Image image = new Options_Image();
    public string audio;
    public string pdf;
    public string video;
    public string audioLocal;
    public string academic;
    public string showMode;
    public string subtitles;
    public int rowIndex;
    public int colIndex;
    public bool lockedPosition;
    public string expected;
    public string speaker;
    public string table;
    public int startRow;
    public int endRow;
    public int startCol;
    public int endCol;
    public string cells;

    public void Serialize(JsonData jsData)
    {
        type = jsData.ToString("type");
        id = jsData.ToString("id");
        text = jsData.ToString("text");
        prompt = jsData.ToString("prompt");
        hideText = jsData.ToString("hideText");
        image.Serialize(jsData["image"]);
        audio = jsData.ToString("audio");
        pdf = jsData.ToString("pdf");
        video = jsData.ToString("video");
        audioLocal = jsData.ToString("audioLocal");
        academic = jsData.ToString("academic");
        showMode = jsData.ToString("showMode");
        subtitles = jsData.ToString("subtitles");
        rowIndex = jsData.ToInt("rowIndex");
        colIndex = jsData.ToInt("colIndex");
        lockedPosition = jsData.ToBool("lockedPosition");
        expected = jsData.ToString("expected");
        speaker = jsData.ToString("speaker");
        table = jsData.ToString("table");
        startRow = jsData.ToInt("startRow");
        endRow = jsData.ToInt("endRow");
        startCol = jsData.ToInt("startCol");
        endCol = jsData.ToInt("endCol");
        cells = jsData.ToString("cells");
    }
}

public class Options_Image : ISerialize
{
    public string id;
    public string url;
    public int size;
    public string sha1;
    public string mimeType;
    public int width;
    public int height;
    public string language;
    public string title;
    public int duration;
    public string thumbnails;

    public void Serialize(JsonData jsData)
    {
        id = jsData.ToString("id");
        url = jsData.ToString("url");
        size = jsData.ToInt("size");
        sha1 = jsData.ToString("sha1");
        mimeType = jsData.ToString("mimeType");
        width = jsData.ToInt("width");
        height = jsData.ToInt("height");
        language = jsData.ToString("language");
        title = jsData.ToString("title");
        duration = jsData.ToInt("duration");
        thumbnails = jsData.ToString("thumbnails");
    }
}