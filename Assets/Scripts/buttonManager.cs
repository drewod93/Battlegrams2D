using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonManager : MonoBehaviour {

	public InputField inputField;

	public void submit(){
		gameObject.GetComponent<WordData> ().addWord (inputField.text.ToString());
	}

	public void peel(){
		gameObject.GetComponent<WordData> ().addLetter ((char)('a' + Random.Range (0, 25)));
	}

	public void suggest(){
		gameObject.GetComponent<WordData> ().suggestWord ();
	}
}
