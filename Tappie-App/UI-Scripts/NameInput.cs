using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class NameInput : Control
{
	private CustomLineEdit _nameInput;
	private TextureButton _deleteButton;
	private NameInputContainer _parent;
	public override void _Ready()
	{	
		_nameInput = GetNode<CustomLineEdit>("LineEdit");
		_deleteButton = GetNode<TextureButton>("Close");
		_deleteButton.Pressed += Close;
		_parent = GetParent<NameInputContainer>();
	}

	private void Close()
	{
		if (_parent != null)
		{
			_parent.RemoveChild(this);
		}
	}

	public void Focus()
	{
		if (_nameInput != null)
		{
			_nameInput.GrabFocus();
		}
	}

	public string GetName()
	{
		return _nameInput.Text;
	}

	public void DisableDeleteButton()
	{
		if (_deleteButton != null)
		{
			_deleteButton.Disabled = true;
			_deleteButton.Visible = false;
		}
	}

	public void EnableDeleteButton()
	{
		if (_deleteButton != null)
		{
			_deleteButton.Disabled = false;
			_deleteButton.Visible = true;
		}
	}
}
