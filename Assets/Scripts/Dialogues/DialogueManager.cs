using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.ShaderGraph;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Animator animator;

    private PlayerInputs controls;

    private void Awake()
    {
        controls = new();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        animator.enabled = false;
        sentences = new Queue<string>();
    }

    private void Update()
    {
        if (controls.Interactions.ChangeDialogue.triggered)
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        animator.enabled = true;
        animator.SetBool("IsOpen", true);

        nameText.text = dialogue.name;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentense(sentence));
    }
    IEnumerator TypeSentense(string setence)
    {
        dialogueText.text = "";
        foreach (char letter in setence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
        FindObjectOfType<TrainerMovement>().SetCanMove(true);
        //GameManager.instance.SetIsBattle(true);
        GameManager.instance.StartBattle();
    }
}