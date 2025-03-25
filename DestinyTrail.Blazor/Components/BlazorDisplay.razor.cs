using System;
using DestinyTrail.Engine;
using DestinyTrail.Engine.Interfaces;
using Microsoft.AspNetCore.Components;

namespace DestinyTrail.Blazor.Components;

public partial class BlazorDisplay : IDisplay
{
    [Parameter]
    public string Heading { get; set; } = "BlazorDisplay";

    [Parameter]
    public string CssClass { get; set; } = "";
    public string outputText = "";

    public List<string> Items { get; set; } = new();

    public async Task Write(string message)
    {
        outputText += $"<div>{message}</div>";
        await InvokeAsync(StateHasChanged);
    }

    public async Task WriteTitle(string message)
    {
        outputText += $"<div class='title'>{message}</div>";
        await InvokeAsync(StateHasChanged);
    }

    public async Task Clear()
    {
        outputText = "";
        await InvokeAsync(StateHasChanged);
    }


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task ScrollToBottom()
    {
        // TODO;
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

    public async Task WriteError(string message)
    {
        outputText += $"<span class='error'>{message}</span>";
        await InvokeAsync(StateHasChanged);
    }
}
