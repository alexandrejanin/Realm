﻿using System;
using Sirenix.OdinInspector;
using UnityEngine;

#pragma warning disable 0649

[Serializable]
public class Race {
	[TabGroup("Names")] public string collectiveName, individualName, adjective;

	[SerializeField, TabGroup("Names")] private string[] placeNames, maleFirstNames, femaleFirstNames, lastNames;

	[SerializeField, TabGroup("Names")] private Vector2Int placeNameLength = new(1, 2), firstNameLength = new(1, 2), lastNameLength = new(1, 3);

	[Range(0, 1), TabGroup("Characteristics")]
	public float expansionism, hostility;

	[TabGroup("Characteristics")] public bool likesWater;

	[MinMaxSlider(0f, 1f, true), TabGroup("Range")]
	public Vector2 heightRange = new(0, 1), tempRange = new(0, 1), humidityRange = new(0, 1);

	[Range(0, 1), TabGroup("Preferred")] public float heightPreferred = .5f, tempPreferred = .5f, humidityPreferred = .5f;

	[SerializeField, Range(0, 1), TabGroup("Weight")]
	private float heightWeight = .33f, tempWeight = .33f, humidityWeight = .33f;

	private float TotalWeight => heightWeight + tempWeight + humidityWeight;
	public float HeightWeight => heightWeight / TotalWeight;
	public float TempWeight => tempWeight / TotalWeight;
	public float HumidityWeight => humidityWeight / TotalWeight;

	public bool IsValidTile(Tile tile) => !tile.IsWater && heightRange.Contains(tile.height) && tempRange.Contains(tile.temp) && humidityRange.Contains(tile.humidity);

	public string GetPlaceName() {
		var placeName = "";
		var length = placeNameLength.Random();
		for (var i = 0; i < length; i++) {
			placeName += placeNames[GameController.Random.Next(0, placeNames.Length)];
		}

		return placeName.Capitalize();
	}

	public string GetFirstName(bool isFemale) {
		var firstName = "";
		var length = firstNameLength.Random();
		var names = isFemale ? femaleFirstNames : maleFirstNames;
		for (var i = 0; i < length; i++) {
			firstName += names.RandomItem();
		}

		return firstName.Capitalize();
	}

	public string GetLastName() {
		var lastName = "";
		var length = lastNameLength.Random();
		for (var i = 0; i < length; i++) {
			lastName += lastNames.RandomItem();
		}

		return lastName.Capitalize();
	}

	public override string ToString() => collectiveName.Capitalize();
}