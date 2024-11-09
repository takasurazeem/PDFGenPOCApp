using DrawnUi.Maui.Draw;
using DrawnUi.Maui.Infrastructure;
using SkiaSharp;

namespace PDFGenPOCApp
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                Super.DisplayException(this, e);
            }
        }
        string GenerateFileName(DateTime timeStamp, string extension)
        {
            var filename = $"DrawnUi_{timeStamp:yyyy-MM-dd_HHmmss}.{extension}";

            return filename;
        }
        private bool _lockLogs;

        private string _BindableText;
        public string BindableText
        {
            get
            {
                return _BindableText;
            }
            set
            {
                if (_BindableText != value)
                {
                    _BindableText = value;
                    OnPropertyChanged();
                }
            }
        }
        async Task CreatePdf(float width)
        {
            //setup our report to print
            BindableText = "This text came from bindings";
            var vendor = "DrawnUI";
            var filename = GenerateFileName(DateTime.Now, "pdf");

            //render and share
            Files.CheckPermissionsAsync(async () =>
            {

                try
                {
                    _lockLogs = true;
                    string fullFilename = null;
                    var subfolder = "Pdf";
                    var scale = 1; //do not change this
                    var destination = new SKRect(0, 0, width, float.PositiveInfinity);
                   
                    //we need a local file to ba saved in order to share it
                    fullFilename = Files.GetFullFilename(filename, StorageType.Cache, subfolder);

                    if (File.Exists(fullFilename))
                    {
                        File.Delete(fullFilename);
                    }

                  
                    using (var ms = new MemoryStream())
                    using (var stream = new SKManagedWStream(ms))
                    {
                        using (var document = SKDocument.CreatePdf(stream, new SKDocumentPdfMetadata
                        {
                            Author = vendor,
                            Producer = vendor,
                            Subject = this.Title
                        }))
                        {
                            using (var canvas = document.BeginPage(400, 500))
                            {
                                var ctx = new SkiaDrawingContext()
                                {
                                    Canvas = canvas,
                                    Width = 400,
                                    Height = 500
                                };


                            }
                            document.EndPage();
                            document.Close();
                        }

                        ms.Position = 0;
                        var content = ms.ToArray();

                        var file = Files.OpenFile(fullFilename, StorageType.Cache, subfolder);

                        // Write the bytes to the FileStream of the FileDescriptor
                        await file.Handler.WriteAsync(content, 0, content.Length);

                        // Ensure all bytes are written to the underlying device
                        await file.Handler.FlushAsync();

                        Files.CloseFile(file, true);
                        await Task.Delay(500);
                    }

                    //can share the file now
                    Files.Share("PDF", new string[] { fullFilename });
                }
                catch (Exception e)
                {
                    Super.Log(e);
                }
                finally
                {
                    _lockLogs = false;
                }
            });
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var status = await Permissions.RequestAsync<Permissions.StorageWrite>();

            if (status == PermissionStatus.Granted)
            {
                CreatePdf(1240);  // A4 page for 150 DPI
            }
            else
            {
                // Handle the case where permission is denied
                Console.WriteLine("Permission denied to access storage.");
            }
        }
    }

}
