using Microsoft.Win32;
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

            for (int page = 0; page < 3; page++)
            {
                var tbl = CreateTable();
                for (int i = 0; i < 52; i++)
                {
                    var row = new TableRow();
                    row.Cells.Add(CreateCell($"{i}: {faker.Name.FirstName()}"));
                    row.Cells.Add(CreateCell(faker.Name.LastName()));
                    row.Cells.Add(CreateCell(faker.Address.StateAbbr()));
                    row.Cells.Add(CreateCell(faker.Address.City()));

                    tbl.RowGroups[0].Rows.Add(row);
                }
                flowDoc.Blocks.Add(new Section(tbl) { BreakPageBefore = true });
            }
        }

        readonly Thickness _cellBorderThickness = new Thickness(1, 1, 0, 0);
        private TableCell CreateCell(string text)
        {
            var para = new Paragraph(new Run(text)) { BorderBrush = bb, BorderThickness = _cellBorderThickness };
            return new TableCell(para);
        }

        private Table CreateTable()
        {
            var tbl = new Table()
            {
                CellSpacing = 0,
                BorderThickness = new Thickness(0, 0, 1, 1),
                BorderBrush = bb,
                BreakPageBefore = true,
            };
            tbl.RowGroups.Add(new TableRowGroup());
            return tbl;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var fileDlg = new SaveFileDialog();
            fileDlg.Filter = "*.xps|*.xps";
            fileDlg.FileName = "c:\\temp\\foo.xps";
            if (fileDlg.ShowDialog() == true)
            {
                FlowToXps.SaveAsXps(fileDlg.FileName, flowDoc);
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            DoThePrint(flowDoc);
        }

        /// <summary>
        /// from https://stackoverflow.com/a/853461/534812
        /// </summary>
        /// <param name="document"></param>
        private void DoThePrint(FlowDocument document)
        {
            // Clone the source document's content into a new FlowDocument.
            // This is because the pagination for the printer needs to be
            // done differently than the pagination for the displayed page.
            // We print the copy, rather that the original FlowDocument.
            System.IO.MemoryStream s = new System.IO.MemoryStream();
            TextRange source = new TextRange(document.ContentStart, document.ContentEnd);
            source.Save(s, DataFormats.Xaml);
            FlowDocument copy = new FlowDocument();
            TextRange dest = new TextRange(copy.ContentStart, copy.ContentEnd);
            dest.Load(s, DataFormats.Xaml);

            // Create a XpsDocumentWriter object, implicitly opening a Windows common print dialog,
            // and allowing the user to select a printer.

            // get information about the dimensions of the seleted printer+media.
            System.Printing.PrintDocumentImageableArea ia = null;
            System.Windows.Xps.XpsDocumentWriter docWriter = System.Printing.PrintQueue.CreateXpsDocumentWriter(ref ia);

            if (docWriter != null && ia != null)
            {
                DocumentPaginator paginator = ((IDocumentPaginatorSource)copy).DocumentPaginator;

                // Change the PageSize and PagePadding for the document to match the CanvasSize for the printer device.
                paginator.PageSize = new Size(ia.MediaSizeWidth, ia.MediaSizeHeight);
                Thickness t = new Thickness(20, 20, 20, 20 );  // copy.PagePadding;
                copy.PagePadding = new Thickness(
                                 Math.Max(ia.OriginWidth, t.Left),
                                   Math.Max(ia.OriginHeight, t.Top),
                                   Math.Max(ia.MediaSizeWidth - (ia.OriginWidth + ia.ExtentWidth), t.Right),
                                   Math.Max(ia.MediaSizeHeight - (ia.OriginHeight + ia.ExtentHeight), t.Bottom));

                copy.ColumnWidth = double.PositiveInfinity;
                //copy.PageWidth = 528; // allow the page to be the natural with of the output device

                // Send content to the printer.
                docWriter.Write(paginator);
            }

        }
    }
}
