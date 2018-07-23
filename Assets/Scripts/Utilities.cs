using UnityEngine;
using System;
using System.Reflection;

public static class Utilities {
    public static void MergeValues<T> (T target, T source) {
        var properties = target.GetType().GetFields();

        foreach (var prop in properties) {
            var value = prop.GetValue(source);
            if (value != null) prop.SetValue(target, value);
        }
    }
}