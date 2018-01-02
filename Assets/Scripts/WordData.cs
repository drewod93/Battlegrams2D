using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	things to refactor:
		-is stringlegality the "best" way to pass things around? STRUCT
		-system.text.stringbuilder
		-the character difference code exists as a method and in islegal
		-should I be using so many char[]? apparently lists are the way to go in c# 
		-get rid of bullshit test code
		-printing lists?

*/


public class WordData : MonoBehaviour {

	//associates anagram strings to lists of words that make those anagrams
	public Dictionary<string, List<string>> anagrams;
	//hashset for looking up if a word is tournament legal
	public HashSet<string> allWordsHash;
	public string[] allWords;

	public GameObject guessedWordsGO;
	public GameObject looseLettersGO;
	private List<string> guessedWords;
	private List<char> looseLetters;

	void Awake(){
		LoadWords ();
	}
	// Use this for initialization
	void Start () {
		guessedWords = new List<string>();
		looseLetters = new List<char>(); 
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void LoadWords(){
		TextAsset twl = Resources.Load ("twl") as TextAsset;
		if (twl == null){
			Debug.Log ("failed to load text");
		}
		else {
			allWords = twl.text.Split (new char[] { '\n' });
			anagramize (allWords);
			allWordsHash = new HashSet<string> (allWords);
		}
	}

	public string isLegal (string word){
		//check that it's a legal scrabble word
		if (!allWordsHash.Contains(word)){
			Debug.Log ("Not a legal Scrabble Word");
			return null;
		}

		//check if it's a combination of loose letters
		char[] looseLettersArray = looseLetters.ToArray ();
		if (isSubsetArray(word.ToCharArray(), looseLettersArray)){
			return "";
		}

		//check if it's a legal combination of loose letters and combined words
		for (int i = 0; i < guessedWords.Count; i++){
			string comboString = new string (looseLettersArray) + guessedWords [i];
			//Debug.Log (comboString);
			if (isSubsetArray(word.ToCharArray(), comboString.ToCharArray())){
				return guessedWords[i];
			}
		}

		Debug.Log (word + " is missing letters!");
		return null;
	}

	bool isSubsetArray (char[] sub, char[] super){
		int[] subLetterCounts = new int[26];
		int[] superLetterCounts = new int[26];

		for (int i = 0; i< sub.Length; i++){
			subLetterCounts [(int)sub[i] - (int)'a']++;
		}
		for (int i = 0; i< super.Length; i++){
			superLetterCounts [(int)super[i] - (int)'a']++;
		}

		/*char alpha = 'a';
		for (int i = 0; i < 26; i++){
			Debug.Log (alpha.ToString() + ": " + subLetterCounts [i].ToString () + " vs " + superLetterCounts [i].ToString ());
			alpha++;
		}*/

		for (int i = 0; i< 26; i++){
			if (subLetterCounts[i] > superLetterCounts[i]){
				char culprit = (char)((int)'a' + i);
				//Debug.Log (new string(sub) + " has an invalid " + culprit);
				return false;
			}
		}
		return true;
	}

	//fcreate a dictionary mapping anagrams to legal words (for use by AI)
	void anagramize (string[] words){

		/*int maxAnagrams = 0;
		List<string> maxAnagramsList = new List<string>{ };*/
		//string maxAnagramsString;

		anagrams = new Dictionary<string, List<string>> ();
		string sorted = "";
		for (int i = 0; i < words.Length; i++){

			sorted = sortWord (words [i]);
			if (anagrams.ContainsKey(sorted)){
				anagrams [sorted].Add (words [i]);

				/*if (anagrams[sorted].Count > maxAnagrams){
					maxAnagrams = anagrams [sorted].Count;
					maxAnagramsList = anagrams [sorted];
					//maxAnagramsString = sorted;
				}*/
			}
			else {
				anagrams.Add (sorted, new List<string> { words [i] });
			}
		}

		/*
		Debug.Log ("maxanagrams is: " + maxAnagrams.ToString ());
		//Debug.Log ("the winning combination is: " + maxAnagramsString);
		string allAnagrams = "";
		for (int i = 0; i< maxAnagramsList.Count; i++){
			Debug.Log (maxAnagramsList [i]);
		}*/

	}

	string sortWord (string orig){
		char[] sortedArray = orig.ToCharArray ();
		System.Array.Sort (sortedArray);
		return new string (sortedArray);
	}
	char[] arrayDif (char[] existingWord, char[] newWord){
		int[] existingWordLetters = new int[26];
		int[] newWordLetters = new int[26];
		string difString = "";

		for (int i = 0; i < existingWord.Length; i++){
			existingWordLetters [(int)existingWord [i] - (int)'a']++;
		}
		for (int i = 0; i < newWord.Length; i++){
			newWordLetters [(int)newWord [i] - (int)'a']++;
		}
		for (int i = 0; i < 26; i++){
			for (int j = 0; j < newWordLetters[i] - existingWordLetters[i]; j++){
				difString += (char)(i + (int)'a');
			}
		}
		//Debug.Log (difString);
		return difString.ToCharArray();
	}

	public void suggestWord(){
		//create a list of possible strings
		List<string> combos = new List<string>();
		List<string> looseCombos = letterCombos (looseLetters);
		for (int i = 0; i < guessedWords.Count; i++){
			for (int j = 0; j < looseCombos.Count; j++){
				string newEntry = sortWord (guessedWords [i] + looseCombos [j]);
				if (!combos.Contains(newEntry)){
					combos.Add (newEntry);
				}
			}
		}
		for (int i = 0; i < looseCombos.Count; i++){
			combos.Add (looseCombos [i]);
		}
		for (int i = 0; i < combos.Count; i++){
			if (anagrams.ContainsKey(combos[i])){
				List<string> dicEntry = anagrams [combos [i]];
				Debug.Log ("possible word found: " + dicEntry[0]);
				break;
			}
		}
		Debug.Log ("no possible words");
	}
	/*'a, b, c'
	returnString: c

	*/

	List<string> letterCombos(List<char> letterList){
		List<string> returnList = new List<string> ();
		letterCombosHelper (returnList, letterList);
		return returnList;
	}
	void letterCombosHelper (List<string> returnList, List<char> letterList)
	{
		if (letterList.Count == 1) {
			returnList.Add (letterList [0].ToString ());
		} else {
			List<char> newLetterList = new List<char> (letterList);
			char first = letterList [0];
			newLetterList.Remove (first);
			returnList.Add (first.ToString ());
			letterCombosHelper (returnList, newLetterList);
			int length = returnList.Count;
			for (int i = 0; i < length; i++){
				string newEntry = sortWord (first.ToString () + returnList [i]);
				if (!returnList.Contains (newEntry)){
					returnList.Add (newEntry);
				}
			}
		}
	}

	void printList(List<char> charList){
		string printString = "";
		for (int i = 0; i < charList.Count; i++){
			printString += " " + charList [i];
		}
		Debug.Log (printString);
	}
	void printList(List<string> strList){
		string printString = "";
		for (int i = 0; i < strList.Count; i++){
			printString += " " + strList [i];
		}
		Debug.Log (printString);
	}

	//stringlegality will either be null (invalid), "" (all from loose), or the word to combine with
	public void addWord (string word){
		string stringLegality = isLegal (word);
		if (stringLegality != null){

			//comprised of entirely loose letters
			if (stringLegality == ""){
				Debug.Log ("Adding " + word + " entirely from loose letters!");
				for (int i = 0; i < word.Length; i++){
					looseLetters.Remove (word [i]);
				}
			}
			//combined with an existing word
			else {
				char[] charDif = arrayDif (stringLegality.ToCharArray (), word.ToCharArray ());
				Debug.Log ("Combining " + stringLegality + " and " + new string (charDif) + "!");
				for (int i = 0; i < charDif.Length; i++){
					looseLetters.Remove (charDif [i]);
				}
				guessedWords.Remove (stringLegality);
			}

			guessedWords.Add (word);
			guessedWordsGO.GetComponent<UnityEngine.UI.Text> ().text = string.Join("\r\n", guessedWords.ToArray());
			looseLettersGO.GetComponent<UnityEngine.UI.Text> ().text = new string(looseLetters.ToArray());
		}
		else {
			Debug.Log (word + " is invalid");
		}
	}
	public void addLetter (char c){
		looseLetters.Add (c);
		looseLettersGO.GetComponent<UnityEngine.UI.Text> ().text = new string(looseLetters.ToArray());
	}
	public void removeLetter (char c){
		looseLetters.Remove (c);
		looseLettersGO.GetComponent<UnityEngine.UI.Text> ().text = new string(looseLetters.ToArray());
	}

}
