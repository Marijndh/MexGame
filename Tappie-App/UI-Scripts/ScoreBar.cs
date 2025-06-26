using Godot;
using System;

public partial class ScoreBar : Panel
{
	private Label _scoreLabel;
	private Label _nameLabel;
	private Label _positionLabel;
	private TextureRect _image;

	private string _imagePath = "res://Assets/Sprites/Score/";
	public override void _Ready()
	{
		_scoreLabel = GetNode<Label>("Score");
		_nameLabel = GetNode<Label>("Name");
		_positionLabel = GetNode<Label>("Position");
		_image = GetNode<TextureRect>("Image");
	}

	public void SetData(int score, int position, string name, bool isLast)
	{
		_scoreLabel.Text = score.ToString();
		_nameLabel.Text = name;

		SetStyleBox(score);
		SetImageOrPosition(position, isLast);
	}

	private void SetStyleBox(int score)
	{
		Color color = GetColorFromScore(score);
		StyleBoxFlat styleBox = new();
		styleBox.CornerDetail = 10;
		styleBox.BgColor = color;
		styleBox.SetBorderWidthAll(2);
		styleBox.BorderColor = Colors.Black;


		this.Set("panel", styleBox);
		QueueRedraw();

	}

	private void SetImageOrPosition(int position, bool last)
	{
		if (last)
		{
			_image.Texture = GD.Load<Texture2D>(_imagePath + "last.png");
		}
		else if (position > 2)
		{
			_positionLabel.Text = position.ToString();
		}
		else
		{
			_image.Texture = GD.Load<Texture2D>(_imagePath + position + ".png");
		}

		if (_positionLabel.Text == "") _positionLabel.Hide();
	}

	private static readonly int[] ScoreRanking = new int[]
	{
		21,
		600, 500, 400, 300, 200, 100,
		65, 64, 63, 62, 61,
		54, 53, 52, 51,
		43, 42, 41,
		32, 31
	};

private Color GetColorFromScore(int score)
	{
		int index = Array.IndexOf(ScoreRanking, score);
		if (index == -1)
		{
			GD.PrintErr($"Invalid score: {score}");
			return new Color(1, 0, 1); // Magenta for debugging  
		}

		float t = index / (float)(ScoreRanking.Length - 1); // 0 = best, 1 = worst  

		if (t < 0.5f)
		{
			// Green to Yellow  
			float localT = t / 0.5f;
			return new Color(1f * localT, 1f, 0f);
		}
		else
		{
			// Yellow to Red  
			float localT = (t - 0.5f) / 0.5f;
			return new Color(1f, 1f - localT, 0f);
		}
	}

}
