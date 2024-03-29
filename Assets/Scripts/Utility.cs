﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public static class Utility {
    public static string Capitalize(this string s) => string.IsNullOrEmpty(s) ? s : string.Concat(s.First().ToString().ToUpper(), s.Substring(1));
    public static string ToTitleCase(this string s) => string.IsNullOrEmpty(s) ? s : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s);

    public static T RandomItem<T>(this IList<T> list, Random random) => list[random.Next(list.Count)];

    public static T RandomValue<T>(Random random) where T : IConvertible, IFormattable, IComparable => ((T[])Enum.GetValues(typeof(T))).RandomItem(random);

    public static void FromList(this Dropdown dropdown, string[] values) {
        float height = 50 * values.Length;

        var template = dropdown.template.rect;

        template.height = height;

        dropdown.template.anchoredPosition = new Vector2(0, height + 60);

        dropdown.options = values.Select(name => new Dropdown.OptionData(name)).ToList();

        dropdown.onValueChanged.Invoke(dropdown.value);
    }

    public static int Abs(this int i) => i < 0 ? -i : i;
    public static float Abs(this float i) => i < 0 ? -i : i;

    public static int Sign(this int i) => i > 0 ? 1 : i < 0 ? -1 : 0;
    public static float Sign(this float f) => f > 0 ? 1 : f < 0 ? -1 : 0;

    public static bool Contains(this Vector2 v2, float f) => v2.x <= f && f <= v2.y;
    public static float Average(this Vector2 v2) => (v2.x + v2.y) / 2;
    public static float Range(this Vector2 v2) => v2.y - v2.x;

    public static Vector2Int RandomDirection(Random random) {
        int x, y;

        do {
            x = random.Next(-1, 2);
            y = random.Next(-1, 2);
        } while (x == 0 && y == 0);

        return new Vector2Int(x, y);
    }

    public static bool Bool(this Random random) => random.NextDouble() > 0.5f;
}
