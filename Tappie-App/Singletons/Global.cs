using Godot;
using System;
using System.Collections.Generic;

public partial class Global : Node
{
	public static List<string> playerNames = new List<string>{"Sjaak", "Kees"};

	public static List<string> games = new List<string>{"mexen", "hardcore_mexen"};

	public static string current_game = "mexen";
}
