﻿//Copyright (c) 2014 Melvyn Laily
//http://arcanesanctum.net

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.

using System;
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
	public partial class ImageDiff : Control
	{

		private double _SplitterPosition;
		/// <summary>
		/// Percentage
		/// </summary>
		public double SplitterPosition
		{
			get { return _SplitterPosition; }
			set
			{
				if (value < 0) value = 0;
				if (value > 1) value = 1;
				_SplitterPosition = value;
				this.Invalidate();
			}
		}

		public Image FirstImage { get; protected set; }
		public Image SecondImage { get; protected set; }

		public Rectangle ActualImageLocation { get; private set; }

		private bool mouseDownHappened = false;

		public ImageDiff()
			: base()
		{
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.DoubleBuffered = true;
			this.SplitterPosition = .5;
			this.MouseUp += (o, e) => mouseDownHappened = false;
			this.MouseDown += (o, e) => mouseDownHappened = true;
			this.MouseDown += ImageDiff_MouseMove;
			this.MouseMove += ImageDiff_MouseMove;
			LoadDefaultImage();
		}


		public void LoadDefaultImage()
		{
			this.SetImages(ColorMatrixViewer.Properties.Resources.mire, null);
		}

		public void SetImages(Image first = null, Image second = null)
		{
			bool firstWasNull = first == null;
			bool secondWasNull = second == null;
			if (firstWasNull && secondWasNull)
			{
				LoadDefaultImage();
			}

			//At all time, if one of the images is set, the other is set too

			if (first == null) first = this.FirstImage;
			if (second == null) second = this.SecondImage;

			if (first == null)
			{
				first = new Bitmap(second);
			}
			if (second == null)
			{
				second = new Bitmap(first);
			}

			//And both images always have the same dimensions

			if (first.Width != second.Width || first.Height != second.Height)
			{
				if (firstWasNull)
				{
					first = new Bitmap(second);
				}
				else if (secondWasNull)
				{
					second = new Bitmap(first);
				}
				else
				{
					throw new ArgumentException("The FirstImage and SecondImage must have the same dimensions!");
				}
			}

			this.FirstImage = first;
			this.SecondImage = second;

			this.Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.Clear(SystemColors.ControlDark);

			int thisWidth = this.Width;
			int thisHeight = this.Height;

			if (this.FirstImage != null)
			{
				//both images have the same dimensions
				int imageWidth = this.FirstImage.Width;
				int imageHeight = this.FirstImage.Height;

				double widthRatio = (double)thisWidth / imageWidth;
				double heightRatio = (double)thisHeight / imageHeight;

				//the smallest of the two so that the image always fits
				double ratio = widthRatio <= heightRatio ? widthRatio : heightRatio;

				int width = Round(ratio * imageWidth);
				int height = Round(ratio * imageHeight);

				//center the image
				int x = Round((thisWidth - width) / 2f);
				int y = Round((thisHeight - height) / 2f);

				ActualImageLocation = new Rectangle(x, y, width, height);

				var firstSrcRect = new Rectangle(0, 0, Round(imageWidth * this.SplitterPosition), imageHeight);
				var firstDestRect = new Rectangle(x, y, Round(width * this.SplitterPosition), height);

				var secondSrcRect = new Rectangle(firstSrcRect.Width, 0, imageWidth - Round(imageWidth * this.SplitterPosition), imageHeight);
				var secondDestRect = new Rectangle(x + firstDestRect.Width, y, width - Round(width * this.SplitterPosition), height);

				e.Graphics.DrawImage(this.FirstImage, firstDestRect, firstSrcRect, GraphicsUnit.Pixel);
				e.Graphics.DrawImage(this.SecondImage, secondDestRect, secondSrcRect, GraphicsUnit.Pixel);
			}
		}

		void ImageDiff_MouseMove(object sender, MouseEventArgs e)
		{
			// do not continue if the initial mouse down did not happen on the control.
			// prevent incorrect separator moves when maximizing the window
			if (!mouseDownHappened) return;
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				double x = e.X;//, y = e.Y;
				if (ActualImageLocation == null || ActualImageLocation.Width == 0) return;
				else
				{
					x -= ActualImageLocation.X;
				}

				this.SplitterPosition = x / ActualImageLocation.Width;
			}
		}

		private static int Round(double d)
		{
			return (int)Math.Round(d, MidpointRounding.AwayFromZero);
		}

	}
}
