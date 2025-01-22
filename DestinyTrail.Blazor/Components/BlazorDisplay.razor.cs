using System;
using DestinyTrail.Engine;

namespace DestinyTrail.Blazor.Components;

public partial class BlazorDisplay : IDisplay
{
    public string outputText = "";

    public List<string> Items { get;set; } = new();

    public void Write(string message)
    {
        outputText += message;
    }

    public void WriteTitle(string message)
    {
        outputText += "TODO: BUILD A TITLE DISPLAY FOR THIS...<br/>" + message;
    }

    public void Clear()
    {
        outputText = "";
    }


    public void ScrollToBottom()
    {
        // TODO;
    }

    public void WriteError(string message)
    {
        outputText += $"<span class='error'>{message}</span>";
    }
}
