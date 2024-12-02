using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using DestinyTrail.Engine;


namespace DestinyTrail;

public class ADisplay : IDisplay
{



    private ListBox _output { get; set; }
    public ItemCollection AvaloniaItems => _output?.Items ?? throw new NullReferenceException();

    public List<string> Items
    {
        get
        {
            return (List<string>)AvaloniaItems.Cast<string>();
        }
    }



    public ADisplay(ListBox output)
    {
        _output = output;
    }

    public void Write(string message)
    {
        _output.Items.Add(message);
    }

    public void WriteTitle(string message)
    {
        _output.Items.Add("TODO: BUILD A TITLE DISPLAY FOR THIS...\n" + message);
    }

    public void Clear()
    {
        if (_output != null)
        {
            _output.Items.Clear();
        }
        else
        {
            Console.WriteLine("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
        }
    }


    public void ScrollToBottom()
    {
        if (_output != null)
        {
            if (_output.Items.Count > 0)
            {
                var lastItem = _output.Items[_output.Items.Count - 1];

                if (lastItem != null)
                {
                    _output.ScrollIntoView(lastItem);
                }
            }
        }
    }

    public void WriteError(string message)
    {
        _output.Items.Add(message);
    }
}


