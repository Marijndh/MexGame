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

	public void SetData(int score, int position, string name, bool last, bool finished)
	{
		_scoreLabel.Text = finished ? score.ToString() : "?";
		_nameLabel.Text = name;

		SetStyleBox(score, last, finished);
		SetImageOrPosition(position, last, finished);
	}

	private void SetStyleBox(int score, bool last, bool finished)
	{
		Color color = Colors.Purple; // Default color for unfinished scores
		if (finished && !last)
		{
			color = GetColorFromScore(score);
		} 
		else if (finished && last)
		{
			color = Colors.Red; // Color for the last unfinished score
		}
		StyleBoxFlat styleBox = new StyleBoxFlat
		{
			BgColor = color,
			BorderColor = Colors.Black,
			BorderWidthBottom = 5,
			BorderWidthLeft = 3,
			BorderWidthRight = 5,
			BorderWidthTop = 3,
			CornerRadiusTopLeft = 20,
			CornerRadiusTopRight = 20,
			CornerRadiusBottomLeft = 20,
			CornerRadiusBottomRight = 20
		};


		AddThemeStyleboxOverride("panel", styleBox);
		QueueRedraw();

	}

	private void SetImageOrPosition(int position, bool last, bool finished)
	{
		if (last && finished)
		{
			_image.Texture = GD.Load<Texture2D>(_imagePath + "last.svg");
		}
		else if (position > 2)
		{
			_positionLabel.Text = position.ToString();
		}
		else
		{
			_image.Texture = GD.Load<Texture2D>(_imagePath + position + ".svg");
		}

		if (_positionLabel.Text == "") _positionLabel.Hide();
	}

	private Color GetColorFromScore(int score)
	{
		int index = Array.IndexOf(ScoreUtils.ScoreRanking, score);
		if (index == -1)
		{
			GD.PrintErr($"Invalid score: {score}");
			return new Color(1, 0, 1); // Magenta for debugging  
		}

		float t = index / (float)(ScoreUtils.ScoreRanking.Length - 1); // 0 = best, 1 = worst  

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
