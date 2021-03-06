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
using System.Text;
using System.Runtime.InteropServices;

namespace ColorMatrixViewer
{
	/// <summary>
	/// Store various built in ColorMatrix
	/// </summary>
	public static class BuiltinMatrices
	{
		/// <summary>
		/// no color transformation
		/// </summary>
		public static float[,] Identity { get; private set; }
		/// <summary>
		/// simple colors transformations
		/// </summary>
		public static float[,] Negative { get; private set; }
		public static float[,] GrayScale { get; private set; }
		public static float[,] Sepia { get; private set; }
		public static float[,] Red { get; private set; }
		public static float[,] HueShift180 { get; private set; }

		public static float[,] NegativeGrayScale { get; private set; }
		public static float[,] NegativeSepia { get; private set; }
		public static float[,] NegativeRed { get; private set; }

		/// <summary>
		/// theoretical optimal transfomation (but ugly desaturated pure colors due to "overflows"...)
		/// Many thanks to Tom MacLeod who gave me the idea for these inversion modes
		/// </summary>
		public static float[,] NegativeHueShift180 { get; private set; }
		/// <summary>
		/// high saturation, good pure colors
		/// </summary>
		public static float[,] NegativeHueShift180Variation1 { get; private set; }
		/// <summary>
		/// overall desaturated, yellows and blue plain bad. actually relaxing and very usable
		/// </summary>
		public static float[,] NegativeHueShift180Variation2 { get; private set; }
		/// <summary>
		/// high saturation. yellows and blues plain bad. actually quite readable
		/// </summary>
		public static float[,] NegativeHueShift180Variation3 { get; private set; }
		/// <summary>
		/// //not so readable, good colors (CMY colors a bit desaturated, still more saturated than normal)
		/// </summary>
		public static float[,] NegativeHueShift180Variation4 { get; private set; }

		public static IEnumerable<KeyValuePair<string, float[,]>> All { get; private set; }

		static BuiltinMatrices()
		{
			Identity = new float[,] {
				{  1.0f,  0.0f,  0.0f,  0.0f,  0.0f },
				{  0.0f,  1.0f,  0.0f,  0.0f,  0.0f },
				{  0.0f,  0.0f,  1.0f,  0.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
			};
			Negative = new float[,] {
				{ -1.0f,  0.0f,  0.0f,  0.0f,  0.0f },
				{  0.0f, -1.0f,  0.0f,  0.0f,  0.0f },
				{  0.0f,  0.0f, -1.0f,  0.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
				{  1.0f,  1.0f,  1.0f,  0.0f,  1.0f }
			};
			GrayScale = new float[,] {
				{  0.3f,  0.3f,  0.3f,  0.0f,  0.0f },
				{  0.6f,  0.6f,  0.6f,  0.0f,  0.0f },
				{  0.1f,  0.1f,  0.1f,  0.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
			};
			NegativeGrayScale = Transform.Multiply(Negative, GrayScale);
			Red = new float[,] {
				{  1.0f,  0.0f,  0.0f,  0.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
			};
			Red = Transform.Multiply(GrayScale, Red);
			NegativeRed = Transform.Multiply(NegativeGrayScale, Red);
			Sepia = new float[,] {
				{ .393f, .349f, .272f, 0.0f, 0.0f},
				{ .769f, .686f, .534f, 0.0f, 0.0f},
				{ .189f, .168f, .131f, 0.0f, 0.0f},
				{  0.0f,  0.0f,  0.0f, 1.0f, 0.0f},
				{  0.0f,  0.0f,  0.0f, 0.0f, 1.0f}
			};
			NegativeSepia = Transform.Multiply(Negative, Sepia);
			HueShift180 = new float[,] {
				{ -0.3333333f,  0.6666667f,  0.6666667f, 0.0f, 0.0f },
				{  0.6666667f, -0.3333333f,  0.6666667f, 0.0f, 0.0f },
				{  0.6666667f,  0.6666667f, -0.3333333f, 0.0f, 0.0f },
				{  0.0f,              0.0f,        0.0f, 1.0f, 0.0f },
				{  0.0f,              0.0f,        0.0f, 0.0f, 1.0f }
			};
			NegativeHueShift180 = Transform.Multiply(Negative, HueShift180);
			NegativeHueShift180Variation1 = new float[,] {
				//most simple working method for shifting hue 180deg.
				{  1.0f, -1.0f, -1.0f, 0.0f, 0.0f },
				{ -1.0f,  1.0f, -1.0f, 0.0f, 0.0f },
				{ -1.0f, -1.0f,  1.0f, 0.0f, 0.0f },
				{  0.0f,  0.0f,  0.0f, 1.0f, 0.0f },
				{  1.0f,  1.0f,  1.0f, 0.0f, 1.0f }
			};
			NegativeHueShift180Variation2 = new float[,] {
				//generated with QColorMatrix http://www.codeguru.com/Cpp/G-M/gdi/gdi/article.php/c3667
				{  0.39f, -0.62f, -0.62f, 0.0f, 0.0f },
				{ -1.21f, -0.22f, -1.22f, 0.0f, 0.0f },
				{ -0.16f, -0.16f,  0.84f, 0.0f, 0.0f },
				{   0.0f,   0.0f,   0.0f, 1.0f, 0.0f },
				{   1.0f,   1.0f,   1.0f, 0.0f, 1.0f }
			};
			NegativeHueShift180Variation3 = new float[,] {
				{     1.089508f,   -0.9326327f, -0.932633042f,  0.0f,  0.0f },
				{  -1.81771779f,    0.1683074f,  -1.84169245f,  0.0f,  0.0f },
				{ -0.244589478f, -0.247815639f,    1.7621845f,  0.0f,  0.0f },
				{          0.0f,          0.0f,          0.0f,  1.0f,  0.0f },
				{          1.0f,          1.0f,          1.0f,  0.0f,  1.0f }
			};
			NegativeHueShift180Variation4 = new float[,] {
				{  0.50f, -0.78f, -0.78f, 0.0f, 0.0f },
				{ -0.56f,  0.72f, -0.56f, 0.0f, 0.0f },
				{ -0.94f, -0.94f,  0.34f, 0.0f, 0.0f },
				{   0.0f,   0.0f,   0.0f, 1.0f, 0.0f },
				{   1.0f,   1.0f,   1.0f, 0.0f, 1.0f }
			};

			List<KeyValuePair<string, float[,]>> all = new List<KeyValuePair<string, float[,]>>();
			foreach (var prop in typeof(BuiltinMatrices).GetProperties())
			{
				if (prop.Name != "All")
				{
					all.Add(new KeyValuePair<string, float[,]>(prop.Name, (float[,])prop.GetValue(null)));
				}
			}
			All = all;
		}
	}
}
