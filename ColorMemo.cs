using UnityEngine;
using UnityEditor;

public class ColorMemo : EditorWindow
{
	Color color1;
	Color color2;
	Color color3;
	Color color4;
	Color color5;

	[MenuItem ("Window/ColorMemo")]
	static void Init ()
	{
		GetWindow (typeof(ColorMemo));
	}

	void OnGUI ()
	{
		color1 = new Color (0, 0, 0);
		color1 = EditorGUILayout.ColorField ("HEX:", color1);
		EditorGUILayout.Separator ();

		color2 = new Color (38.0f / 255.0f, 50.0f / 255.0f, 72.0f / 255.0f);
		color2 = EditorGUILayout.ColorField ("HEX:", color2);
		EditorGUILayout.Separator ();

		color3 = new Color (126.0f / 255.0f, 138.0f / 255.0f, 162.0f / 255.0f);
		color3 = EditorGUILayout.ColorField ("HEX:", color3);
		EditorGUILayout.Separator ();

		color4 = new Color (1, 1, 1);
		color4 = EditorGUILayout.ColorField ("HEX:", color4);
		EditorGUILayout.Separator ();

		color5 = new Color (1, 152.0f / 255.0f, 0);
		color5 = EditorGUILayout.ColorField ("HEX:", color5);
		EditorGUILayout.Separator ();

		GUI.enabled = true;
	}
}

