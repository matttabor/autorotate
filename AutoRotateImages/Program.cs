using System;
using System.Linq;
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
            var files = Directory.EnumerateFiles(path).Where(f => f.EndsWith(".jpg") || f.EndsWith(".jpeg") || f.EndsWith("JPG") || f.EndsWith("JPEG")).ToList();

            if (files.Count < 1)
            {
                Console.WriteLine("No .jpg images found in {0}.\nPress enter to exit.", path);
                Console.Read();
                return;
            }

            foreach (var item in files)
            {
                //System.IO.FileStream fs = new System.IO.FileStream(item, System.IO.FileMode.Open);
                FileStream fs = new FileStream(item, FileMode.Open);
                Image img = Image.FromStream(fs);
                PropertyItem pi = img.GetPropertyItem(274);
                int orientation = pi.Value[0];
                EncoderParameters encoderParameters = new EncoderParameters(1);
                ImageFormat sourceFormat = img.RawFormat;
                MemoryStream ms;
                byte[] ni;

                switch (orientation)
                {
                    case 6:
                        encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Transformation, (long)EncoderValue.TransformRotate90);

                        // Set orientation to normal so if we run this again it doesn't rotate an already rotated image
                        pi.Value[0] = 1;
                        img.SetPropertyItem(pi);

                        // Added new memorystream
                        ms = new MemoryStream();

                        // Read into memorystream
                        img.Save(ms, GetEncoder(sourceFormat), encoderParameters);
                        fs.Close();
                        File.Delete(item);

                        // Read from memorystream into byte array
                        ni = ms.ToArray();

                        // Save out using new filestream.
                        using (FileStream nfs = new FileStream(item, FileMode.Create, FileAccess.ReadWrite))
                        {
                            nfs.Write(ni, 0, ni.Length);
                        }

                        break;
                    case 8:
                        encoderParameters.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Transformation, (long)System.Drawing.Imaging.EncoderValue.TransformRotate270);
                        
                        // Set orientation to normal so if we run this again it doesn't rotate an already rotated image
                        pi.Value[0] = 1;
                        img.SetPropertyItem(pi);
                        
                        // Added new memorystream
                        ms = new MemoryStream();

                        // Read into memorystream
                        img.Save(ms, GetEncoder(sourceFormat), encoderParameters);
                        fs.Close();
                        File.Delete(item);

                        // Read from memorystream into byte array
                        ni = ms.ToArray();

                        // Save out using new filestream.
                        using (FileStream nfs = new FileStream(item, FileMode.Create, FileAccess.ReadWrite))
                        {
                            nfs.Write(ni, 0, ni.Length);
                        }
                        break;
                }


            }
        }

        public static void SaveJpeg(string path, Image img, EncoderParameters encoderParameters)
        {
            
        }

        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            foreach (var info in ImageCodecInfo.GetImageEncoders())
                if (info.FormatID == format.Guid)
                    return info;
            return null;
        }
    }
}
