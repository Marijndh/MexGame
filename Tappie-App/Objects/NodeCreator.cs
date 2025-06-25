using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
public static class NodeCreator
{
	private const string SceneBasePath = "res://Nodes/CustomNodes/";

	public static Node CreateNode(string sceneFileName, Godot.Collections.Dictionary<string, Variant> namedProperties = null, string nodeName = null)
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
			foreach (KeyValuePair<string, Variant> pair in namedProperties)
			{
				Node subNode = instance.GetNodeOrNull(pair.Key);
				if (subNode == null)
				{
					GD.PrintErr($"Subnode '{pair.Key}' not found in scene '{scenePath}'.");
					continue;
				}

				switch (subNode)
				{
					case Label label when pair.Value.VariantType == Variant.Type.String:
						label.Text = pair.Value.AsString();
						break;
					case LineEdit input when pair.Value.VariantType == Variant.Type.String:
						input.Text = pair.Value.AsString();
						break;
					default:
						GD.PrintErr($"Unsupported node or property type at '{pair.Key}'");
						break;
				}
			}
		}
		return instance;
	}
}
