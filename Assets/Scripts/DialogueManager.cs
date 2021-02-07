using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    // Displayed in the UI
    public GameObject DialogueBox;

    public Image portrait;
    public Text nameText;
    public Text dialogueText;

    // Fed into UI
    public Sprite[] portraits;
    private Queue<string> names;
    private Queue<string> sentences;

    public bool isTalking;

    void Awake() {
        instance = this;

        names = new Queue<string>();
        sentences = new Queue<string>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartDialogue(Dialogue dialogue) {
        Cursor.visible = true;
        Time.timeScale = 0;
        isTalking = true;
        DialogueBox.SetActive(true);

        names.Clear();
        sentences.Clear();

        foreach (string name in dialogue.names) {
            names.Enqueue(name);
        }
        foreach (string sentence in dialogue.sentences) {
            sentences.Enqueue(sentence);
        }

        DisplayNextNameAndSentence();
    }

    public void DisplayNextNameAndSentence() {
        if (sentences.Count == 0) {
            EndDialogue();
            return;
        }

        string name = names.Dequeue();
        string sentence = sentences.Dequeue();

        nameText.text = name;
        // dialogueText.text = sentence;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));

        if (name == "Camilla") {
            portrait.sprite = portraits[0];
        } else if (name == "Franz") {
            portrait.sprite = portraits[1];
        } else if (name == "Girl") {
            portrait.sprite = portraits[2];
        } else {
            portrait.sprite = portraits[3];
        }
    }

    IEnumerator TypeSentence(string sentence) {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray()) {
            dialogueText.text += letter;
            yield return null;
        }
    }

    void EndDialogue() {
        Cursor.visible = false;
        Time.timeScale = 1;
        isTalking = false;
        DialogueBox.SetActive(false);
    }
}
