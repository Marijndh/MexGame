using Godot;
using System;
using Godot.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.Serialization;

public partial class NameInputContainer : VBoxContainer
{
	private List<NameInput> _nameInputs;
	private TextureButton _addButton;

	private GameManager _gameManager;
	public override void _Ready()
	{
		_gameManager =  GetNode<GameManager>("/root/GameManager");

		_addButton = GetNode<TextureButton>("AddButton");
		_addButton.Pressed += () => AddPlayerInput();

		_nameInputs = GetAllNameInputs();

		List<Player> players = _gameManager.GetPlayers();
		
		for (int i = 0; i < players.Count; i++)
		{
			if (i < _nameInputs.Count)
			{
				_nameInputs[i].SetName(players[i].Name);
			}
			else
			{
				AddPlayerInput(players[i].Name);
			}
		}

		SelectFirstEmpty();
	}

	private void AddPlayerInput(string name = null)
	{
		int index = _nameInputs.Count;

		if (index < 8)
		{
			NameInput nameInput = (NameInput)NodeCreator.CreateNode(
				"NameInput",
				new Godot.Collections.Dictionary<string, Variant> { },
				index.ToString()
			);
			_nameInputs.Add(nameInput);

			AddChild(nameInput);
			MoveChild(nameInput, index);
			nameInput.SetName(name ?? string.Empty);
			nameInput.Focus();

			// If limit is reached, remove add button
			if (_nameInputs.Count == 8 && _addButton.IsInsideTree())
			{
				RemoveChild(_addButton);
			}
		}

		if (_nameInputs.Count == 2)
		{
			_nameInputs[0].EnableDeleteButton();
		}
	}

	public void SelectFirstEmpty()
	{
		foreach (NameInput nameInput in _nameInputs)
		{
			if (string.IsNullOrEmpty(nameInput.GetName()))
			{
				nameInput.Focus();
				return;
			}
		}
	}

	private void AddPLayerInput()
	{
		int index = _nameInputs.Count;

		if (index < 8)
		{
			NameInput nameInput = (NameInput)NodeCreator.CreateNode(
				"NameInput",
				new Godot.Collections.Dictionary<string, Variant>
				{},
				index.ToString()
			);
			_nameInputs.Add( nameInput );

			AddChild(nameInput);
			MoveChild(nameInput, index);

			nameInput.Focus();

			// If limit is reached, remove add button
			if (index == 7)
			{
				RemoveChild(_addButton);
			}
		}

		if (_nameInputs.Count == 2)
		{
			_nameInputs[0].EnableDeleteButton();
		}
	}

	private List<NameInput> GetAllNameInputs()
	{
		List<NameInput> nameInputs = new List<NameInput>();
		for (int i = 0; i < GetChildCount(); i++)
		{
			Node child = GetChild(i);
			if (child is NameInput nameInput)
			{
				nameInputs.Add(nameInput);
			}
		}
		return nameInputs;
	}
	public (List<string> names, int empty) GetNames()
	{
		int emptyCount = 0;
		List<string> names = new List<string>();
		foreach (NameInput nameInput in _nameInputs)
		{
			string name = nameInput.GetName();
			if (!string.IsNullOrEmpty(name))
			{
				names.Add(name);
			}
			else
			{
				emptyCount++;
			}
		}
		return (names, emptyCount);
	}

	public void RemoveChild(NameInput nameInput)
	{
		if (_nameInputs.Contains(nameInput))
		{
			_nameInputs.Remove(nameInput);
			RemoveChild(nameInput);
			nameInput.QueueFree();
			if (_nameInputs.Count < 8 && !_addButton.IsInsideTree())
			{
				AddChild(_addButton);
			}
		}
		if (_nameInputs.Count == 1)
		{
			_nameInputs[0].DisableDeleteButton();
		}
	}
}
