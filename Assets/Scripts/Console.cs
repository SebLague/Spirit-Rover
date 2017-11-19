using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class Console : MonoBehaviour {

	const string decorativeChars = ":();";
	const string legalChars = "abcdefghijklmnopqrstuvwxyz1234567890 .," + decorativeChars;

    public int maxNumLinesOnScreen = 10;
    public int charLimit = 10;
    public float lineSpacing = 2;
    public Text textTemplate;
	public Image caret;
	public Image highlightLine;

    public float blinkTime = .2f;
    public float blinkDelay = 1f;
	public Transform helpMenu;
	public Transform map;

    float lastBlinkTime;
    float lastKeyTime;

    [HideInInspector]
    public string testString;
    [HideInInspector]
	[SerializeField]
	Text[] textFields;

    List<string> lines = new List<string>();
    int selectedLineIndex;
    int firstDisplayedLineIndex;
    int caretCharIndex;

    void Awake()
    {
        GenerateTextFields();
        ClearTextFields();
        lines.Add("");


    }

    void Start()
    {
        CustomInput.instance.RegisterKey(KeyCode.Backspace);
        CustomInput.instance.RegisterKey(KeyCode.LeftArrow);
        CustomInput.instance.RegisterKey(KeyCode.RightArrow);
        CustomInput.instance.RegisterKey(KeyCode.UpArrow);
        CustomInput.instance.RegisterKey(KeyCode.DownArrow);
		Rover.OnCommandRunForFirstTime += CommandRunOnRover;
		Rover.OnCommandsFinished += CommandsFinished;
		highlightLine.gameObject.SetActive (false);
    }

	void CommandsFinished() {
		highlightLine.gameObject.SetActive (false);
	}

	void CommandRunOnRover(Command command) {
		highlightLine.gameObject.SetActive (true);
		float y = textFields [command.lineIndex].rectTransform.position.y;
		highlightLine.rectTransform.position = new Vector3 (highlightLine.rectTransform.position.x, y, highlightLine.rectTransform.position.z);
	}

    void Update () {
       
        // text input
        string input = Input.inputString;
        foreach (char c in input)
        {
            if (lines[selectedLineIndex].Length >= charLimit)
            {
                break;
            }
            if (legalChars.Contains(c.ToString().ToLower()))
            {
                if (caretCharIndex < lines[selectedLineIndex].Length)
                {
                    lines[selectedLineIndex] = lines[selectedLineIndex].Insert(caretCharIndex, c.ToString());
                }
                else
                {
                    lines[selectedLineIndex] += c;
                }
                caretCharIndex++;
                lastKeyTime = Time.time;
            }
        }

        HandleControlInput();

        UpdateDisplay();

	}

    void Upload()
    {
		List<string> simplifiedCommands = lines.Select (x => SimplifyStringToEssentials (x)).ToList ();

        FindObjectOfType<LevelSpawner>().ResetLevel();
		Command[] commands = Command.CommandsFromLines(simplifiedCommands);
        Rover rover = FindObjectOfType<Rover>();
        rover.SetCommands(commands);
    }

	string SimplifyStringToEssentials(string s) {
		s = s.Trim ().ToLower ();
		foreach (char c in decorativeChars) {
			if (s.Contains(c)) {
				s = s.Replace(c+"","");
			}
		}
		return s;
	}

    bool OnTextEntered(int lineIndex)
    {
		string lineText = SimplifyStringToEssentials (lines [lineIndex]);
		if (lineText.Length > 0) {
			if (lineText.Contains("upload") || lineText[lineText.Length-1] == 'u')
	        {
	            lines[lineIndex] = lines[lineIndex].Replace("upload", "");
				lines[lineIndex] = lines[lineIndex].Replace("u", "");
	            Upload();
	            return true;
	        }
			if (lineText.Contains ("help") || lineText[lineText.Length-1] == 'h') {
				lines[lineIndex] = lines[lineIndex].Replace("help", "");
				lines[lineIndex] = lines[lineIndex].Replace("h", "");
				helpMenu.gameObject.SetActive (!helpMenu.gameObject.activeSelf);
				return true;
			}
			if (lineText.Contains ("map") || lineText[lineText.Length-1] == 'm') {
				lines[lineIndex] = lines[lineIndex].Replace("map", "");
				lines[lineIndex] = lines[lineIndex].Replace("m", "");
				map.gameObject.SetActive (!map.gameObject.activeSelf);
				return true;
			}
		}

        return false;
    }

    void HandleControlInput()
    {
		bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // New line
		if (Input.GetKeyDown(KeyCode.Return))
		{
            bool specialCommandEntered = OnTextEntered(selectedLineIndex);
            if (specialCommandEntered)
            {
                caretCharIndex = lines[selectedLineIndex].Length;
            }
            else
            {
                lastKeyTime = Time.time;
				if (lines.Count < maxNumLinesOnScreen) {
					if (selectedLineIndex < lines.Count - 1) {
						lines.Insert (selectedLineIndex + 1, "");
					} else {
						lines.Add ("");
					}
					selectedLineIndex++;
					caretCharIndex = 0;
				} else {
					selectedLineIndex++;
					selectedLineIndex = Mathf.Clamp (selectedLineIndex, 0, maxNumLinesOnScreen - 1);
					caretCharIndex = lines [selectedLineIndex].Length;
				}
            }
		}

        // Backspace
        if (CustomInput.instance.GetKeyPress(KeyCode.Backspace))
        {
			lastKeyTime = Time.time;
            if (lines[selectedLineIndex].Length == 0)
            {
                if (lines.Count != 1)
                {
                    lines.RemoveAt(selectedLineIndex);
                    if (selectedLineIndex > 0)
                    {
                        selectedLineIndex--;
                        caretCharIndex = lines[selectedLineIndex].Length;
                    }
                }
            }
            else
            {
                if (caretCharIndex > 0)
                {
                    caretCharIndex--;
                    lines[selectedLineIndex] = lines[selectedLineIndex].Remove(caretCharIndex, 1);
                }
            }
        }

        // Arrow keys
		if (CustomInput.instance.GetKeyPress(KeyCode.UpArrow))
		{
			lastKeyTime = Time.time;
            if (shift)
            {
                selectedLineIndex = 0;
            }
            else
            {
                selectedLineIndex = Mathf.Clamp(selectedLineIndex - 1, 0, int.MaxValue);
            }
            caretCharIndex = lines[selectedLineIndex].Length;
		}
		if (CustomInput.instance.GetKeyPress(KeyCode.DownArrow))
		{
			lastKeyTime = Time.time;
            if (shift && lines.Count>0)
			{
                selectedLineIndex = lines.Count-1;
			}
			else
			{
				selectedLineIndex = Mathf.Clamp(selectedLineIndex + 1, 0, lines.Count - 1);
			}
           
            caretCharIndex = lines[selectedLineIndex].Length;
		}


        if (CustomInput.instance.GetKeyPress(KeyCode.LeftArrow))
		{
			lastKeyTime = Time.time;
            if (shift)
            {
                caretCharIndex = 0;
            }
            else
            {
                caretCharIndex = Mathf.Clamp(caretCharIndex - 1, 0, int.MaxValue);
            }
		}
        if (CustomInput.instance.GetKeyPress(KeyCode.RightArrow))
		{
			lastKeyTime = Time.time;
            if (shift)
            {
                caretCharIndex = lines[selectedLineIndex].Length;
            }
            else
            {
                caretCharIndex = Mathf.Clamp(caretCharIndex + 1, 0, Mathf.Max(0, lines[selectedLineIndex].Length));
            }
		}
    }

    void UpdateDisplay()
    {
        if (selectedLineIndex < firstDisplayedLineIndex)
        {
            firstDisplayedLineIndex = selectedLineIndex;
        }
        if (selectedLineIndex >= firstDisplayedLineIndex + maxNumLinesOnScreen)
        {
            firstDisplayedLineIndex = selectedLineIndex - maxNumLinesOnScreen+1;
        }

        for (int i = 0; i < textFields.Length; i++)
        {
            if (firstDisplayedLineIndex + i > lines.Count-1)
            {
                break;
            }
            textFields[i].text = lines[firstDisplayedLineIndex + i];
        }

		// Draw caret
		bool caretVisible = false;
        if (Time.time - lastKeyTime > blinkDelay)
        {
            caretVisible = (int)((Time.time - (lastKeyTime+blinkDelay))/blinkTime) % 2 == 0;
        }
        else
        {
            caretVisible = true;
        }

        if (caretVisible)
        {
            caret.enabled = true;
            Text selectedField = textFields[selectedLineIndex - firstDisplayedLineIndex];
            selectedField.font.RequestCharactersInTexture(selectedField.text, selectedField.fontSize, selectedField.fontStyle);

            float caretOffsetX = 0;


            for (int i = 0; i < caretCharIndex; i++)
            {
                CharacterInfo info;
                selectedField.font.GetCharacterInfo(selectedField.text[i], out info, selectedField.fontSize, selectedField.fontStyle);
                caretOffsetX += info.advance;
            }

            caret.rectTransform.position = selectedField.rectTransform.position;
            caret.rectTransform.localPosition += Vector3.right * (caretOffsetX + caret.rectTransform.rect.width / 2f);
        }
        else
        {
            caret.enabled = false;
        }
    }

    public void GenerateTextFields()
    {

        DeleteTextFields();

        textFields = new Text[maxNumLinesOnScreen];
        textTemplate.enabled = true;
        for (int i = 0; i < maxNumLinesOnScreen; i++)
        {
            Text t = Instantiate(textTemplate,textTemplate.transform.parent);
            t.rectTransform.position = textTemplate.rectTransform.position + Vector3.down * i * lineSpacing;
            t.text = testString;
            t.name = "Text field " + i;
            textFields[i] = t;
        }
        textTemplate.enabled = false;
    }

    public void DeleteTextFields()
    {
		if (textFields != null)
		{
			for (int i = 0; i < textFields.Length; i++)
			{
                if (textFields[i] != null)
                {
                    DestroyImmediate(textFields[i].gameObject);
                }
			}
		}

        textTemplate.enabled = true;
    }

    void ClearTextFields()
    {
        if (textFields != null)
        {
            for (int i = 0; i < textFields.Length; i++)
            {
                if (textFields[i] != null)
                {
                    textFields[i].text = "";
                }
            }
        }
    }
}
