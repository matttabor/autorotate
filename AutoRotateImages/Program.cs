using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace AutoRotateImages
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = args[0];
            //string path = @"C:\Users\McTabor\Pictures\Emmett Samples";
            var files = Directory.GetFiles(path, "*.jpg");

            if (files.Length < 1)
            {
                Console.WriteLine("No .jpg images found in {0}.\nPress enter to exit.", path);
                Console.Read();
                return;
            }

            foreach (var item in files)
            {
                bool edited = false;
                FileStream fs = new FileStream(item, FileMode.Open);
                Image im = Image.FromStream(fs);
                ImageFormat sourceFormat = im.RawFormat;
                EncoderParameters encoderParameters = null;
                PropertyItem pi = im.GetPropertyItem(274);
                int orientation = pi.Value[0];

                encoderParameters = new EncoderParameters(1);

                switch (orientation)
                {
                    case 1:
                        break;
                    case 2:
                        
                        break;
                    case 3:
                       
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                    case 6:
                        encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Transformation, (long)EncoderValue.TransformRotate90);
                        edited = true;
                        pi.Value[0] = 1;
                        im.SetPropertyItem(pi);
                        break;
                    case 7:
                        break;
                    case 8:
                        encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Transformation, (long)EncoderValue.TransformRotate270);
                        edited = true;
                        pi.Value[0] = 1;
                        im.SetPropertyItem(pi);
                        break;
                }

                if (edited)
                {
                    string newpath = path + "\\" + Path.GetFileNameWithoutExtension(item) + "-1" + ".jpg";
                    im.Save(newpath, GetEncoder(sourceFormat), encoderParameters);
                    fs.Close();
                    File.Delete(item);
                }

                im.Dispose();
                if (encoderParameters != null)
                    encoderParameters.Dispose();
                fs.Dispose();
            }

            files = Directory.GetFiles(path, "*-1.jpg");
            foreach (var f in files)
            {
                File.Move(f, f.Replace("-1", ""));
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            foreach (var info in ImageCodecInfo.GetImageEncoders())
                if (info.FormatID == format.Guid)
                    return info;
            return null;
        }
    }
}
