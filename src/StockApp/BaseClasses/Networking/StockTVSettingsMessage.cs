using System.Collections.Generic;

namespace StockApp.BaseClasses
{
    //public class StockTVSettingsMessage
    //{
    //    private string MessagePart;
    //    public StockTVSettingsMessage()
    //    {
    //        this.Settings = new();
    //    }

    //    public StockTVSettingsMessage(string messagePart) : this()
    //    {
    //        this.MessagePart = messagePart;
    //        var parts = messagePart.TrimEnd(';').Split(';');
    //        foreach (var p in parts)
    //        {
    //            var x = p.Split('=');
    //            AddSetting(x[0], x[1]);
    //        }
    //    }

    //    private void AddSetting(string topic, string value)
    //    {
    //        Settings.Add(StockTVSetting.Get(topic, value));
    //    }

    //    public List<StockTVSetting> Settings { get; private set; }

    //    public override string ToString()
    //    {
    //        return MessagePart;
    //    }
    //}


}
