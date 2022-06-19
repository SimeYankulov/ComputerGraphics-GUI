using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace Draw
{
	/// <summary>
	/// Класът правоъгълник е основен примитив, който е наследник на базовия Shape.
	/// </summary>
	[Serializable]
	public class GroupShape : Shape
	{
		#region Constructor
		public GroupShape(RectangleF rect) : base(rect)
		{
		}

		public GroupShape(RectangleShape rectangle) : base(rectangle)
		{
		}

		#endregion

		public List<Shape> SubShapes = new List<Shape>();
		/// <summary>
		/// Проверка за принадлежност на точка point към правоъгълника.
		/// В случая на правоъгълник този метод може да не бъде пренаписван, защото
		/// Реализацията съвпада с тази на абстрактния клас Shape, който проверява
		/// дали точката е в обхващащия правоъгълник на елемента (а той съвпада с
		/// елемента в този случай).
		/// </summary>
		public override bool Contains(PointF point)
		{
			//for every item  in suBShape
			//check if point in item 
			foreach (Shape shape in SubShapes)
			{
				if (shape.Contains(point)) return true;
				else return false;
			}
			return false;

		}
		/// <summary>
		/// Частта, визуализираща конкретния примитив.
		/// </summary>
		public override void DrawSelf(Graphics grfx)
		{
			foreach (Shape item in SubShapes)
			{
				base.DrawSelf(grfx);
				item.DrawSelf(grfx);
			}
		}
		public override PointF Location
		{
			get { return base.Location; }
			set { }
		}
	}
}
