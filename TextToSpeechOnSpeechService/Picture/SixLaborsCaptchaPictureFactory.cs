using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using TextToSpeechOnSpeechService.Infrastructure;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing;

namespace TextToSpeechOnSpeechService
{
    public class SixLaborsCaptchaPictureFactory : ICaptchaPictureFactory
    {
        private readonly IConfiguration configuration;

        public SixLaborsCaptchaPictureFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public MemoryStream GetCaptchPictureStream(string text)
        {
            var config = configuration.GetSection("Captcha").Get<CaptchaConfig>();

            using (var image = new Image<Rgba32>(config.ImageWidth, config.ImageHeight))
            {
                FontCollection collection = new FontCollection();
                FontFamily fontFamily = collection.Install(config.FontFamilyPath);
                Font font = fontFamily.CreateFont(config.FontSize);
                var fontBrush = new SolidBrush(Color.Gray);
                var stringStartingPoint = new Point(0, 20);

                var rectangle = new RectangleF(0, 0, image.Width, image.Height);
                image.Mutate(i => i.Fill(new SolidBrush(Color.White)));


                if (config.DisturbString)
                {
                    DrawDisturbedString(image, text, font, fontBrush, stringStartingPoint, config);
                }
                else
                {
                    DrawStringContinuosly(image, text, font, fontBrush, stringStartingPoint);
                }

                if (config.DisturbtionLines)
                {
                    AddDisturbtionLines(image, config);
                }

                MemoryStream ms = new MemoryStream();
                image.SaveAsPng(ms);
                ms.Position = 0;
                return ms;
            }
        }

        private void AddDisturbtionLines(Image image, CaptchaConfig config)
        {
            var linesPen = new Pen(Color.Gray, config.DisturbtionLinesWidth);
            var quaterHeight = config.ImageHeight / (config.DisturbtionLinesCount + 1);

            for (int i = 1; i <= config.DisturbtionLinesCount; i++)
            {
                image.Mutate(x => x.DrawLines(linesPen, new Point(0, quaterHeight * i), new Point(config.ImageWidth, quaterHeight * i)));
            }
        }

        private void DrawStringContinuosly(Image image, string text, Font font, IBrush fontBrush, Point startingPoint)
        {
            image.Mutate(x => x.DrawText(text, font, fontBrush, startingPoint));

        }

        private void DrawDisturbedString(Image image, string text, Font baseFont, IBrush fontBrush, Point startingPoint, CaptchaConfig config)
        {
            var textSplitted = text.ToCharArray();
            var random = new Random();
            int yOffset = 0;

            Point letterPoint = startingPoint;
            foreach (var letter in textSplitted)
            {
                var stringToDraw = letter.ToString();

                var letterFontSize = Math.Max(1, baseFont.Size + random.Next(-config.DisturbStringFontResizeFactor, config.DisturbStringFontResizeFactor));
                var letterFont = new Font(baseFont.Family, letterFontSize);


                image.Mutate(x =>
                {
                     var degrees = random.Next(-config.DisturbStringRotateFactor, config.DisturbStringRotateFactor);
                    // x.Rotate(degrees);
                    x.DrawText(stringToDraw, letterFont, fontBrush, letterPoint).Rotate(degrees);
                    // x.Rotate(-degrees);
                });
                RendererOptions options = new RendererOptions(letterFont, dpi: 72)
                {
                    ApplyKerning = true,
                };
                FontRectangle rect = TextMeasurer.Measure(stringToDraw, options);
                var dividedHeight = (int)rect.Height / config.DisturbStringHeightDivideFactor;
                yOffset *= -1;//back to normal position
                yOffset += random.Next(-dividedHeight, dividedHeight);

                var xOffset = (int)rect.Width - ((int)rect.Width / config.DisturbStringWidthDivideFactor);
                letterPoint.Offset(xOffset, yOffset);

            }
        }
    }
}
