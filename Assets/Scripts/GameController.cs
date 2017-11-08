using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public GameObject cubePrefab;
	new Vector3 cubePosition;
	GameObject currentCube;
	public Text CargoText;
	public static GameObject activePlane;
	public static int airplaneX, airplaneY;
	int gridWidth;
	int gridHeight;
	static GameObject[ , ] grid;
	int airplaneMoveY, airplaneMoveX;

	// variables to keep track of plane turns and cargo
	int numOfTurns;
	int cargoTons;
	int maxCargo = 90;
	int cargoPoints;
	float turnTime;
	public static int deliveryDepotX, deliveryDepotY;


	// Use this for initialization
	void Start () {

		gridWidth = 16;
		gridHeight = 9;
		grid = new GameObject[gridWidth, gridHeight];
		numOfTurns = 1;
		cargoTons = 0; // plane starts off with 0 ton of cargo
		turnTime = 1.5f; // plane turns every 1.5 seconds;
		deliveryDepotX = 15;
		deliveryDepotY = 0;

		// starting location of airplane
		airplaneX = 0;
		airplaneY = 8;

		for (int y = 0; y < gridHeight; y++) {
			for (int x = 0; x < gridWidth; x++) {
				cubePosition = new Vector3 (x * 2 - 16, y * 2 - 10, 0);
				currentCube = Instantiate (cubePrefab, cubePosition, Quaternion.identity);
				currentCube.GetComponent<CubeBehavior>().cubePositionX = x;
				currentCube.GetComponent<CubeBehavior>().cubePositionY = y;
				grid [x, y] = currentCube;

				if (x == airplaneX && y == airplaneY) {
					currentCube.GetComponent<Renderer> ().material.color = Color.red;
					currentCube.GetComponent<CubeBehavior> ().plane = true;
					activePlane = currentCube;
				}

				if (x == deliveryDepotX && y == deliveryDepotY) {
					currentCube.GetComponent<Renderer> ().material.color = Color.black;
				}



			}

		}
		airplaneMoveX = 0;
		airplaneMoveY = 0;

	}

	public static void ProcessClick (GameObject currentCube) {

		// deactivate active plane
		if (currentCube == activePlane && currentCube.GetComponent<CubeBehavior>().plane){
			currentCube.GetComponent<Renderer> ().material.color = Color.blue;
			activePlane = null;
		}
	    // activate deactivated plane
		else if (currentCube != activePlane && currentCube.GetComponent<CubeBehavior>().plane) {
			currentCube.GetComponent<Renderer> ().material.color = Color.red;
			activePlane = currentCube;
		} 

	}


	//	Tracks what arrow keys are being pressed
	void DetectKeyboardInput() {
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			airplaneMoveY = -1;
			airplaneMoveX = 0;
		} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
			airplaneMoveY = 1;
			airplaneMoveX = 0;
		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			airplaneMoveY = 0;
			airplaneMoveX = -1;
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			airplaneMoveY = 0;
			airplaneMoveX = 1;
		}

	}

	void MoveAirplane(){

		// check if airplane is active
		if (activePlane) {
			
			// move airplane from previous spot, and set cube to black if it's a delivery depot
			if (airplaneX == deliveryDepotX && airplaneY == deliveryDepotY) {
				grid [deliveryDepotX, deliveryDepotY].GetComponent<Renderer> ().material.color = Color.black;
			}
			// if it isn't a delivery depot, set it to white
			else {
				grid [airplaneX, airplaneY].GetComponent<Renderer> ().material.color = Color.white;
			}

			// check if plane moves
			if (airplaneMoveX != 0 || airplaneMoveY !=0) {
				grid [airplaneX, airplaneY].GetComponent<CubeBehavior> ().plane = false;

			}
				

			// if airplane is active, then it should move in the direction that the player presses the key for
			airplaneX += airplaneMoveX;
			airplaneY += airplaneMoveY;

			// make sure airplane doesn't move off the grid in X direction
			if (airplaneX >= gridWidth) {
				airplaneX = gridWidth - 1;
			} else if (airplaneX < 0) {
				airplaneX = 0;
			}

			// make sure airplane doesn't move off the grid in Y direction
			if (airplaneY >= gridHeight) {
				airplaneY = gridHeight - 1;
			} else if (airplaneY < 0) {
				airplaneY = 0;
			}
				
			grid [airplaneX, airplaneY].GetComponent<Renderer> ().material.color = Color.red;

			activePlane = grid [airplaneX, airplaneY];
			grid [airplaneX, airplaneY].GetComponent<CubeBehavior> ().plane = true;
		} 

		// reset movement for next turn
		airplaneMoveX = 0;
		airplaneMoveY = 0;


	}



	// Update is called once per frame
	void Update () {
		// check for keyboard input and record it, but don't execute the move yet
		DetectKeyboardInput ();

		if (Time.time > turnTime * numOfTurns)  {
			// execute move here
			MoveAirplane();


			numOfTurns++; // As time in the game increases by 1.5 seconds and the plane continues to make turns,

			// check if cube is a plane
			if (airplaneX == 0 && airplaneY == 8 && activePlane){
				// increase cargo in the plane by 10 tons
				cargoTons += 10;

				// if cargoTons is more than maxCargo, then set value of cargoTons equal to value of maxCargo
				if (cargoTons > maxCargo) {
					cargoTons = maxCargo;

				}

			}

			// checks if plane is at deliveryDepot
			if (airplaneX == deliveryDepotX && airplaneY == deliveryDepotY) {
				// scores +1 cargo
				cargoPoints += cargoTons;
				// removes all cargo into deliveryDepot
				cargoTons = 0;
			}


		}
		CargoText.text = "Tons of Cargo: " + cargoTons + " Cargo Points: " + cargoPoints;
	}
}
