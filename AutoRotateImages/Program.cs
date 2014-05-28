using System;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using ExifLib;

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
                using (ExifReader reader = new ExifReader(item))
                {
                    UInt16 orientation;
                    reader.GetTagValue(ExifTags.Orientation, out orientation);

                    if(orientation != 1)
                    {
                        EncoderParameters encoderParameters = new EncoderParameters(1);
                        FileStream fs = new FileStream(item, FileMode.Open);
                        Image img = Image.FromStream(fs);
                        PropertyItem pi = img.GetPropertyItem(274);

                        bool edited = false;

                        switch (orientation)
                        {
                            // TODO: Handle other orientations and flips
                            case 6:
                                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Transformation, (long)EncoderValue.TransformRotate90);
                                edited = true;
                                break;
                            case 8:
                                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Transformation, (long)System.Drawing.Imaging.EncoderValue.TransformRotate270);
                                edited = true;
                                break;
                        }

                        if (edited)
                        {
                            // Set orientation to normal so if we run this again it doesn't rotate an already rotated image
                            pi.Value[0] = 1;
                            img.SetPropertyItem(pi);

                            // Added new memorystream
                            MemoryStream ms = new MemoryStream();

                            // Read into memorystream
                            img.Save(ms, GetEncoder(img.RawFormat), encoderParameters);
                            reader.Dispose();
                            fs.Close();
                            File.Delete(item);

                            // Read from memorystream into byte array
                            byte[] ni = ms.ToArray();

                            // Save out using new filestream.
                            using (FileStream nfs = new FileStream(item, FileMode.Create, FileAccess.ReadWrite))
                            {
                                nfs.Write(ni, 0, ni.Length);
                            }
                        }
                    }
                }
            }
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
