using StockApp.BaseClasses.Zielschiessen;
using StockApp.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace StockApp.Views
{
    /// <summary>
    /// Interaction logic for ZielSpielerView.xaml
    /// </summary>
    public partial class ZielSpielerView : UserControl
    {
        int prevRowIndex = -1;

        public ZielSpielerView()
        {
            InitializeComponent();

            this.dgStartliste.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(dgStartliste_PreviewMouseLeftButtonDown);
            this.dgStartliste.Drop += new DragEventHandler(dgStartliste_Drop);
        }

        public delegate Point GetDragDropPosition(IInputElement theElement);

        private bool IsTheMouseOnTargetRow(Visual theTarget, GetDragDropPosition pos)
        {
            Rect posBounds = VisualTreeHelper.GetDescendantBounds(theTarget);
            Point theMousePos = pos((IInputElement)theTarget);
            return posBounds.Contains(theMousePos);
        }

        private DataGridRow GetDataGridRowItem(int index)
        {
            if (dgStartliste.ItemContainerGenerator.Status
                != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                return null;

            return dgStartliste.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
        }
        private int GetDatagridItemCurrentRowIndex(GetDragDropPosition pos)
        {
            int curIndex = -1;
            for (int i = 0; i < dgStartliste.Items.Count; i++)
            {
                DataGridRow itm = GetDataGridRowItem(i);
                if (IsTheMouseOnTargetRow(itm, pos))
                {
                    curIndex = i;
                    break;
                }
            }
            return curIndex;
        }

        void dgStartliste_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            prevRowIndex = GetDatagridItemCurrentRowIndex(e.GetPosition);
            if (prevRowIndex < 0)
                return;
            dgStartliste.SelectedIndex = prevRowIndex;


            if (!(dgStartliste.Items[prevRowIndex] is Teilnehmer selectedTln))
                return;

            DragDropEffects dragDropEffects = DragDropEffects.Move;
            if (DragDrop.DoDragDrop(dgStartliste, selectedTln, dragDropEffects) !=
                DragDropEffects.None)
            {
                dgStartliste.SelectedItem = selectedTln;
            }

        }

        void dgStartliste_Drop(object sender, DragEventArgs e)
        {
            if (prevRowIndex < 0)
                return;

            int index = this.GetDatagridItemCurrentRowIndex(e.GetPosition);

            if (index < 0)
                return;

            if (index == prevRowIndex)
                return;

            if (index == dgStartliste.Items.Count - 1)
            {
                //MessageBox.Show("this row-index cannot be used for drag drop ");
                // return;
            }

            ((sender as DataGrid).DataContext as ZielSpielerViewModel).MoveTeilnehmer(prevRowIndex, index);

        }

    }
}

