using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class squarecontroller : MonoBehaviour
{

    // Use this for initialization

    GameObject line;
    Material linematerial;
    Vector3 mousePosition, squarePosition;
    GameObject movingCircle,myCircle;
    int clickcounter = 0;
    List<Vector3> squarepositions;
    int levelcounter = 1;


    void Start()
    {
        //copy a reference to the list of positions that are stored in waypointgenerator
        movingCircle = Resources.Load("Prefabs/MovingCircle") as GameObject;
		myCircle = Instantiate(movingCircle, Vector3.zero, Quaternion.identity);
		myCircle.GetComponent<SpriteRenderer>().enabled = false;
        linematerial = Resources.Load("linematerial") as Material;
		
    }

    public void drawLine(Vector3 startPoint, Vector3 endPoint)
    {
        line = new GameObject();
        line.name = "Waypointline";
        line.tag = "line";

        line.AddComponent<LineRenderer>();
        line.GetComponent<LineRenderer>().sharedMaterial = linematerial;
        line.GetComponent<LineRenderer>().startWidth = 0.1f;
        line.GetComponent<LineRenderer>().endWidth = 0.1f;

        Vector3[] vertices = new Vector3[2];

        vertices[0] = startPoint;
        vertices[1] = endPoint;

        line.GetComponent<LineRenderer>().SetPositions(vertices);
    }

    bool checkallsquaresred()
    {
        bool allsquaresred = true;
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("square"))
        {
            if (g.GetComponent<SpriteRenderer>().color != Color.red)
            {
                allsquaresred = false;
            }
        }
        return allsquaresred;
    }

    IEnumerator moveCircle(GameObject circle, List<Vector3> positions)
    {
        if (circle != null)
        {
			circle.GetComponent<SpriteRenderer>().enabled = true;
            for (int i = 1; i < positions.Count; i++)
            {
                float distance = 0;
                while (distance < 1f)
                {
					Debug.Log(positions[i]);
                    circle.transform.position = Vector3.Lerp(circle.transform.position, positions[i], distance);
                    distance += 0.1f;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return null;
            }
           circle.GetComponent<SpriteRenderer>().enabled = false;
        }
    }


    // Update is called once per frame
    void Update()
    {
        //x and y coordinates in pixels
        mousePosition = new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y, 10f);
        //screen points
        //	Debug.Log(mousePosition.x + " "+mousePosition.y);

        squarePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        //	Debug.Log(squarePosition.x + " "+squarePosition.y);

        transform.position = squarePosition;

        if (Input.GetMouseButtonDown(0))
        {
            //Camera.main.GetComponent<waypointgenerator>().copySquare(squarePosition);
            //I want to detect whether or not I've hit a square
            RaycastHit2D hit = new RaycastHit2D();
            hit = Physics2D.Raycast(squarePosition, Vector3.forward);

            //write what I hit in the console
            if (hit.collider != null)
            {
                //first things first, can I click the box, is it the next box? 
                squarepositions = Camera.main.GetComponent<gamecontroller>().positions;
                Vector3 boxhitposition = hit.collider.transform.position;

                if (Vector3.Distance(boxhitposition, squarepositions[clickcounter]) < 0.2f)
                {
                    hit.collider.gameObject.GetComponent<SpriteRenderer>().color = Color.red;

					//draw line to the previous box
				

                    clickcounter++;
					Debug.Log(clickcounter);
					if (clickcounter > 1){
							drawLine(squarepositions[clickcounter-1],squarepositions[clickcounter-2]);
					}
                }


            }
            //**** can only be red if it is the next number ****//
            //TASK
            /*
			Create an animation that generates a small circle which at the start of each level,
			travels slowly from one box to the next until it reaches the last box, after which
			it disappears.  You may use a coroutine to perform this task
			 */
            //if all the squares are red
            if (checkallsquaresred())
            {
                clickcounter = 0;
                //reset the scene and add a box
                Camera.main.GetComponent<gamecontroller>().resetScene();	
                //increase the level
                levelcounter++;
                //update the UI text with the level counter here
                GameObject.Find("Level").GetComponent<Text>().text = "<b>" + levelcounter + "</b>";
            }

            //ADD a counter as to how many times resetscene was called, and I want you to show it in the TOP RIGHT of the screen
            if (levelcounter > 1)
            {
                //I need to spawn the circle.
                //place a circle in the first position
                squarepositions = Camera.main.GetComponent<gamecontroller>().positions;
				myCircle.transform.position = squarepositions[0];
				StopAllCoroutines();
                StartCoroutine(moveCircle(myCircle, squarepositions));
            }
        }
    }
}
