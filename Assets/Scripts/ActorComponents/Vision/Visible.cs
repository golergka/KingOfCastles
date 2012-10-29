using UnityEngine;
using System.Collections;
using System.Collections.Generic;

interface IVisibleListener {

	void OnNoticedBy(Vision observer);
	void OnLostBy(Vision observer);

}

public static class VisibleGrid {

	public static DTRMLong gridStep = new DTRMLong(10);
	public static int gridSize = 25;
	public static List<Visible>[,] grid = new List<Visible> [gridSize, gridSize];
	private static bool init = false;

	private static void Init() {

		for(int x=0; x<gridSize; x++)
			for(int y=0; y<gridSize; y++)
				grid[x,y] = new List<Visible>();

		init = true;

	}

	public static List<Visible> GetCell(DTRMVector2 position) {

		if (!init)
			Init();

		int x = ( ( position.x / gridStep ) + gridSize / 2 ).ToInt();
		int y = ( ( position.y / gridStep ) + gridSize / 2 ).ToInt();

		if ( x < 0 || x >= gridSize ||
			 y < 0 || y >= gridSize ) {

			Debug.LogError("Coordinates beyond limits for vision grid: " + position.ToString() );
			return null;

		}

		return grid[x, y];

	}

	public static List<Visible> GetNeighbors(DTRMVector2 position) {

		if (!init)
			Init();

		int x = ( ( position.x / gridStep ) + gridSize / 2 ).ToInt();
		int y = ( ( position.y / gridStep ) + gridSize / 2 ).ToInt();

		if ( x < 0 || x >= gridSize ||
			 y < 0 || y >= gridSize ) {

			Debug.LogError("Coordinates beyond limits for vision grid: " + position.ToString() );
			return null;

		}

		List<Visible> result = new List<Visible>();

		// foreach(Visible v in grid[x][y])
		// 	result.Add(v);

		for (int loopX = x-1; loopX <= x+1; loopX++)
			for (int loopY = y-1; loopY <= y+1; loopY++)
				if (loopX >= 0 && loopX < gridSize &&
					loopY >= 0 && loopY < gridSize)
					result.AddRange(grid[loopX, loopY]);

		return result;

	}

}

public class Visible : DTRMComponent {

	// public static List<Visible> allVisibles = new List<Visible>();
	private List<Visible> myCell = null;

	// all visions that have me in range. They may actually not see me if I'm invisible
	public List<Vision> inRangeOfVisions = new List<Vision>();

	private bool _visible;
	public bool visible {

		get { return _visible; }
		set {

			if ( _visible == value )
				return;

			_visible = value;

			foreach(Vision vision in inRangeOfVisions)
				vision.ChangedVisibility(this);


		}

	}

	public bool visibleOnStart = true;

	public override void DTRMStart() {

		visible = visibleOnStart;
		myCell = VisibleGrid.GetCell(myPosition.position);
		myCell.Add(this);

	}

	public override void DTRMUpdate() {

		List<Visible> myNewCell = VisibleGrid.GetCell(myPosition.position);

		if (myCell == myNewCell)
			return;

		if (myCell != null)
			myCell.Remove(this);

		myNewCell.Add(this);
		myCell = myNewCell;


	}

	void OnDestroy() {

		visible = false;

	}

	void OnDisable() {

		visible = false;

	}

	void OnEnable() {

		visible = true;

	}

}
