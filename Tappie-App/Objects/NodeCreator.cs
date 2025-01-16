using Godot;
using System;

public static class NodeCreator 
{
	internal static Node getPopUp(string text) {
		// Load the custom scene
        PackedScene scene = (PackedScene)ResourceLoader.Load("res://Windows/CustomNodes/PopUp.tscn");
        
		// Instance the custom scene
		Node popUp = scene.Instantiate();

		Label textLabel = popUp.GetChild(1).GetNode<Label>("Text");

		textLabel.Text = text;

		return popUp;
	}

    internal static Node getKnightPopUp(string name, int amount, bool isGiving) {
        string text;
        if (isGiving) {
            text = name + "\n Mag " + amount + " strafpunt(en) uitdelen!";
        }
        else text = name + "\n Krijgt " + amount + " strafpunt(en)!";
		// Load the custom scene
        PackedScene scene = (PackedScene)ResourceLoader.Load("res://Windows/CustomNodes/KnightPopUp.tscn");
        
		// Instance the custom scene
		Node popUp = scene.Instantiate();

		Label textLabel = popUp.GetChild(1).GetNode<Label>("Text");

		textLabel.Text = text;

		return popUp;
	}

    internal static Node getKnightPopUpEverybodyDrinks() {
        string text = "Iedereen krijgt 1 strafpunt!";
		// Load the custom scene
        PackedScene scene = (PackedScene)ResourceLoader.Load("res://Windows/CustomNodes/KnightPopUp.tscn");
        
		// Instance the custom scene
		Node popUp = scene.Instantiate();

		Label textLabel = popUp.GetChild(1).GetNode<Label>("Text");

		textLabel.Text = text;

		return popUp;
	}

    internal static Node getPunishmentPopUp(string name, int amount, bool isGiving) {
        string text;
        if (isGiving) {
            text = name + "\n Mag " + amount + " strafpunt(en) uitdelen!";
        }
        else text = name + "\n Krijgt " + amount + " strafpunt(en)!";
		// Load the custom scene
        PackedScene scene = (PackedScene)ResourceLoader.Load("res://Windows/CustomNodes/PunishmentPopUp.tscn");
        
		// Instance the custom scene
		Node popUp = scene.Instantiate();

		Label textLabel = popUp.GetChild(1).GetNode<Label>("Text");

		textLabel.Text = text;

		return popUp;
        }

		public static Node getNameInput(int index, string customName = "") {
		// Load the custom scene
        PackedScene scene = (PackedScene)ResourceLoader.Load("res://Windows/CustomNodes/NameInput.tscn");
        
        // Instance the custom scene
        Node nameInput = scene.Instantiate();
        
        // Add the custom object at the second to last position
        nameInput.Name = index+"";
        nameInput.GetChild(0).GetNode<LineEdit>("LineEdit").Text = customName;

		return nameInput;
	}

    internal static Node getDeleteButton()
    {
        // Load the custom scene
        PackedScene myCustomScene = (PackedScene)ResourceLoader.Load("res://Windows/CustomNodes/DeletePlayerInputButton.tscn");
        
        // Instance the custom scene
        return myCustomScene.Instantiate();
    }

    internal static Node getMexenInfo()
    {
        // Load the custom scene
        PackedScene myCustomScene = (PackedScene)ResourceLoader.Load("res://Windows/CustomNodes/MexenInfo.tscn");
        
        // Instance the custom scene
        return myCustomScene.Instantiate();
    }

    internal static Node getHardcoreMexenInfo()
    {
        // Load the custom scene
        PackedScene myCustomScene = (PackedScene)ResourceLoader.Load("res://Windows/CustomNodes/HardcoreMexenInfo.tscn");
        
        // Instance the custom scene
        return myCustomScene.Instantiate();
    }

    internal static Node getAddButton()
    {
        // Load the custom scene
        PackedScene scene = (PackedScene)ResourceLoader.Load("res://Windows/CustomNodes/AddPlayerInputButton.tscn");
        
        // Instance the custom scene
        return scene.Instantiate();
    }

        internal static VBoxContainer getPlayerInputsContainer()
    {
        // Load the custom scene
        PackedScene scene = (PackedScene)ResourceLoader.Load("res://Windows/CustomNodes/PlayerInputsContainer.tscn");
        
        // Instance the custom scene
        return (VBoxContainer)scene.Instantiate();
    }
}
