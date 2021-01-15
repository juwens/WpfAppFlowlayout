using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfAppFlowlayout
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly Brush bb = Brushes.Black;
        readonly Thickness th = new Thickness(2);

        public MainWindow()
        {
            InitializeComponent();

            var faker = new Bogus.Faker();
            for (int i = 0; i < 100; i++)
            {
                var row = new TableRow();
                row.Cells.Add(CreateCell(faker.Name.FirstName()));
                row.Cells.Add(CreateCell(faker.Name.LastName()));
                row.Cells.Add(CreateCell(faker.Address.StateAbbr()));
                row.Cells.Add(CreateCell(faker.Address.City()));

                tbl.RowGroups[0].Rows.Add(row);
            }

            FlowToXps.SaveAsXps("c:/temp/foo.xps", flowDoc);
        }

        readonly Thickness _cellBorderThickness = new Thickness(1, 1, 0, 0);
        private TableCell CreateCell(string text)
        {
            var para = new Paragraph(new Run(text)) { BorderBrush = bb, BorderThickness = _cellBorderThickness };
            return new TableCell(para);
        }
    }
}
