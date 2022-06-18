using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {
    public virtual string Name { get; }
    public readonly Location location;
    public readonly Floor floor;

    public readonly List<Character> characters = new();
    public readonly List<Item> items = new();
    public readonly List<Interactable> interactables = new();

    public RoomManager(string name, Location location) {
        Name = name;
        this.location = location;
        floor = new Floor();
    }
}
