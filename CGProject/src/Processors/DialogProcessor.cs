//using Draw.src.Model;
using Draw.src.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace Draw
{
	/// <summary>
	/// Класът, който ще бъде използван при управляване на диалога.
	/// </summary>
	[Serializable]
	public class DialogProcessor : DisplayProcessor
	{
		#region Constructor
		
		public DialogProcessor()
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Избран елемент.
		/// </summary>
		private List<Shape> selection = new List<Shape>();
		public List<Shape> Selection
		{
			get { return selection; }
			set { selection = value; }
		}

		private List<Shape> copyList = new List<Shape>();
		public List<Shape> CopyList
		{
			get { return copyList; }
			set { copyList = value; }
		}

		/// <summary>
		/// Дали в момента диалога е в състояние на "влачене" на избрания елемент.
		/// </summary>
		private bool isDragging;
		public bool IsDragging {
			get { return isDragging; }
			set { isDragging = value; }
		}
		
		/// <summary>
		/// Последна позиция на мишката при "влачене".
		/// Използва се за определяне на вектора на транслация.
		/// </summary>
		private PointF lastLocation;
		public PointF LastLocation {
			get { return lastLocation; }
			set { lastLocation = value; }
		}
		
		#endregion
		
		/// <summary>
		/// Добавя примитив - правоъгълник на произволно място върху клиентската област.
		/// </summary>
		public void AddRandomRectangle()
		{
			Random rnd = new Random();
			int x = rnd.Next(100,1000);
			int y = rnd.Next(100,600);
			
			RectangleShape rect = new RectangleShape(new Rectangle(x,y,100,200));
			rect.FillColor = Color.White;
			rect.StrokeColor = Color.Black;
			ShapeList.Add(rect);
		}
		public void AddRandomEllipse()
		{
			Random rnd = new Random();
			int x = rnd.Next(100, 1000);
			int y = rnd.Next(100, 600);

			EllipseShape ellipse = new EllipseShape(new Rectangle(x, y, 100, 200));

			ellipse.TransformationMatrix.RotateAt(0,
				new PointF(ellipse.Rectangle.X + ellipse.Width / 2, ellipse.Rectangle.Y + ellipse.Height / 2));

			ellipse.FillColor = Color.White;
			ellipse.StrokeColor = Color.Black;

			ShapeList.Add(ellipse);
		}


		public void GroupShapes()
		{ 
			float minx = float.PositiveInfinity;
			float maxx = float.NegativeInfinity;
			float miny = float.PositiveInfinity;
			float maxy = float.NegativeInfinity;

			foreach (Shape shape in selection)
			{
				if (minx > shape.Rectangle.X) minx = shape.Rectangle.X;
				if (maxx < shape.Rectangle.X + shape.Width) maxx = shape.Rectangle.X + shape.Width;
				if (miny > shape.Rectangle.Y) miny = shape.Rectangle.Y;
				if (maxy < shape.Rectangle.Y + shape.Height) maxy = shape.Rectangle.Y + shape.Height;
			}

			RectangleShape rect = new RectangleShape(new Rectangle((int)minx, (int)miny, (int)maxx - (int)minx, (int)maxy - (int)miny));

			GroupShape gs = new GroupShape(rect);
			gs.SubShapes = Selection;

			ShapeList.Add(gs);

			foreach (Shape item in gs.SubShapes)
            {
				ShapeList.Remove(item);
            }

			
		}

		/// <summary>
		/// Проверява дали дадена точка е в елемента.
		/// Обхожда в ред обратен на визуализацията с цел намиране на
		/// "най-горния" елемент т.е. този който виждаме под мишката.
		/// </summary>
		/// <param name="point">Указана точка</param>
		/// <returns>Елемента на изображението, на който принадлежи дадената точка.</returns>
		public Shape ContainsPoint(PointF point)
		{
			for(int i = ShapeList.Count - 1; i >= 0; i--){
				if (ShapeList[i].Contains(point)){
					//ShapeList[i].FillColor = Color.Red;
						
					return ShapeList[i];
				}	
			}
			return null;
		}

		/// <summary>
		/// Транслация на избраният елемент на вектор определен от <paramref name="p>p</paramref>
		/// </summary>
		/// <param name="p">Вектор на транслация.</param>
		public void TranslateTo(PointF p)
		{
			if (selection.Count > 0)
			{

				foreach (Shape item in Selection)
				{
					item.TransformationMatrix.RotateAt(item.Rotate * (-1),
					new PointF(item.Rectangle.X + item.Width / 2, item.Rectangle.Y + item.Height / 2));

					item.Location =
					new PointF(item.Location.X + p.X - lastLocation.X, item.Location.Y + p.Y - lastLocation.Y);

					item.TransformationMatrix.RotateAt(item.Rotate,
					new PointF(item.Rectangle.X + item.Width / 2, item.Rectangle.Y + item.Height / 2));
				}
				lastLocation = p;
				//selection.Location = 
				//new PointF(selection.Location.X + p.X - lastLocation.X, selection.Location.Y + p.Y - lastLocation.Y);
				//lastLocation = p;
			}
		}
		public override void DrawShape(Graphics grfx, Shape item)
		{
			//For every item in subshape visualize
			base.DrawShape(grfx, item);

			if (selection.Contains(item))
			{
				GraphicsState state = grfx.Save();

				Matrix m = grfx.Transform.Clone();
				m.Multiply(item.TransformationMatrix);

				grfx.Transform = m;
				grfx.DrawRectangle(
					new Pen(Color.Red),
					item.Location.X - 3,
					item.Location.Y - 3,
					item.Width + 6,
					item.Height + 6
			
					);
				grfx.Restore(state);
			}
		}

		public virtual void ChangeColor(Color clr)
		{
			foreach (Shape s in selection)
			{
				s.FillColor = clr;
			}
		}

		public virtual void ChangeStrokeColor(Color clr)
		{
			foreach (Shape s in selection)
			{
				s.StrokeColor = clr;
			}
		}

		public virtual void ChangeLineWidth(int temp)
		{
			foreach (Shape s in selection)
			{
				s.LineWidth = temp;
			}
		}

		internal void ChangeHeight(int temp)
		{
			foreach (Shape s in selection)
			{
				s.Height = temp;
			}
		}

		internal void ChangeWidth(int v)
		{
			foreach (Shape s in selection)
			{
				s.Width = v;
			}
		}

		internal void ChangeOpacity(int v)
		{

			foreach (Shape s in selection)
			{
				s.FillColor = Color.FromArgb(v, s.FillColor);
			}
		}

		internal void Delete()
		{
			foreach (Shape s in selection)
			{
				ShapeList.Remove(s);
			}
		}
		internal void Rotate(int v)
		{
			foreach (Shape s in selection)
			{
				s.TransformationMatrix.RotateAt(v,
				new PointF(s.Rectangle.X + s.Width / 2, s.Rectangle.Y + s.Height / 2));
				s.Rotate += v;
			}
		}
		internal void SetName(string v)
		{
			foreach (Shape s in selection)
			{
				s.Name = v;
			}
		}
		internal void Clear()
		{
			ShapeList.Clear();
		}
		public void Copy()
		{
						
		}
		public void Paste()
		{
			foreach (Shape item in CopyList)
				ShapeList.Add(item);
		}
		public void Cut()
		{
			CopyList = Selection;
			foreach (Shape item in Selection)
				ShapeList.Remove(item);
		}
		
		public void WriteShapeListToFile(object obj)
		{
			SaveFileDialog fd = new SaveFileDialog();
			if (fd.ShowDialog() == DialogResult.OK)
			{
				// Instantiate FileStream class
				FileStream fs = new FileStream(fd.FileName, FileMode.Create);
				// Instantiate BinaryFormatter class
				BinaryFormatter bf = new BinaryFormatter();
				// Use the formatter Serialize method to write
				// the ShapeList into the file 
				bf.Serialize(fs, obj);
				//close the opened file
				fs.Close();
			}
		}
		public object LoadShapeList()
		{
			// Instantiate FileStream class
			// Instantiate BinaryFormatter class
			// Create a temp var
			// Use Deserialize method of the BF class cast to list shape and assign temp var
			//ShapeList - clear

			object obj;
			OpenFileDialog fd = new OpenFileDialog();

			IFormatter bf = new BinaryFormatter();

			fd.ShowDialog();
		
			FileStream fs = new FileStream(fd.FileName, FileMode.Open);

			obj = bf.Deserialize(fs);
			
			fs.Close();
			
			return obj;

		}
        internal void AddRandomTriangle()
        {
			Random rnd = new Random();
			int x = rnd.Next(100, 1000);
			int y = rnd.Next(100, 600);

			TriangleShape triangle = new TriangleShape(new Rectangle(x, y, 100, 200));

			triangle.FillColor = Color.White;
			triangle.StrokeColor = Color.Black;

			ShapeList.Add(triangle);
		}
        internal void AddRandomLine()
        {
			Random rnd = new Random();
			int x = rnd.Next(100, 1000);
			int y = rnd.Next(100, 600);

			LineShape line = new LineShape(new Rectangle(x, y, 300, 1));

			line.FillColor = Color.White;
			line.StrokeColor = Color.Black;

			ShapeList.Add(line);
		}

        internal void AddRandomCircle()
        {
			Random rnd = new Random();
			int x = rnd.Next(100, 1000);
			int y = rnd.Next(100, 600);

			CircleShape line = new CircleShape(new Rectangle(x, y, 100, 100));

			line.FillColor = Color.White;
			line.StrokeColor = Color.Black;

			ShapeList.Add(line);
		}
    }

}
