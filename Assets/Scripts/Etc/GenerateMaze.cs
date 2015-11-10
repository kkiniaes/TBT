using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateMaze : MonoBehaviour {
	private class Edge {
		private Point p1;
		private Point p2;

		public Point P1 { get { return p1; } }
		public Point P2 { get { return p2; } }

		public Edge(int x1, int y1, int x2, int y2) {
			if(x1 < x2) {
				this.p1 = new Point(x1, y1);
				this.p2 = new Point(x2, y2);
			} else {
				this.p1 = new Point(x2, y2);
				this.p2 = new Point(x1, y1);
			}
		}

		public Edge(Point p1, Point p2) {
			if(p1.x < p2.x) {
				this.p1 = p1;
				this.p2 = p2;
			} else {
				this.p1 = p2;
				this.p2 = p1;
			}
		}

		public override bool Equals(object o) {
			if(o.GetType() != typeof(Edge)) return false;
			Edge e = (Edge)o;

			return p1.x == e.P1.x && p1.y == e.P1.y && p2.x == e.P2.x && p2.y == e.P2.y;
		}

		public override int GetHashCode ()
		{
			const int prime = 31;
			int result = 1;

			unchecked {
				result = prime * result + p1.x;
				result = prime * result + p1.y;
				result = prime * result + p2.x;
				result = prime * result + p2.y;
			}

			return result;
		}
	}

	private enum Dir {
		Up,
		Down,
		Left,
		Right,
	}

	private class Point {
		public int x;
		public int y;

		public static readonly Point UP = new Point (0, 1);
		public static readonly Point DOWN = new Point (0, -1);
		public static readonly Point LEFT = new Point (-1, 0);
		public static readonly Point RIGHT = new Point (1, 0);

		public Point(int x, int y) {
			this.x = x;
			this.y = y;
		}

		public static Point operator +(Point p1,  Point p2) {
			return new Point (p1.x + p2.x, p1.y + p2.y);
		}

		public static Point operator +(Point p,  Dir direction) {
			switch (direction) {
				case Dir.Up: return p + UP;
				case Dir.Down: return p + DOWN;
				case Dir.Left: return p + LEFT;
				case Dir.Right: return p + RIGHT;
				default: return null;
			}
		}

		public static Point operator +(Dir direction, Point p) {
			return p + direction;
		}
	}

	public int columns;
	public int rows;
	public float loopingAmount = 0.01f; //%, between 0 & 1
	public bool addGoals;
	public bool border;
	public Vector3 origin;
	public Vector3 blockSize;
	public GameObject blockPrefab;
	public GameObject goalPrefab;

	private System.Random rand;
	private List<GameObject> blocks;

	// Use this for initialization
	void Start () {
		rand = new System.Random ();
		HashSet<Edge> connections = getRandomConnections(new Point(columns, rows));

		int extraBorderSpace = (border) ? 2 : 0;
		Point size = new Point (columns * 2 + extraBorderSpace - 1, rows * 2 + extraBorderSpace - 1);
		bool[,] maze = initializedBoolArray (size, true);
		int offset = (border) ? 1 : 0;
		foreach (Edge e in connections) {
			int betweenX = Mathf.Min(e.P1.x * 2 + offset, e.P2.x * 2 + offset) + Mathf.Abs((e.P2.x * 2 + offset) - (e.P1.x * 2 + offset)) / 2;
			int betweenY = Mathf.Min(e.P1.y * 2 + offset, e.P2.y * 2 + offset) + Mathf.Abs((e.P2.y * 2 + offset) - (e.P1.y * 2 + offset)) / 2;
			maze[e.P1.x * 2 + offset, e.P1.y * 2 + offset] = false;
			maze[betweenX, betweenY] = false;
			maze[e.P2.x * 2 + offset, e.P2.y * 2 + offset] = false;
		}

		for (int x = offset; x < maze.GetLength(0) - offset; x++) {
			for (int y = offset; y < maze.GetLength(1) - offset; y++) {
				if(!((x - offset) % 2 != 0 && (y - offset) % 2 != 0) && rand.NextDouble() < loopingAmount) {
					maze[x, y] = false;
				}
			}
		}

		blocks = new List<GameObject> ();
		for (int x = 0; x < maze.GetLength(0); x++) {
			for (int y = 0; y < maze.GetLength(1); y++) {
				if(maze[x, y]) {
					float posX = origin.x + x * blockSize.x;
					float posY = origin.y;
					float posZ = origin.z + y * blockSize.z;
					blocks.Add(Instantiate(blockPrefab, new Vector3(posX, posY, posZ), Quaternion.identity) as GameObject);
					blocks[blocks.Count - 1].transform.localScale = blockSize;
					PhysicsAffected.TryAddPM(blocks[blocks.Count - 1].GetComponent<PhysicsModifyable>());
				}
			}
		}

		if (addGoals) {
			//float ySign = Mathf.Sign(origin.y);

			float posX1 = origin.x + extraBorderSpace * blockSize.x / 2;
			float posY1 = origin.y;// + blockSize.y / 2 * ySign;
			float posZ1 = origin.z + extraBorderSpace * blockSize.z / 2;
			GameObject goal1 = Instantiate(goalPrefab, new Vector3(posX1, posY1, posZ1), Quaternion.identity) as GameObject;
			goal1.transform.localScale = blockSize * 0.4f;

			float posX2 = origin.x + (size.x - extraBorderSpace) * blockSize.x;
			float posY2 = origin.y;// + blockSize.y / 2 * ySign;
			float posZ2 = origin.z + (size.y - extraBorderSpace) * blockSize.z;
			GameObject goal2 = Instantiate(goalPrefab, new Vector3(posX2, posY2, posZ2), Quaternion.identity) as GameObject;
			goal2.transform.localScale = blockSize * 0.4f;
		}
	}

	private HashSet<Edge> getRandomConnections(Point size) {
		int[,] visited = initializedIntArray (size, false);
		int visitedCount = 0;
		HashSet<Edge> edges = new HashSet<Edge>();
		List<Point> points = new List<Point> ();

		Point initialPoint = new Point (rand.Next (size.x), rand.Next (size.y));
		points.Add (initialPoint);
		visit (initialPoint, ref visited, ref visitedCount);

		//int maxConnections = (size.x * size.y * 4 - size.x * 2 - size.y * 2) / 2;
		while (visitedCount < size.x * size.y) {
			Point active;

			//active = points[0];
			//active = points[rand.Next(points.Count)];
			active = points[points.Count - 1];

			Point unvisitedNeighbor = unvisitedNeighborOf(active, size, visited);

			if(unvisitedNeighbor == null) {
				points.Remove(active);
			} else {
				visit(unvisitedNeighbor, ref visited, ref visitedCount);
				points.Add(unvisitedNeighbor);
				edges.Add(new Edge(active, unvisitedNeighbor));
			}
		}

		return edges;
	}

	private Point unvisitedNeighborOf(Point p, Point size, int[,] visited) {
		List<Point> unvisitedNeighbors = new List<Point> (4);
		foreach (Dir direction in System.Enum.GetValues(typeof(Dir))) {
			Point neighbor = p + direction;
			if(inBounds(neighbor, size) && visited[neighbor.x, neighbor.y] < 1) {
				unvisitedNeighbors.Add(neighbor);
			}
		}
		if (unvisitedNeighbors.Count > 0) {
			return unvisitedNeighbors [rand.Next (unvisitedNeighbors.Count)];
		} else {
			return null;
		}
	}
	private Point unvisitedNeighborOf(Point p, Point size, int[,] visited, int maxVisits) {
		List<Point> unvisitedNeighbors = new List<Point> (4);
		foreach (Dir direction in System.Enum.GetValues(typeof(Dir))) {
			Point neighbor = p + direction;
			if(inBounds(neighbor, size) && visited[neighbor.x, neighbor.y] <= maxVisits) {
				unvisitedNeighbors.Add(neighbor);
			}
		}
		if (unvisitedNeighbors.Count > 0) {
			return unvisitedNeighbors [rand.Next (unvisitedNeighbors.Count)];
		} else {
			return null;
		}
	}

	private void visit(Point p, ref int[,] visited, ref int visitedCount) {
		visited[p.x, p.y]++;
		visitedCount++;
	}
	
	private bool inBounds(Point p, Point size) {
		return p.x >= 0 && p.x < size.x && p.y >= 0 && p.y < size.y;
	}

	private int[,] initializedIntArray(Point size, bool value) {
		int[,] array = new int[size.x, size.y];
		for (int x = 0; x < array.GetLength(0); x++) {
			for (int y = 0; y < array.GetLength(1); y++) {
				array[x, y] = 0;
			}
		}
		return array;
	}

	private bool[,] initializedBoolArray(Point size, bool value) {
		bool[,] array = new bool[size.x, size.y];
		for (int x = 0; x < array.GetLength(0); x++) {
			for (int y = 0; y < array.GetLength(1); y++) {
				array[x, y] = value;
			}
		}
		return array;
	}
}
