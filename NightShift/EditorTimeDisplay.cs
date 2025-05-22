using System;
using UnityEngine;
using UnityEngine.UI;

namespace NightShift;

public class EditorTimeDisplay
{
    private const string TopBarPath = "_UIMaster/MainCanvas/Editor/Top Bar";
    private const string SearchBarPath = TopBarPath + "/Search/SearchInput";
    private const string SteamButtonPath = TopBarPath + "/ButtonSteam";
    private const string ShipNamePath = TopBarPath + "/ShipName/ShipName/Text Area/Text";
    
    private const string EditorClock = "NightShiftEditorClock";
    private const string EditorClockText = "Text";
    
    private readonly KSPUtil.DefaultDateTimeFormatter _dateTimeFormatter;
    private readonly TMPro.TextMeshProUGUI _text;
    
    public EditorTimeDisplay()
    {
        GameObject topBar = GameObject.Find(TopBarPath);

        GameObject editorClock = GameObject.Find(TopBarPath + "/" + EditorClock);
        if (!editorClock)
        {
            editorClock = new(EditorClock)
            {
                layer = LayerMask.NameToLayer("UI")
            };
        }
        
        Image img = editorClock.GetComponent<Image>();
        if (!img)
        {
            img = editorClock.AddComponent<Image>();
        }
        
        CanvasRenderer cr = editorClock.GetComponent<CanvasRenderer>();
        if (!cr)
        {
            editorClock.AddComponent<CanvasRenderer>();
        }
        
        RectTransform rect = editorClock.GetComponent<RectTransform>();
        if (!rect)
        {
            rect = editorClock.AddComponent<RectTransform>();
        }
        rect.SetParent(topBar.transform);

        GameObject steamBtn = GameObject.Find(SteamButtonPath);
        rect.anchoredPosition = new(steamBtn ? -334 : -294, -12);
        rect.anchorMax = new(1, 1);
        rect.anchorMin = new(1, 1);
        rect.sizeDelta = new(160, 22);
        
        GameObject searchBar = GameObject.Find(SearchBarPath);
        Image searchBarBg = searchBar.GetComponent<Image>();
        var borders = new Vector4(4, 4, 4, 4);
        var sprite = Sprite.Create(
            searchBarBg.mainTexture as Texture2D, 
            new(0,0,16,16), 
            Vector2.one * 0.5f, 
            100, 0, 
            SpriteMeshType.FullRect, 
            borders);
        img.sprite = sprite;
        img.type = Image.Type.Sliced;
        
        GameObject timeText = GameObject.Find(TopBarPath + "/" + EditorClock + "/" + EditorClockText);
        if (!timeText)
        {
            timeText = new(EditorClockText)
            {
                layer = LayerMask.NameToLayer("UI")
            };
        }
        _text = timeText.GetComponent<TMPro.TextMeshProUGUI>();
        if (!_text)
        {
            _text = timeText.AddComponent<TMPro.TextMeshProUGUI>();
        }
        CanvasRenderer crx = timeText.GetComponent<CanvasRenderer>();
        if (!crx)
        {
            timeText.AddComponent<CanvasRenderer>();
        }
        RectTransform rect2 = timeText.GetComponent<RectTransform>();
        if (!rect2)
        {
            rect2 = timeText.AddComponent<RectTransform>();
        }
        rect2.SetParent(editorClock.transform);
        rect2.anchoredPosition = new(0, 0);
        rect2.anchorMax = new(1, 1);
        rect2.anchorMin = new(1, 1);
        
        rect2.sizeDelta = new(200, 22);
        
        GameObject shipNameBtn = GameObject.Find(ShipNamePath);
        if (shipNameBtn)
        {
            var font = shipNameBtn.GetComponent<TMPro.TextMeshProUGUI>()?.font;
            if (font)
            {
                _text.font = font;
            }
        }
        
        _text.fontSize = 16f;
        _text.color = Color.green;
        _text.fontStyle = TMPro.FontStyles.Normal;
        _text.margin = new(2, 2, 2, 2);
        _text.alignment = TMPro.TextAlignmentOptions.Center;
        _text.transform.localPosition = new(0, 0, 0);
        
        _dateTimeFormatter = new();
        
        Update(TimeMode.Pause, 0);
    }

    public void Update(TimeMode? mode, double time)
    {
        int years = _dateTimeFormatter.GetUTYear();
        int days = _dateTimeFormatter.GetUTDay();
        int hours =  _dateTimeFormatter.GetUTHour();
        int minutes =  _dateTimeFormatter.GetUTMinute();
        int seconds = _dateTimeFormatter.GetUTSecond();

        string dateAndTime = $"Y{years:00}, D{days:00} - {hours:00}:{minutes:00}:{seconds:00}";
        string suffix = mode switch
        {
            TimeMode.ReReWind => " <<<< ",
            TimeMode.Rewind => " <<< ",
            TimeMode.Back => " << ",
            TimeMode.Pause => " || ",
            TimeMode.Forward => " >> ",
            TimeMode.FastForward => " >>> ",
            TimeMode.FastFastForward => " >>>> ",
            _ => ""
        };

        int hours2 = (int)(time * 24);
        int minutes2 =(int)(time * 1440) - (hours2 * 60);
        string ampm = time <= 0.5 ? "AM" : "PM";
        int hoursAdjusted = (hours2 + 12) % 12; 

        _text.text = $"{time:0.0000} - {hoursAdjusted:00}:{minutes2:00} {ampm}{suffix}";
    }
}