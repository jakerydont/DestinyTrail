using System;
using DestinyTrail.Engine;
using Microsoft.AspNetCore.Components;

namespace DestinyTrail.Blazor.Components;

public partial class BlazorDisplay : IDisplay
{
    [Parameter]
    public string Heading { get; set; } = "BlazorDisplay";

    public string outputText = "";

    public List<string> Items { get; set; } = new();

    public void Write(string message)
    {
        outputText += $"<div>{message}</div>";
        InvokeAsync(StateHasChanged);
    }

    public void WriteTitle(string message)
    {
        outputText += $"<div class='title'>{message}</div>";
        InvokeAsync(StateHasChanged);
    }

    public void Clear()
    {
        outputText = "";
        InvokeAsync(StateHasChanged);
    }


    public void ScrollToBottom()
    {
        // TODO;
    }

    public void WriteError(string message)
    {
        outputText += $"<span class='error'>{message}</span>";
        InvokeAsync(StateHasChanged);
    }
}
