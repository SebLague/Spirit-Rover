using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command {

    public enum CommandType { Ignore, Null, Accelerate, Brake, Left, Right, Wait };
    public readonly CommandType commandType;
    public readonly float duration;
    public readonly int lineIndex;

    public Command(CommandType commandType, float duration, int lineIndex)
    {
        this.commandType = commandType;
        this.duration = duration;
        this.lineIndex = lineIndex;
    }

    public void Log()
    {
        Debug.Log("Command: " +commandType + " Duration: " + duration);
    }

    public static Command[] CommandsFromLines(List<string> lines)
    {
        Command[] commands = new Command[lines.Count];
        for (int i = 0; i < lines.Count; i++)
        {
            commands[i] = CommandFromText(lines[i], i);
        }
        return commands;
    }

    public static Command CommandFromText(string text, int lineIndex)
    {
		CommandType t = CommandType.Null; // rover ignores strings that aren't intended as commands; e.g. empty line
		if (string.IsNullOrEmpty (text)) {
			t = CommandType.Ignore;
		}
		else {
	        if (text.Length > 0)
	        {
	            switch (text[0])
	            {
	                case 'a':
	                    t = CommandType.Accelerate;
	                    break;
	                case 'b':
	                    t = CommandType.Brake;
	                    break;
	                case 'l':
	                    t = CommandType.Left;
	                    break;
	                case 'r':
	                    t = CommandType.Right;
	                    break;
	                case 'w':
	                    t = CommandType.Wait;
	                    break;
	            }
	        }
	       
	    }
		float time = ExtractNumberFromText(text);
		return new Command(t, time, lineIndex);
	}
    static float ExtractNumberFromText(string text)
    {
        string legalChars = "1234567890.,";
        List<string> legalStrings = new List<string>();
        legalStrings.Add("");
        foreach (char c in text)
        {
            if (legalChars.Contains(c.ToString()))
            {
                legalStrings[legalStrings.Count - 1] += c;
            }
            else
            {
                legalStrings.Add("");
            }
        }

        //legalStrings.Sort((x, y) => y.Length.CompareTo(x.Length));
        foreach (string timeString in legalStrings)
        {
            float time = 0;
            string formattedTimeString = timeString.Replace(',', '.');
            if (float.TryParse(formattedTimeString, out time))
            {
                return time;
            }
        }
  
        return 0;
    }
}
