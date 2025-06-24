using Godot;
using System;
using System.Collections.Generic;

public static class NodeCreator
{
	private const string SceneBasePath = "res://Nodes/CustomNodes";

	public static Node CreateNode(string sceneFileName, Dictionary<string, object> namedProperties = null, string nodeName = null)
	{
		string scenePath = SceneBasePath + sceneFileName + ".tscn";
		PackedScene scene = ResourceLoader.Load<PackedScene>(scenePath);

		if (scene == null)
		{
			GD.PrintErr($"Scene not found: {scenePath}");
			return null;
		}

		Node instance = scene.Instantiate();
		if (!string.IsNullOrEmpty(nodeName))
			instance.Name = nodeName;

		if (namedProperties != null)
		{
			foreach (KeyValuePair<string, object> pair in namedProperties)
			{
				Node subNode = instance.GetNodeOrNull(pair.Key);
				if (subNode == null)
				{
					GD.PrintErr($"Subnode '{pair.Key}' not found in scene '{scenePath}'.");
					continue;
				}

				switch (subNode)
				{
					case Label label when pair.Value is string str:
						label.Text = str;
						break;
					case LineEdit input when pair.Value is string strInput:
						input.Text = strInput;
						break;
					// Add support for other types here
					default:
						GD.PrintErr($"Unsupported node or property type at '{pair.Key}'");
						break;
				}
			}
		}
		return instance;
	}
}
