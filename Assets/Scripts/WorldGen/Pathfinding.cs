using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public static class Pathfinding {
	[CanBeNull]
	public static LinkedList<Tile> FindPath([NotNull] Tile start, [NotNull] Tile goal, [CanBeNull] Race race) {
		var open = new Heap<Tile>(start.world.width);
		var closed = new HashSet<Tile>();

		open.Add(start);

		while (open.Count > 0) {
			var current = open.RemoveFirst();
			closed.Add(current);

			if (current == goal) {
				return RetracePath(start, goal);
			}

			foreach (var neighbor in current.GetNeighbors()) {
				if (neighbor.IsWater || closed.Contains(neighbor)) continue;

				var movementCost = current.gCost + GetDistance(current, neighbor);

				if (race != null) {
					var compatibility = neighbor.GetRaceCompatibility(race);

					movementCost += Mathf.RoundToInt((1 - compatibility * compatibility) * 100);
				}

				if (movementCost < neighbor.gCost || !open.Contains(neighbor)) {
					neighbor.gCost = movementCost;
					neighbor.hCost = GetDistance(neighbor, goal);
					neighbor.parent = current;

					if (open.Contains(neighbor)) {
						open.Update(neighbor);
					} else {
						open.Add(neighbor);
					}
				}
			}
		}

		return null;
	}

	[NotNull]
	private static LinkedList<Tile> RetracePath(Tile start, Tile goal) {
		var path = new LinkedList<Tile>();

		var current = new LinkedListNode<Tile>(goal);
		path.AddLast(current);

		while (current.Value != start) {
			current = path.AddBefore(current, current.Value.parent);
		}

		return path;
	}

	private static int GetDistance(Tile a, Tile b) {
		var dx = (a.x - b.x).Abs();
		var dy = (a.y - b.y).Abs();

		if (dx > dy) return 14 * dy + 10 * (dx - dy);
		return 14 * dx + 10 * (dy - dx);
	}
}
