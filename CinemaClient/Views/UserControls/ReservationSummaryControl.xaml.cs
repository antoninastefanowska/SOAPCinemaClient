using CinemaClient.CinemaService;
using Microsoft.Win32;
using PdfSharp.Xps;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace CinemaClient
{
    public partial class ReservationSummaryControl : UserControl
    {
        public ReservationWindow Context { get; set; }
        public ReservationItem ReservationItem { get; set; }

        public ReservationSummaryControl()
        {
            InitializeComponent();
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            MainGrid.DataContext = ReservationItem;
        }

        private void GenerateDocumentFileButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "reservation";
            dialog.DefaultExt = ".pdf";
            dialog.Filter = "PDF Documents (.pdf)|*.pdf";

            if (dialog.ShowDialog() == true)
            {
                Context.StartLoading();
                new Task(() => CreateDocumentFile(dialog.FileName, GenerateDocument())).Start();
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void CreateDocumentFile(string filepath, FlowDocument document)
        {
            const string temporaryFilePath = "tmp.xps";       
            DocumentPaginator paginator = (document as IDocumentPaginatorSource).DocumentPaginator;
                        
            XpsDocument outputFile = new XpsDocument(temporaryFilePath, FileAccess.Write);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(outputFile);
            writer.Write(paginator);
            outputFile.Close();
            
            XpsConverter.Convert(temporaryFilePath, filepath, 1);
            File.Delete(temporaryFilePath);

            Context.StopLoading();
        }

        private FlowDocument GenerateDocument()
        {
            FlowDocument document = new FlowDocument();
            Paragraph paragraph = new Paragraph(new Run("Podsumowanie"));
            paragraph.FontSize = 36;
            document.Blocks.Add(paragraph);

            paragraph = new Paragraph();
            string date = new EpochToDateStringConverter().Convert(ReservationItem.Showing.DateEpoch, null, null, null) as string;
            paragraph.Inlines.Add(new Bold(new Run("Dzień: ")));
            paragraph.Inlines.Add(date);
            document.Blocks.Add(paragraph);

            paragraph = new Paragraph();
            string time = new EpochToTimeStringConverter().Convert(ReservationItem.Showing.DateEpoch, null, null, null) as string;
            paragraph.Inlines.Add(new Bold(new Run("Godzina: ")));
            paragraph.Inlines.Add(time);
            document.Blocks.Add(paragraph);

            paragraph = new Paragraph();
            paragraph.Inlines.Add(new Bold(new Run("Film: ")));
            paragraph.Inlines.Add(ReservationItem.Showing.FilmTitle);
            document.Blocks.Add(paragraph);

            paragraph = new Paragraph();
            paragraph.Inlines.Add(new Bold(new Run("Zarezerwowane miejsca: ")));
            document.Blocks.Add(paragraph);

            List list = new List();
            ListItem listItem;
            foreach (Seat seat in ReservationItem.Reservation.Seats)
            {
                paragraph = new Paragraph();
                paragraph.Inlines.Add(new Bold(new Run("Rząd: ")));
                paragraph.Inlines.Add((seat.Row + 1).ToString());
                paragraph.Inlines.Add(new Bold(new Run(" Numer: ")));
                paragraph.Inlines.Add((seat.Column + 1).ToString());
                listItem = new ListItem(paragraph);
                list.ListItems.Add(listItem);
            }
            document.Blocks.Add(list);
            return document;         
        }
    }
}
