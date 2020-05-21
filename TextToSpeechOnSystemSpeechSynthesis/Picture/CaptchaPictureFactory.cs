using Microsoft.Extensions.Configuration;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using TextToSpeechOnSystemSpeechSynthesis.Infrastructure;

namespace TextToSpeechOnSystemSpeechSynthesis
{
    public class CaptchaPictureFactory : ICaptchaPictureFactory
    {
        private readonly IConfiguration configuration;

        public CaptchaPictureFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public MemoryStream GetCaptchPictureStream(string text)
        {
            var config = configuration.GetSection("Captcha").Get<CaptchaConfig>();

            using (Bitmap bitmap = new Bitmap(config.ImageWidth, config.ImageHeight, PixelFormat.Format32bppArgb))
            {
                var font = new Font(config.FontFamilyName, config.FontSize, config.FontStyle);
                var fontBrush = new SolidBrush(Color.Gray);
                var stringStartingPoint = new Point(0, 20);

                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, config.ImageWidth, config.ImageHeight);

                if (config.DisturbString)
                {
                    DrawDisturbedString(graphics, text, font, fontBrush, stringStartingPoint, config);
                }
                else
                {
                    DrawStringContinuosly(graphics, text, font, fontBrush, stringStartingPoint);
                }

                if (config.DisturbtionLines)
                {
                    AddDisturbtionLines(graphics, config);
                }

                MemoryStream ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                return ms;
            }
        }

        private void AddDisturbtionLines(Graphics graphics, CaptchaConfig config)
        {
            var linesPen = new Pen(Color.Gray, config.DisturbtionLinesWidth);
            var quaterHeight = config.ImageHeight / (config.DisturbtionLinesCount + 1);

            for (int i = 1; i <= config.DisturbtionLinesCount; i++)
            {
                graphics.DrawLine(linesPen, new Point(0, quaterHeight * i), new Point(config.ImageWidth, quaterHeight * i));
            }
        }

        private void DrawStringContinuosly(Graphics graphics, string text, Font font, Brush fontBrush, Point startingPoint)
        {
            graphics.DrawString(text, font, fontBrush, startingPoint);
        }

        private void DrawDisturbedString(Graphics graphics, string text, Font baseFont, Brush fontBrush, Point startingPoint, CaptchaConfig config)
        {
            var textSplitted = text.ToCharArray();
            var random = new Random();
            int yOffset = 0;

            Point letterPoint = startingPoint;
            foreach (var letter in textSplitted)
            {
                var stringToDraw = letter.ToString();

                var letterFontSize = Math.Max(1, baseFont.Size + random.Next(-config.DisturbStringFontResizeFactor, config.DisturbStringFontResizeFactor));
                var letterFont = new Font(baseFont.FontFamily, letterFontSize, baseFont.Style);

                graphics.RotateTransform(random.Next(-config.DisturbStringRotateFactor, config.DisturbStringRotateFactor));

                graphics.DrawString(stringToDraw, letterFont, fontBrush, letterPoint);
                graphics.ResetTransform();


                var stringSize = graphics.MeasureString(stringToDraw, letterFont).ToSize();
                var dividedHeight = stringSize.Height / config.DisturbStringHeightDivideFactor;
                yOffset *= -1;//back to normal position
                yOffset += random.Next(-dividedHeight, dividedHeight);

                var xOffset = stringSize.Width - (stringSize.Width / config.DisturbStringWidthDivideFactor);
                letterPoint.Offset(xOffset, yOffset);

            }
        }
    }
}
