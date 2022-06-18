﻿using System;
using System.Collections.Generic;
using System.Linq;

public class Body {
	public readonly HashSet<BodyPart> bodyParts;

	private readonly HashSet<Equipable> equipables = new();

	public HashSet<Equipable> Equipables {
		get {
			equipables.Clear();
			foreach (var bodyPart in bodyParts) {
				if (bodyPart.equipable != null && !equipables.Contains(bodyPart.equipable)) {
					equipables.Add(bodyPart.equipable);
				}
			}

			return equipables;
		}
	}

	public Body(BodyType bodyType) {
		bodyParts = GetBody(bodyType);

		foreach (var bodyPart in bodyParts) {
			bodyPart.body = this;
		}
	}

	public void RemovePart(BodyPart bodyPart) {
		foreach (var child in bodyPart.children) {
			RemovePart(child);
		}

		bodyPart.parent = null;
		bodyParts.Remove(bodyPart);
	}

	public IEnumerable<BodyPart> PartsWithAttribute(BodyPartAttribute attribute) => bodyParts.Where(part => part.HasAttribute(attribute));
	public bool HasPartWithAttribute(BodyPartAttribute attribute) => bodyParts.Any(part => part.HasAttribute(attribute));

	private static HashSet<BodyPart> GetBody(BodyType bodyType) {
		HashSet<BodyPart> bodyParts;

		switch (bodyType) {
			case BodyType.Humanoid:
				var torso = new BodyPart("Torso", null, Slot.Torso, BodyPartX.Middle, BodyPartY.Middle, BodyPartZ.Center,
					new HashSet<BodyPartAttribute> {BodyPartAttribute.Breathing, BodyPartAttribute.Vital});
				var abdomen = new BodyPart("Abdomen", torso, Slot.None, BodyPartX.Middle, BodyPartY.Bottom, BodyPartZ.Center,
					new HashSet<BodyPartAttribute> {BodyPartAttribute.Breathing, BodyPartAttribute.Vital});
				var neck = new BodyPart("Neck", torso, Slot.Neck, BodyPartX.Middle, BodyPartY.Top, BodyPartZ.Center,
					new HashSet<BodyPartAttribute> {BodyPartAttribute.Breathing, BodyPartAttribute.Vital});
				var head = new BodyPart("Head", neck, Slot.Head, BodyPartX.Middle, BodyPartY.Top, BodyPartZ.Center,
					new HashSet<BodyPartAttribute> {BodyPartAttribute.Breathing, BodyPartAttribute.Seeing, BodyPartAttribute.Thinking, BodyPartAttribute.Vital});
				var leftArm = new BodyPart("Left Arm", torso, Slot.None, BodyPartX.Left, BodyPartY.Middle, BodyPartZ.Center,
					new HashSet<BodyPartAttribute> {BodyPartAttribute.Limb});
				var leftHand = new BodyPart("Left Hand", leftArm, Slot.Hand, BodyPartX.Left, BodyPartY.Bottom, BodyPartZ.Center,
					new HashSet<BodyPartAttribute> {BodyPartAttribute.Grasping});
				var rightArm = new BodyPart("Right Arm", torso, Slot.None, BodyPartX.Right, BodyPartY.Middle, BodyPartZ.Center,
					new HashSet<BodyPartAttribute> {BodyPartAttribute.Limb});
				var rightHand = new BodyPart("Right Hand", rightArm, Slot.Hand, BodyPartX.Right, BodyPartY.Bottom, BodyPartZ.Center,
					new HashSet<BodyPartAttribute> {BodyPartAttribute.Grasping});
				var leftLeg = new BodyPart("Left Leg", abdomen, Slot.Legs, BodyPartX.Left, BodyPartY.Bottom, BodyPartZ.Center,
					new HashSet<BodyPartAttribute> {BodyPartAttribute.Limb, BodyPartAttribute.Walking});
				var leftFoot = new BodyPart("Left Foot", leftLeg, Slot.Feet, BodyPartX.Left, BodyPartY.Bottom, BodyPartZ.Center,
					new HashSet<BodyPartAttribute> {BodyPartAttribute.Walking});
				var rightLeg = new BodyPart("Right Leg", abdomen, Slot.Legs, BodyPartX.Right, BodyPartY.Bottom, BodyPartZ.Center,
					new HashSet<BodyPartAttribute> {BodyPartAttribute.Limb, BodyPartAttribute.Walking});
				var rightFoot = new BodyPart("Right Foot", leftLeg, Slot.Feet, BodyPartX.Right, BodyPartY.Bottom, BodyPartZ.Center,
					new HashSet<BodyPartAttribute> {BodyPartAttribute.Walking});
				bodyParts = new HashSet<BodyPart> {head, neck, torso, abdomen, leftArm, leftHand, rightArm, rightHand, leftLeg, leftFoot, rightLeg, rightFoot};
				break;
			case BodyType.Quadruped:
				bodyParts = new HashSet<BodyPart>();
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(bodyType), bodyType, null);
		}

		return bodyParts;
	}
}

public enum BodyType {
	Humanoid,
	Quadruped
}