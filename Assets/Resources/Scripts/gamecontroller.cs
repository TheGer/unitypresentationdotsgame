using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gamecontroller : MonoBehaviour {

	//squareprefab is the file, square is what is drawn on screen
	GameObject squarePrefabwithNumber,square;

	Text clockText;
	int counter,numbersquares = 0;

	int totalsquares = 25;

	public List<Vector3> positions;

	Vector3 randomPosition;

	bool check,cancreate=false;

	// Use this for initialization
	void Start () {
		
		//get the square prefab
		squarePrefabwithNumber = Resources.Load("Prefabs/Squarewithnumber") as GameObject;
		//clock text
		clockText = GameObject.Find("Clock").GetComponent<Text>();
		
		//add the squarecontroller script to the square generated at the beginning ONLY
		square.AddComponent<squarecontroller>();
		//start the timer
		StartCoroutine(updateClock());

		//create a new list of positions for the placement of the squares
		positions = new List<Vector3>();
		
		//initialize squares
		numbersquares =1;
		StartCoroutine(createSquares(numbersquares));
		
		
	}

	IEnumerator createSquares(int numsquares)
	{
		//modify this code such that blocks CANNOT spawn on top of each other
		for(int i=0;i<numsquares;i++)
		{
			Debug.Log(i);
			do{
				check = false;
				randomPosition = new Vector3(Random.Range(-4.5f,4.5f),Random.Range(-4.5f,4.5f));
				if (positions.Count==0){
					check = true;
				//	positions.Add(randomPosition);
				}
				else {
					cancreate = true;
					foreach (Vector3 position in positions)
					{
						if (Vector3.Distance(randomPosition,position)<1f)
						{
							Debug.Log("too close"+randomPosition);
							cancreate = false;
						//	yield return new WaitForSeconds(0.2f);	
						}
					}
					if (cancreate)
					{
						check = true;

					}
				}
					

			}while(!check);
			
			createSquareWithNumber(randomPosition);
			positions.Add(randomPosition);
			yield return null;
		}
	}

	IEnumerator updateClock()
	{
		float timer = 0f;
		while(true)
		{
			timer++;
			string minutes = Mathf.Floor(timer/60f).ToString("00");
			string seconds = Mathf.Floor(timer%60f).ToString("00");
			clockText.text = minutes +":"+seconds;
			yield return new WaitForSeconds(1f);
		}
	}

	
	public GameObject createSquareWithNumber(Vector3 position)
	{
		counter++;
		square = Instantiate(squarePrefabwithNumber,position,Quaternion.identity);
		square.GetComponentInChildren<Text>().text = counter.ToString();
		return square;
	}

	public void resetScene()
	{
			counter=0;
			positions = new List<Vector3>();
			foreach(GameObject line in GameObject.FindGameObjectsWithTag("line"))
			{
				Destroy(line);
			}
				
			foreach(GameObject square in GameObject.FindGameObjectsWithTag("square"))
			{
				Destroy(square);
			}
			if (numbersquares<totalsquares){
				numbersquares++;	
			}
			else{
				numbersquares = 1;
			}
			StartCoroutine(createSquares(numbersquares));
			


	}

	public void loadMenu()
    {
		SceneManager.LoadScene("menu");
	}

	// Update is called once per frame
	void Update () {
		//press the space bar to reset the scene
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			loadMenu();
		}
		


	}
}
