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
	public Vector3 origin;
	public Vector3 blockSize;
	public GameObject blockPrefab;

	private System.Random rand;
	private List<GameObject> blocks;

	// Use this for initialization
	void Start () {
		rand = new System.Random ();
		HashSet<Edge> connections = getRandomConnections(new Point(columns, rows));

		Point size = new Point (columns * 2 + 1, rows * 2 + 1);
		bool[,] maze = initializedBoolArray (size, true);
		foreach (Edge e in connections) {
			int betweenX = Mathf.Min(e.P1.x * 2 + 1, e.P2.x * 2 + 1) + Mathf.Abs((e.P2.x * 2 + 1) - (e.P1.x * 2 + 1)) / 2;
			int betweenY = Mathf.Min(e.P1.y * 2 + 1, e.P2.y * 2 + 1) + Mathf.Abs((e.P2.y * 2 + 1) - (e.P1.y * 2 + 1)) / 2;
			maze[e.P1.x * 2 + 1, e.P1.y * 2 + 1] = false;
			maze[betweenX, betweenY] = false;
			maze[e.P2.x * 2 + 1, e.P2.y * 2 + 1] = false;
		}

		blockPrefab.transform.localScale = blockSize;
		blocks = new List<GameObject> ();
		for (int x = 0; x < maze.GetLength(0); x++) {
			for (int y = 0; y < maze.GetLength(1); y++) {
				if(maze[x, y]) {
					float posX = origin.x + x * blockSize.x;
					float posY = origin.y;
					float posZ = origin.z + y * blockSize.z;
					blocks.Add(Instantiate(blockPrefab, new Vector3(posX, posY, posZ), Quaternion.identity) as GameObject);
				}
			}
		}
	}

	private HashSet<Edge> getRandomConnections(Point size) {
		bool[,] visited = initializedBoolArray (size, false);
		int visitedCount = 0;
		HashSet<Edge> edges = new HashSet<Edge>();
		List<Point> points = new List<Point> ();

		Point initialPoint = new Point (rand.Next (size.x), rand.Next (size.y));
		points.Add (initialPoint);
		visit (initialPoint, ref visited, ref visitedCount);


		while (visitedCount < size.x * size.y) {
			//Point active = points[0];
			//Point active = points[rand.Next(points.Count)];
			Point active = points[points.Count - 1];

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

	private Point unvisitedNeighborOf(Point p, Point size, bool[,] visited) {
		List<Point> unvisitedNeighbors = new List<Point> (4);
		foreach (Dir direction in System.Enum.GetValues(typeof(Dir))) {
			Point neighbor = p + direction;
			if(inBounds(neighbor, size) && !visited[neighbor.x, neighbor.y]) {
				unvisitedNeighbors.Add(neighbor);
			}
		}
		if (unvisitedNeighbors.Count > 0) {
			return unvisitedNeighbors [rand.Next (unvisitedNeighbors.Count)];
		} else {
			return null;
		}
	}

	private void visit(Point p, ref bool[,] visited, ref int visitedCount) {
		visited[p.x, p.y] = true;
		visitedCount++;
	}
	
	private bool inBounds(Point p, Point size) {
		return p.x >= 0 && p.x < size.x && p.y >= 0 && p.y < size.y;
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
