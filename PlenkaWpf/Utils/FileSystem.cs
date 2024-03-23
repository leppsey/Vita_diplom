using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

using PlenkaAPI;
using PlenkaAPI.Task1;


namespace PlenkaWpf.Utils
{
    /// <summary>
    ///     Класс для работы с файлами
    /// </summary>
    internal static class FileSystem

    {
        private static Image CreateAndFitImage(byte[] bitmap, Document document)
        {
            var image = new Image(ImageDataFactory.Create(bitmap)).SetTextAlignment(TextAlignment.CENTER);
            FitImageToDocument(image, document);

            return image;
        }

        private static void FitImageToDocument(Image image, Document document)
        {
            var widthscaler =
                (document.GetPageEffectiveArea(PageSize.A4).GetWidth() - document.GetLeftMargin() -
                 document.GetRightMargin()) / image.GetImageWidth();

            var heighscaler =
                (document.GetPageEffectiveArea(PageSize.A4).GetHeight() - document.GetTopMargin() -
                 document.GetBottomMargin()) / image.GetImageHeight();

            float scaler;

            if (widthscaler < heighscaler)
            {
                scaler = widthscaler;
            }
            else
            {
                scaler = heighscaler;
            }

            image.Scale(scaler, scaler);
        }

        /// <summary>
        ///     Функция экспорта результатов в пдф
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="tempBitmap">График температуры</param>
        /// <param name="nBitMap">График вязкости</param>
        /// <param name="task1MathModel">Результаты расчетов и начальные параметры</param>
        public static void
            ExportPdf(string path, byte[] tempBitmap, byte[] nBitMap, Task1MathClass task1MathModel) // todo Переписать
        {
             var results = task1MathModel.Results;
            // var writer = new PdfWriter(path);
            // var pdf = new PdfDocument(writer);
            // var document = new Document(pdf);
            //
            // //Trace.WriteLine($"-----------------------------------{Directory.GetCurrentDirectory()}");
            // var fontFilename = "../../../resources/fonts/Times_New_Roman.ttf";
            // var font = PdfFontFactory.CreateFont(fontFilename, PdfEncodings.IDENTITY_H);
            //
            // var header = new Paragraph("Отчёт о моделировании неизотермического течения аномально-вязкого материала").SetFont(font)
            //     .SetTextAlignment(TextAlignment.CENTER)
            //     .SetFontSize(20);
            //
            // document.SetFont(font);
            //
            // var tGraphImage = CreateAndFitImage(tempBitmap, document);
            // document.Add(header);
            // document.Add(new Paragraph("Входные данные"));
            //
            // var initialTable = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            //
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph("Тип материала")));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph(mathModel.Cp.MaterialName)));
            //
            // initialTable.AddCell(new Cell(1, 2).Add(new Paragraph("Геометрические параметры канала").SetTextAlignment(TextAlignment.CENTER)));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph("Длина, м")));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph(mathModel.Cp.L.ToString())));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph("Ширина, м")));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph(mathModel.Cp.W.ToString())));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph("Глубина, м")));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph(mathModel.Cp.H.ToString())));
            //
            //
            //
            // initialTable.AddCell(new Cell(1, 2).Add(new Paragraph("Параметры свойств материала").SetTextAlignment(TextAlignment.CENTER)));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph("Плотность кг/м³")));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph(mathModel.Cp.P.ToString())));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph("Удельная теплоёмкость, Дж/(кг·°С)")));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph(mathModel.Cp.C.ToString())));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph("Температура плавления, °С")));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph(mathModel.Cp.T0.ToString())));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph("Температура стеклования, °С")));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph(mathModel.Cp.Tg.ToString())));
            //
            // initialTable.AddCell(new Cell(1, 2).Add(new Paragraph("Режимные параметры процесса").SetTextAlignment(TextAlignment.CENTER)));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph("Скорость движения крышки, м/с")));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph(mathModel.Cp.Vu.ToString())));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph("Температура крышки, °С")));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph(mathModel.Cp.Tu.ToString())));
            //
            // initialTable.AddCell(new Cell(1, 2).Add(new Paragraph("Параметры математической модели").SetTextAlignment(TextAlignment.CENTER)));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph("Шаг расчёта по длине канала, м")));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph(mathModel.Cp.Step.ToString())));
            //
            // initialTable.AddCell(new Cell(1, 2).Add(new Paragraph("Эмпирические коэффициенты математической модели")
            //                                             .SetTextAlignment(TextAlignment.CENTER)));
            //
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph("Коэффициент консистенции при температуре приведения, Па·сⁿ")));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph(mathModel.Cp.U0.ToString())));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph("Первая константа уравнения ВЛФ при температуре стеклования материала")));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph(mathModel.Cp.Const1.ToString())));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph("Вторая константа уравнения ВЛФ при температуре стеклования материала, °С")));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph(mathModel.Cp.Const2.ToString())));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph("Температура приведения, °С")));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph(mathModel.Cp.Tr.ToString())));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph("Индекс течения материала")));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph(mathModel.Cp.N.ToString())));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph("Коэффициент теплоотдачи от крышки канала к материалу, Вт/(м²·°С)")));
            // initialTable.AddCell(new Cell(1, 1).Add(new Paragraph(mathModel.Cp.Au.ToString())));
            //
            // document.Add(initialTable);
            // document.Add(new AreaBreak());
            //
            // document.Add(new Paragraph("График температуры"));
            // document.Add(tGraphImage);
            //
            // var nGraphImage = CreateAndFitImage(nBitMap, document);
            // document.Add(new AreaBreak());
            // document.Add(new Paragraph("График вязкости"));
            // document.Add(nGraphImage);
            //
            // document.Add(new Paragraph("Критериальные показатели"));
            // document.Add(new Paragraph($"Температура продукта {results.T} °С"));
            // document.Add(new Paragraph($"Вязкость продукта {results.N} Па·с"));
            // document.Add(new Paragraph($"Производительность канала {results.Q} кг/ч"));
            //
            // document.Add(new AreaBreak());
            //
            var resultTable = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();
            resultTable.AddHeaderCell("Время, с");
            resultTable.AddHeaderCell("С1, %");
            resultTable.AddHeaderCell("С2, %");
            resultTable.AddHeaderCell("С3, %");
            resultTable.AddHeaderCell("С4, %");
            resultTable.AddHeaderCell("С5, %");
            resultTable.AddHeaderCell("С6, %");
            resultTable.AddHeaderCell("С7, %");
            
            for (var i = 0; i < results.CordCs.Count; i++)
            {
                resultTable.AddCell(results.CordCs[i].Cord.ToString());
                resultTable.AddCell(results.CordCs[i].C1.ToString());
                resultTable.AddCell(results.CordCs[i].C2.ToString());
                resultTable.AddCell(results.CordCs[i].C3.ToString());
                resultTable.AddCell(results.CordCs[i].C4.ToString());
                resultTable.AddCell(results.CordCs[i].C5.ToString());
                resultTable.AddCell(results.CordCs[i].C6.ToString());
                resultTable.AddCell(results.CordCs[i].C7.ToString());
                resultTable.AddCell(results.CordCs[i].T.ToString());
                
            }
            
            // document.Add(new Paragraph("Таблица параметров состояния"));
            // document.Add(resultTable);
            // document.Close();
        }
    }
}
