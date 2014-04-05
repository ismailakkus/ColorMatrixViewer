﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorMatrixViewer
{
	public partial class MatrixBox : Control
	{

		private float[,] _Matrix = null;
		public float[,] Matrix
		{
			get { return _Matrix; }
			set { _Matrix = value; }
		}

		private TextBox[,] textboxes;

		public event EventHandler MatrixChanged;
		protected void OnMatrixChanged()
		{
			var handler = MatrixChanged;
			if (handler != null)
			{
				handler(this, EventArgs.Empty);
			}
		}

		protected override Size DefaultSize { get { return new Size(238, 88); } }
		protected override Size DefaultMaximumSize { get { return DefaultSize; } }
		protected override Size DefaultMinimumSize { get { return DefaultSize; } }
		public override Size MaximumSize { get { return DefaultSize; } }
		public override Size MinimumSize { get { return DefaultSize; } }

		public MatrixBox()
			: base()
		{
			InitializeMatrixTextboxes(this, new Point(0, 0));
		}

		private void InitializeMatrixTextboxes(Control control, Point location)
		{
			textboxes = new TextBox[5, 5];
			const int xSpacing = 47, ySpacing = 17;
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					var newTextBox = new TextBox();
					newTextBox.Parent = control;
					newTextBox.Location = new Point(location.X + i * xSpacing, location.Y + j * ySpacing);
					newTextBox.Width = 50;
					newTextBox.Height = 20;
					newTextBox.TextAlign = HorizontalAlignment.Center;
					newTextBox.KeyPress += (o, e) => { if (e.KeyChar == ',') { e.Handled = true; newTextBox.SelectedText = "."; } };
					newTextBox.TextChanged += (o, e) => { newTextBox.ClearUndo(); OnMatrixChanged(); };
					newTextBox.MouseWheel += (o, e) =>
					{
						decimal parsed = 0; //decimal type for exact decimal rounding
						if (!decimal.TryParse(newTextBox.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out parsed))
							parsed = 0;
						decimal increment = 1;
						if (ModifierKeys == Keys.Control)
						{
							increment = .1m;
						}
						parsed += increment * (e.Delta / (Math.Abs(e.Delta)));
						//10 significan figures
						newTextBox.Text = parsed.ToString("g10", System.Globalization.CultureInfo.InvariantCulture);
					};
					textboxes[j, i] = newTextBox;
				}
			}
		}

		private void ResetMatrix()
		{
			//autoRefresh = false;
			Matrix = new float[5, 5];
			for (int i = 0; i < Matrix.GetLength(0); i++)
			{
				for (int j = 0; j < Matrix.GetLength(1); j++)
				{
					Matrix[i, j] = BuiltinMatrices.Identity[i, j];
				}
			}
			//autoRefresh = true;
		}


		enum RefreshDirection
		{
			FromMatrix,
			FromTextboxes,
		}
		/// <summary>
		/// Returns true if any change was actually made
		/// </summary>
		private bool RefreshMatrixOrTextBoxes(RefreshDirection direction)
		{
			//autoRefresh = false;
			bool different = false;
			switch (direction)
			{
				case RefreshDirection.FromMatrix:
					for (int i = 0; i < 5; i++)
					{
						for (int j = 0; j < 5; j++)
						{
							string text = Matrix[i, j].ToString();
							if (textboxes[i, j].Text != text)
							{
								textboxes[i, j].Text = text;
								different = true;
							}
						}
					}
					break;
				case RefreshDirection.FromTextboxes:
					try
					{
						for (int i = 0; i < 5; i++)
						{
							for (int j = 0; j < 5; j++)
							{
								float parsed = float.Parse(textboxes[i, j].Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
								if (Matrix[i, j] != parsed)
								{
									Matrix[i, j] = parsed;
									different = true;
								}
							}
						}
					}
					catch (Exception)
					{
						//ResetMatrix();
						MessageBox.Show("Invalid matrix!");
					}
					break;
				default:
					throw new Exception("Fuck you!");
			}
			//autoRefresh = true;
			return different;
		}

	}
}
