﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {
	[SerializeField] private Text freeSpaceText;
	[SerializeField] private Text itemCountText;
	[SerializeField] private GridLayoutGroup inventoryGrid;
	[SerializeField] private GridLayoutGroup equipmentGrid;
	[SerializeField] private ItemSlot itemSlotPrefab;
	[SerializeField] private BodyPartSlot bodyPartSlotPrefab;
	private Character character;
	private readonly List<Item> displayedItems = new();
	private readonly List<ItemSlot> itemSlots = new();

	private readonly List<BodyPart> displayedBodyParts = new();

	private void Awake() {
		character = LocalManager.PlayerCharacter;
	}

	private void Update() {
		if (character == null) return;

		freeSpaceText.text = character.inventory.FreeSpace + "/" + character.inventory.maxSize;
		itemCountText.text = character.inventory.ItemCount.ToString();

		foreach (var item in character.inventory.Items) {
			if (!displayedItems.Contains(item)) {
				var itemSlot = Instantiate(itemSlotPrefab, Vector3.zero, Quaternion.identity, inventoryGrid.transform);
				itemSlot.Item = item;
				displayedItems.Add(item);
				itemSlots.Add(itemSlot);
			}
		}

		itemSlots.RemoveAll(item => item == null);
		foreach (var itemSlot in itemSlots) {
			if (!character.inventory.Contains(itemSlot.Item) || itemSlot.Item == null) {
				displayedItems.Remove(itemSlot.Item);
				Destroy(itemSlot.gameObject);
			}
		}

		foreach (var bodyPart in character.body.bodyParts) {
			if (bodyPart.slot != Slot.None && !displayedBodyParts.Contains(bodyPart)) {
				var bodyPartSlot = Instantiate(bodyPartSlotPrefab, Vector3.zero, Quaternion.identity, equipmentGrid.transform);
				bodyPartSlot.bodyPart = bodyPart;
				displayedBodyParts.Add(bodyPart);
			}
		}
	}
}