using System.Collections.Generic;

public class WorldLog {
	private readonly World world;

	private readonly List<WorldEvent> events = new();

	public WorldLog(World world) {
		this.world = world;
	}

	public void Add(WorldEvent worldEvent) {
		events.Add(worldEvent);
	}

	public string GetLog(WorldEventImportance minImportance) {
		var str = "";

		for (var day = 0; day <= world.Day; day++) {
			var eventFound = false;
			foreach (var worldEvent in events) {
				if (worldEvent.day == day && worldEvent.importance >= minImportance) {
					if (!eventFound) {
						str += $"On {GetDay(day)},";
						eventFound = true;
					}

					str += $" {worldEvent.text},";
				}
			}

			if (eventFound) {
				str = str.Remove(str.Length - 1);
				str += ".\n";
			}
		}

		return str;
	}

	private static string GetDay(int day) {
		return $"day {day}";
	}
}