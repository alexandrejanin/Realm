﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

public class DialogueManager : MonoBehaviour {
	[SerializeField] private GameObject textPanel;
	[SerializeField] private Text textText, speakerText;

	private readonly Queue<Sentence> textQueue = new();

	public void EnqueueSentence(Sentence sentence) {
		textQueue.Enqueue(sentence);
		if (textQueue.Count == 1) {
			DisplayNextSentence();
		}
	}

	private void DisplayNextSentence() {
		if (textQueue.Count > 0) {
			textPanel.SetActive(true);
			var sentence = textQueue.Dequeue();
			speakerText.text = sentence.speaker;
			textText.text = sentence.text;
		} else {
			textPanel.SetActive(false);
		}
	}
}

public struct Sentence {
	public readonly string speaker, text;

	public Sentence(string speaker, string text) {
		this.speaker = speaker;
		this.text = text;
	}
}