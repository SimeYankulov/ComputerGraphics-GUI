using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Draw
{
	/// <summary>
	/// Върху главната форма е поставен потребителски контрол,
	/// в който се осъществява визуализацията
	/// </summary>
	public partial class MainForm : Form
	{
		/// <summary>
		/// Агрегирания диалогов процесор във формата улеснява манипулацията на модела.
		/// </summary>
		private DialogProcessor dialogProcessor = new DialogProcessor();
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}

		/// <summary>
		/// Изход от програмата. Затваря главната форма, а с това и програмата.
		/// </summary>
		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			Close();
		}
		
		/// <summary>
		/// Събитието, което се прихваща, за да се превизуализира при изменение на модела.
		/// </summary>
		void ViewPortPaint(object sender, PaintEventArgs e)
		{
			dialogProcessor.ReDraw(sender, e);
		}
		
		/// <summary>
		/// Бутон, който поставя на произволно място правоъгълник със зададените размери.
		/// Променя се лентата със състоянието и се инвалидира контрола, в който визуализираме.
		/// </summary>
		void DrawRectangleSpeedButtonClick(object sender, EventArgs e)
		{
			dialogProcessor.AddRandomRectangle();
			
			statusBar.Items[0].Text = "Последно действие: Рисуване на правоъгълник";
			
			viewPort.Invalidate();
		}

		/// <summary>
		/// Прихващане на координатите при натискането на бутон на мишката и проверка (в обратен ред) дали не е
		/// щракнато върху елемент. Ако е така то той се отбелязва като селектиран и започва процес на "влачене".
		/// Промяна на статуса и инвалидиране на контрола, в който визуализираме.
		/// Реализацията се диалогът с потребителя, при който се избира "най-горния" елемент от екрана.
		/// </summary>
		void ViewPortMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			
			if (pickUpSpeedButton.Checked)
			{
				Shape temp = dialogProcessor.ContainsPoint(e.Location);

				if (temp != null)
				{
					if (dialogProcessor.Selection.Contains(temp))
					{
						dialogProcessor.Selection.Remove(temp);
					}
					else
					{
						dialogProcessor.Selection.Add(temp);
					}
				}

				if (dialogProcessor.Selection != null)
				{
					statusBar.Items[0].Text = "Последно действие: Селекция на примитив";
					dialogProcessor.IsDragging = true;
					dialogProcessor.LastLocation = e.Location;
					viewPort.Invalidate();
				}
			}
		}

		/// <summary>
		/// Прихващане на преместването на мишката.
		/// Ако сме в режм на "влачене", то избрания елемент се транслира.
		/// </summary>
		void ViewPortMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (dialogProcessor.IsDragging) {
				if (dialogProcessor.Selection != null) statusBar.Items[0].Text = "Последно действие: Влачене";
				dialogProcessor.TranslateTo(e.Location);
				viewPort.Invalidate();
			}
		}

		/// <summary>
		/// Прихващане на отпускането на бутона на мишката.
		/// Излизаме от режим "влачене".
		/// </summary>
		void ViewPortMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			dialogProcessor.IsDragging = false;
		}

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
			dialogProcessor.AddRandomEllipse();

			statusBar.Items[0].Text = "Последно действие: Рисуване на елипса";

			viewPort.Invalidate();
		}

		private void editButton_Click(object sender, EventArgs e)
		{
			if (line.SelectedItem != null)
			{
				dialogProcessor.ChangeLineWidth(int.Parse(line.SelectedItem.ToString()));
				line.SelectedItem = null;
				this.viewPort.Invalidate();
			}
			if (HeightTF.Text.Length != 0)
			{
				dialogProcessor.ChangeHeight(int.Parse(HeightTF.ToString()));
				HeightTF.Clear();
				this.viewPort.Invalidate();
			}
			if (WidthTF.Text.Length != 0)
			{
				dialogProcessor.ChangeWidth(int.Parse(WidthTF.ToString()));
				WidthTF.Clear();
				this.viewPort.Invalidate();
			}
			if (OpacityTF.Text.Length != 0)
			{
				dialogProcessor.ChangeOpacity(int.Parse(OpacityTF.ToString()));
				OpacityTF.Clear();
				this.viewPort.Invalidate();	
			}
			if (RotateTF.Text.Length != 0)
			{
				dialogProcessor.Rotate(int.Parse(RotateTF.ToString()));
				RotateTF.Clear();
				this.viewPort.Invalidate();
			}
			if (nameTF.Text.Length != 0)
			{
				dialogProcessor.SetName(nameTF.ToString());
				nameTF.Clear();
			}

		}

        private void delButton_Click_1(object sender, EventArgs e)
        {
			dialogProcessor.Delete();
			this.viewPort.Invalidate();
		}

        private void strokeColorSelectButton_Click(object sender, EventArgs e)
        {
			ColorDialog dlg = new ColorDialog();
			var dialogResult = dlg.ShowDialog();

			statusBar.Items[0].Text = "Последно действие: Избор на цвят";

			if (dialogResult == DialogResult.OK)
			{
				this.strokeColorSelectButton.BackColor = dlg.Color;
			}

			if (this.dialogProcessor.Selection != null)
			{
				dialogProcessor.ChangeStrokeColor(dlg.Color);
				this.viewPort.Invalidate();
			}

		}

        private void fillColorSelectButton_Click_1(object sender, EventArgs e)
        {
			ColorDialog dlg = new ColorDialog();
			var dialogResult = dlg.ShowDialog();

			statusBar.Items[0].Text = "Последно действие: Избор на цвят";

			if (dialogResult == DialogResult.OK)
			{
				this.fillColorSelectButton.BackColor = dlg.Color;
			}

			if (this.dialogProcessor.Selection != null)
			{
				dialogProcessor.ChangeColor(dlg.Color);
				this.viewPort.Invalidate();
			}
		}

        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
			dialogProcessor.GroupShapes();
			viewPort.Invalidate();
		}

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
			if (e.Control && e.KeyCode == Keys.C)
			{ 
				dialogProcessor.CopyList = dialogProcessor.Selection;
			}
			else if (e.Control && e.KeyCode == Keys.V)
			{
				foreach(Shape item in dialogProcessor.CopyList)
                {
					//Shape temp = new Shape(item);
				}
				viewPort.Invalidate();
				dialogProcessor.Paste();
			}
			if (e.Control && e.KeyCode == Keys.X)
			{
				dialogProcessor.Cut();
				viewPort.Invalidate();
			}
			if (e.Control && e.KeyCode == Keys.R)
			{
				dialogProcessor.Clear();
				viewPort.Invalidate();
			}
			if (e.Control && e.KeyCode == Keys.S)
			{
				dialogProcessor.WriteShapeListToFile((List<Shape>)dialogProcessor.ShapeList);
			}
			if (e.Control && e.KeyCode == Keys.O)
			{
				dialogProcessor.ShapeList = (List<Shape>)dialogProcessor.LoadShapeList();
				viewPort.Invalidate();
			}
			if (e.Control && e.KeyCode == Keys.E)
			{
				Close();
			}
			if (e.Control && e.KeyCode == Keys.Q)
			{
				setCanvClr();
			}
			if (e.Control && e.KeyCode == Keys.T)
			{
				dialogProcessor.AddRandomRectangle();
				viewPort.Invalidate();
			}
			if (e.Control && e.KeyCode == Keys.Y)
			{
				dialogProcessor.AddRandomEllipse();
				viewPort.Invalidate();
			}
			if (e.Control && e.KeyCode == Keys.U)
			{
				dialogProcessor.AddRandomTriangle();
				viewPort.Invalidate();
			}
			if (e.Control && e.KeyCode == Keys.I) {
				dialogProcessor.AddRandomLine();
				viewPort.Invalidate();
			}
		}

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
			dialogProcessor.ShapeList = (List<Shape>)dialogProcessor.LoadShapeList();
			viewPort.Invalidate();
		}
		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			dialogProcessor.WriteShapeListToFile((List<Shape>)dialogProcessor.ShapeList);
		}

		private void setCanvasColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
			setCanvClr();
		}
		private void setCanvClr()
		{
			ColorDialog dlg = new ColorDialog();
			var dialogResult = dlg.ShowDialog();

			if (dialogResult == DialogResult.OK)
			{
				this.BackColor = dlg.Color;
			}
		}
        private void rectangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
			dialogProcessor.AddRandomRectangle();
			viewPort.Invalidate();
		}
        private void ellipseToolStripMenuItem_Click(object sender, EventArgs e)
        {
			dialogProcessor.AddRandomEllipse();
			viewPort.Invalidate();
		}
        private void clearCanvasToolStripMenuItem_Click(object sender, EventArgs e)
        {
			dialogProcessor.Clear();
			viewPort.Invalidate();
		}

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
			dialogProcessor.Copy();
		}

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
			dialogProcessor.Cut();
			viewPort.Invalidate();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
			dialogProcessor.Paste();
			viewPort.Invalidate();
		}

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
			dialogProcessor.AddRandomTriangle();

			statusBar.Items[0].Text = "Последно действие: Рисуване на триъгълник";

			viewPort.Invalidate();
		}

        private void triangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
			dialogProcessor.AddRandomTriangle();

			statusBar.Items[0].Text = "Последно действие: Рисуване на триъгълник";

			viewPort.Invalidate();
		}

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
			dialogProcessor.AddRandomLine();

			statusBar.Items[0].Text = "Последно действие: Рисуване на линия";

			viewPort.Invalidate();
		}

        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
			dialogProcessor.AddRandomLine();

			statusBar.Items[0].Text = "Последно действие: Рисуване на линия";

			viewPort.Invalidate();
		}

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
			dialogProcessor.AddRandomCircle();

			viewPort.Invalidate();
		}
    }
}

