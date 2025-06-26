using Godot;

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

		this.Set("custom_styles/panel", styleBox);
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

	private Color GetColorFromScore(int score)
	{
		// Clamp score to valid range
		score = Mathf.Clamp(score, 21, 600);

		// Normalize the score to a 0–1 range
		float t = (score - 21f) / (600f - 21f);

		if (t < 0.33f)
		{
			// Red to Yellow
			float localT = t / 0.33f;
			return new Color(1, localT, 0); // (Red to Yellow)
		}
		else if (t < 0.66f)
		{
			// Yellow to Green
			float localT = (t - 0.33f) / 0.33f;
			return new Color(1 - localT, 1, 0); // (Yellow to Green)
		}
		else
		{
			// Green to Blue
			float localT = (t - 0.66f) / 0.34f;
			return new Color(0, 1 - localT, localT); // (Green to Blue)
		}
	}
}
