using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace Draw.src.Model
{
	class CircleShape : Shape
	{
		#region Constructor

		public CircleShape(RectangleF rect) : base(rect)
		{
		}

		public CircleShape(RectangleShape rectangle) : base(rectangle)
		{
		}

		#endregion

		/// <summary>
		/// Проверка за принадлежност на точка point към правоъгълника.
		/// В случая на правоъгълник този метод може да не бъде пренаписван, защото
		/// Реализацията съвпада с тази на абстрактния клас Shape, който проверява
		/// дали точката е в обхващащия правоъгълник на елемента (а той съвпада с
		/// елемента в този случай).
		/// </summary>
		public override bool Contains(PointF point)
		{

			if (base.Contains(point))
			{ // Проверка дали е в обекта само, ако точката е в обхващащия правоъгълник.
			  // В случая на правоъгълник - директно връщаме true

				double x = base.Location.X + (base.Width / 2);
				double y = base.Location.Y + (base.Height / 2);

				double temp1 = Math.Pow(point.X - x, 2);
				double temp2 = Math.Pow(point.Y - y, 2);
				double rx = Math.Pow(base.Width / 2, 2);
				double ry = Math.Pow(base.Height / 2, 2);

				double temp3 = temp1 / rx + temp2 / ry;
				if (temp3 <= 1)
					return true;
				else return false;
			}
			else
				// Ако не е в обхващащия правоъгълник, то неможе да е в обекта и => false
				return false;
		}

		/// <summary>
		/// Частта, визуализираща конкретния примитив.
		/// </summary>
		public override void DrawSelf(Graphics grfx)
		{
			base.DrawSelf(grfx);
			GraphicsState state = grfx.Save();

			Matrix m = grfx.Transform.Clone();
			m.Multiply(TransformationMatrix);

			grfx.Transform = m;

			Pen pen = new Pen(StrokeColor);
			pen.Width = LineWidth;

			grfx.FillEllipse(new SolidBrush(FillColor), Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
			grfx.DrawEllipse(pen, Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);

			grfx.DrawLine(pen, Rectangle.X, Rectangle.Y, Rectangle.X, Rectangle.Y + Rectangle.Height);
			grfx.DrawLine(pen, Rectangle.X + Rectangle.Width, Rectangle.Y, Rectangle.X + Rectangle.Width, Rectangle.Y + Rectangle.Height);
			grfx.DrawLine(pen, Rectangle.X, Rectangle.Y + Rectangle.Height / 2, Rectangle.X + Rectangle.Width, Rectangle.Y + Rectangle.Height / 2);


			grfx.Restore(state);
		}
	}
}