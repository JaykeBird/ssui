using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SolidShineUi
{
    /// <summary>
    /// A class containing methods that can be used to load colors from various color palette file formats.
    /// </summary>
    public static class ColorPaletteFileReader

    {
        // The PAL and Adobe/Photoshop file functions come from https://www.cyotek.com/blog/tag/color-palette
        // which has been a really helpful resource while dealing with these files

        // some of the code also comes from https://www.mechanikadesign.com/software/swatchbuckler/
        // however, it seems important to note that Mechanika Design seems to have taken the code wholesale from Cyotek
        // if you compare the two links below, you'll find the text to be exactly the same (including the code)
        // Mechanika Design: https://www.mechanikadesign.com/2014/07/reading-adobe-color-swatch-aco-files-using-c/
        // Cyotek: https://www.cyotek.com/blog/reading-photoshop-color-swatch-aco-files-using-csharp
        // however, the author is different (Richard Moss runs Cyotek, while Warren Galyen runs Mechanika Design)
        // the big thing that separates them (and makes me think Cyotek is the true source) is the publish date of the two posts
        // I'm not entirely sure I understand the full situation here, so I am still going to credit both
        // However, I believe Cyotek to be the original author of the code

        // for this library, these color palettes are currently only used for the "From Palette" section of the Color Picker dialog.
        // since it only displays colors as a flat list, and not as anything more complex, all these functions will do is output a list of colors
        // if you're looking for something more advanced, you'll want to turn to Cyotek's blog linked above

#region PAL

        /// <summary>
        /// Loads colors from a .pal color palette file. Supports JASC and RIFF file types.
        /// </summary>
        /// <param name="filename">The file to load.</param>
        /// <exception cref="FormatException">Thrown if the file format doesn't match the JASC or RIFF types.</exception>
        public static List<Color> LoadPalFilePalette(string filename)
        {
            try
            {
                return LoadJascPalette(filename);
            }
            catch (InvalidDataException)
            {
                try
                {
                    return LoadRiffPalette(filename);
                }
                catch (FormatException e)
                {
                    throw new FormatException("Reached unexpected end of file. This palette file may not be compatible.", e);
                }
                catch (InvalidDataException e)
                {
                    throw new FormatException("Reached unexpected end of file. This palette file may not be compatible.", e);
                }
            }
        }

        /// <summary>
        /// Loads colors from a color palette file formatted using the RIFF format.
        /// </summary>
        /// <param name="filename">The file to load.</param>
        /// <exception cref="FormatException">Thrown if the file doesn't match the expected format.</exception>
        public static List<Color> LoadRiffPalette(string filename)
        {
            try
            {
                List<Color> colors = new List<Color>();
                FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                using (BinaryReader br = new BinaryReader(stream))
                {
                    // RIFF header
                    string riff = ReadByteString(br, 4); // "RIFF"
                    int dataSize = br.ReadInt32();
                    string type = ReadByteString(br, 4); // "PAL "

                    // Data chunk
                    string chunkType = ReadByteString(br, 4); // "data"
                    int chunkSize = br.ReadInt32();
                    short palVersion = br.ReadInt16(); // always 0x0300
                    short palEntries = br.ReadInt16();

                    // Colors
                    for (int i = 0; i < palEntries; i++)
                    {
                        byte red = br.ReadByte();
                        byte green = br.ReadByte();
                        byte blue = br.ReadByte();
                        byte flags = br.ReadByte(); // always 0x00
                        colors.Add(Color.FromRgb(red, green, blue));
                    }
                }
                return colors;
            }
            catch (InvalidDataException e)
            {
                throw new FormatException("Reached unexpected end of file. This palette file may not be compatible.", e);
            }
            catch (EndOfStreamException e)
            {
                throw new FormatException("Reached unexpected end of file. This palette file may not be compatible.", e);
            }
        }

        /// <summary>
        /// Loads colors from a color palette file formatted using the JASC format.
        /// </summary>
        /// <param name="filename">The file to load.</param>
        /// <exception cref="FormatException">Thrown if the file doesn't match the expected format.</exception>
        public static List<Color> LoadJascPalette(string filename)
        {
            using (Stream stream = File.OpenRead(filename))
            {

                if (stream == null)
                {
                    throw new ArgumentNullException(nameof(stream));
                }

                List<Color> results = new List<Color>();

                using (StreamReader reader = new StreamReader(stream))
                {
                    string header;
                    string version;
                    int colorCount;

                    // check signature
                    header = reader.ReadLine() ?? "";
                    version = reader.ReadLine() ?? "";

                    if (header != "JASC-PAL" || version != "0100")
                    {
                        throw new InvalidDataException("Invalid palette file");
                    }

                    colorCount = Convert.ToInt32(reader.ReadLine());
                    for (int i = 0; i < colorCount; i++)
                    {
                        int r;
                        int g;
                        int b;
                        string data;
                        string[] parts;

                        data = reader.ReadLine() ?? "";
                        parts = !string.IsNullOrEmpty(data) ? data.Split(new[]
                                                                         {
                                                             ' ',
                                                             '\t'
                                                           }, StringSplitOptions.RemoveEmptyEntries) : new string[0];

                        if (!int.TryParse(parts[0], out r) || !int.TryParse(parts[1], out g) || !int.TryParse(parts[2], out b))
                        {
                            throw new InvalidDataException(string.Format("Invalid palette contents found with data '{0}'", data));
                        }

                        results.Add(Color.FromRgb((byte)r, (byte)g, (byte)b));
                    }
                }

                return results;
            }
        }

        /// <summary>
        /// Loads colors from a color palette file, reading from raw binary bytes.
        /// </summary>
        /// <param name="filename">The file to load.</param>
        /// <exception cref="FormatException">Thrown if the file doesn't match the expected format.</exception>
        public static List<Color> LoadRawPalette(string filename)
        {
            using (Stream stream = File.OpenRead(filename))
            {
                if (stream == null)
                {
                    throw new ArgumentNullException(nameof(stream));
                }

                List<Color> results = new List<Color>();

                for (int i = 0; i < stream.Length / 3; i++)
                {
                    int r;
                    int g;
                    int b;

                    r = stream.ReadByte();
                    g = stream.ReadByte();
                    b = stream.ReadByte();

                    results.Add(Color.FromRgb((byte)r, (byte)g, (byte)b));
                }

                return results;
            }
        }

        static string ReadByteString(BinaryReader br, int length)
        {
            ASCIIEncoding ae = new ASCIIEncoding();

            return ae.GetString(br.ReadBytes(length));
        }

#endregion

#region Photoshop ACO
        // see notes at top of code file
        // https://www.mechanikadesign.com/2014/07/reading-adobe-color-swatch-aco-files-using-c/
        // https://www.cyotek.com/blog/reading-photoshop-color-swatch-aco-files-using-csharp

        private enum ColorSpace
        {
            Rgb = 0,

            Hsb = 1,

            Cmyk = 2,

            Lab = 7,

            Grayscale = 8
        }

        private enum FileVersion
        {
            Version1 = 1,

            Version2
        }

        /// <summary>
        /// Reads a 16bit unsigned integer in big-endian format.
        /// </summary>
        /// <param name="stream">The stream to read the data from.</param>
        /// <returns>The unsigned 16bit integer cast to an <c>Int32</c>.</returns>
        private static int ReadInt16(Stream stream)
        {
            return (stream.ReadByte() << 8) | (stream.ReadByte() << 0);
        }

        /// <summary>
        /// Reads a 32bit unsigned integer in big-endian format.
        /// </summary>
        /// <param name="stream">The stream to read the data from.</param>
        /// <returns>The unsigned 32bit integer cast to an <c>Int32</c>.</returns>
        private static int ReadInt32(Stream stream)
        {
            return ((byte)stream.ReadByte() << 24) | ((byte)stream.ReadByte() << 16) | ((byte)stream.ReadByte() << 8) | ((byte)stream.ReadByte() << 0);
        }

        /// <summary>
        /// Loads colors from a Photoshop color palette file (.aco). Note that Adobe Swatch Exchange files (.ase) are not supported.
        /// </summary>
        /// <param name="filename">The file to load.</param>
        /// <exception cref="FormatException">Thrown if the file doesn't match the expected format.</exception>
        public static List<Color> LoadPhotoshopPalette(string fileName)
        {
            List<Color> colorPalette;

            using (Stream stream = File.OpenRead(fileName))
            {
                FileVersion version;

                // read the version, which occupies two bytes
                version = (FileVersion)ReadInt16(stream);

                if (version != FileVersion.Version1 && version != FileVersion.Version2)
                    throw new FormatException("This Photoshop palette file has invalid version information.");

                // the specification states that a version2 palette follows a version1
                // the only difference between version1 and version2 is the inclusion 
                // of a name property. Perhaps there's addtional color spaces as well
                // but we can't support them all anyway
                // I noticed some files no longer include a version 1 palette

                colorPalette = ReadSwatches(stream, version);
                if (version == FileVersion.Version1)
                {
                    version = (FileVersion)ReadInt16(stream);
                    if (version == FileVersion.Version2)
                        colorPalette = ReadSwatches(stream, version);
                }
            }

            return colorPalette;
        }

        /// <summary>
        /// Reads a unicode string of the specified length.
        /// </summary>
        /// <param name="stream">The stream to read the data from.</param>
        /// <param name="length">The number of characters in the string.</param>
        /// <returns>The string read from the stream.</returns>
        private static string ReadString(Stream stream, int length)
        {
            byte[] buffer;

            buffer = new byte[length * 2];

            stream.Read(buffer, 0, buffer.Length);

            return Encoding.BigEndianUnicode.GetString(buffer);
        }

        private static List<Color> ReadSwatches(Stream stream, FileVersion version)
        {
            int colorCount;
            List<Color> results;

            results = new List<Color>();

            // read the number of colors, which also occupies two bytes
            colorCount = ReadInt16(stream);

            for (int i = 0; i < colorCount; i++)
            {
                ColorSpace colorSpace;
                int value1;
                int value2;
                int value3;
                int value4;

                // again, two bytes for the color space
                colorSpace = (ColorSpace)(ReadInt16(stream));

                value1 = ReadInt16(stream);
                value2 = ReadInt16(stream);
                value3 = ReadInt16(stream);
                value4 = ReadInt16(stream);

                if (version == FileVersion.Version2)
                {
                    int length;

                    // need to read the name even though currently our colour collection doesn't support names
                    length = ReadInt32(stream);
                    ReadString(stream, length);
                }

                switch (colorSpace)
                {
                    case ColorSpace.Rgb:
                        int red;
                        int green;
                        int blue;

                        // RGB.
                        // The first three values in the color data are red , green , and blue . They are full unsigned
                        //  16-bit values as in Apple's RGBColor data structure. Pure red = 65535, 0, 0.

                        red = value1 / 256; // 0-255
                        green = value2 / 256; // 0-255
                        blue = value3 / 256; // 0-255

                        results.Add(Color.FromRgb((byte)red, (byte)green, (byte)blue));
                        break;
                    case ColorSpace.Hsb:
                        double hue;
                        double saturation;
                        double brightness;

                        // HSB.
                        // The first three values in the color data are hue , saturation , and brightness . They are full 
                        // unsigned 16-bit values as in Apple's HSVColor data structure. Pure red = 0,65535, 65535.

                        hue = value1 / 182.04; // 0-359
                        saturation = value2 / 655.35 / 100; // 0-1.0
                        brightness = value3 / 655.35 / 100; // 0-1.0

                        results.Add(ColorsHelper.CreateFromHSV(hue, saturation, brightness));
                        break;
                    case ColorSpace.Grayscale:

                        int gray;

                        // Grayscale.
                        // The first value in the color data is the gray value, from 0...10000.

                        gray = (int)(value1 / 39.0625); // 0-255

                        results.Add(Color.FromRgb((byte)gray, (byte)gray, (byte)gray));
                        break;
                    default:
                        throw new InvalidDataException(string.Format("Color space '{0}' not supported.", colorSpace));
                }
            }

            return results;
        }

        #endregion

        //#region Adobe ASE (currently doesn't work)

        //// this file format, in particular, suffers from the lack of advanced features which this library doesn't need
        //// all support/handling for color groups, as well as a lot of other data, is simply stripped out
        //// so all that remains is a flat list of colors
        //// this is probably on the verge of unhelpful for users with large/organized palette files, so this may be something to revisit later on

        //public static List<Color> LoadAsePalette(string filename)
        //{
        //    using (Stream stream = File.OpenRead(filename))
        //    {
        //        List<Color> colors = new List<Color>();

        //        int blockCount;

        //        ReadAndValidateVersion(stream);

        //        blockCount = stream.ReadUInt32BigEndian();

        //        for (int i = 0; i < blockCount; i++)
        //        {
        //            int blockType;

        //            blockType = stream.ReadUInt16BigEndian();

        //            // 0x0001 denotes a color block, which is all we're looking for here
        //            if (blockType == 0x0001)
        //            {
        //                colors.Add(ReadColorBlock(stream));
        //            }
        //        }

        //        return colors;
        //    }
        //}

        //private static void ReadAndValidateVersion(Stream stream)
        //{
        //    string signature;
        //    int majorVersion;
        //    int minorVersion;

        //    // get the signature (4 ascii characters)
        //    signature = stream.ReadAsciiString(4);

        //    if (signature != "ASEF")
        //    {
        //        throw new InvalidDataException("Invalid file format.");
        //    }

        //    // read the version
        //    majorVersion = stream.ReadUInt16BigEndian();
        //    minorVersion = stream.ReadUInt16BigEndian();

        //    if (majorVersion != 1 && minorVersion != 0)
        //    {
        //        throw new InvalidDataException("Invalid version information.");
        //    }
        //}

        //private static Color ReadColorBlock(Stream stream)
        //{
        //    string colorMode;
        //    int r;
        //    int g;
        //    int b;

        //    // get the mode of the color, which is stored
        //    // as four ASCII characters
        //    colorMode = stream.ReadAsciiString(4);

        //    // read the color data
        //    // how much data we need to read depends on the
        //    // color mode we previously read
        //    switch (colorMode)
        //    {
        //        case "RGB ":
        //            // RGB is comprised of three floating point values ranging from 0-1.0
        //            float value1;
        //            float value2;
        //            float value3;
        //            value1 = stream.ReadSingleBigEndian();
        //            value2 = stream.ReadSingleBigEndian();
        //            value3 = stream.ReadSingleBigEndian();
        //            r = Convert.ToInt32(value1 * 255);
        //            g = Convert.ToInt32(value2 * 255);
        //            b = Convert.ToInt32(value3 * 255);
        //            break;
        //        case "CMYK":
        //            // CMYK is comprised of four floating point values
        //            throw new InvalidDataException($"Unsupported color mode '{colorMode}'.");
        //        case "LAB ":
        //            // LAB is comprised of three floating point values
        //            throw new InvalidDataException($"Unsupported color mode '{colorMode}'.");
        //        case "Gray":
        //            // Grayscale is comprised of a single floating point value
        //            throw new InvalidDataException($"Unsupported color mode '{colorMode}'.");
        //        default:
        //            throw new InvalidDataException($"Unsupported color mode '{colorMode}'.");
        //    }

        //    return Color.FromRgb((byte)r, (byte)g, (byte)b);
        //}

        //#endregion

        #region Paint.NET/GIMP

        /// <summary>
        /// Loads colors from a PAINT.NET or basic HEX color palette file (.txt;.hex).
        /// </summary>
        /// <param name="filename">The file to load.</param>
        /// <exception cref="ArgumentException">Thrown if the file cannot be opened.</exception>
        public static List<Color> LoadPaintNetPalette(string fileName)
        {
            using (Stream stream = File.OpenRead(fileName))
            {
                if (stream == null)
                {
                    throw new ArgumentException("stream");
                }

                List<Color> results = new List<Color>();

                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        string line;

                        line = reader.ReadLine() ?? "";
                        if (!string.IsNullOrEmpty(line) && !line.StartsWith(";"))
                        {
                            try
                            {
                                results.Add(ColorsHelper.CreateFromHex(line));
                            }
                            catch (FormatException)
                            {
                                Color? c = LoadRgbLine(line);
                                if (c.HasValue)
                                {
                                    results.Add(c.Value);
                                }
                            }
                        }
                    }
                }

                return results;
            }
        }

        /// <summary>
        /// Loads colors from a GIMP color palette file (.gpl).
        /// </summary>
        /// <param name="filename">The file to load.</param>
        /// <exception cref="FormatException">Thrown if the file doesn't match the GIMP file format (the first lime must say "gimp palette").</exception>
        /// <exception cref="ArgumentException">Thrown if the file cannot be opened.</exception>
        public static List<Color> LoadGimpPalette(string fileName)
        {
            using (Stream stream = File.OpenRead(fileName))
            {

                if (stream == null)
                {
                    throw new ArgumentException("stream");
                }

                List<Color> results = new List<Color>();

                using (StreamReader reader = new StreamReader(stream))
                {
                    //bool startReading = false;
                    bool firstLine = true;

                    while (!reader.EndOfStream)
                    {
                        // read the next line in the file
                        string line = reader.ReadLine() ?? "";

                        if (firstLine)
                        {
                            if (line.Trim().ToLowerInvariant() == "gimp palette")
                            {
                                firstLine = false;
                            }
                            else
                            {
                                throw new FormatException("This file does not appear to be a GIMP Palette file.");
                            }
                        }
                        else if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }
                        else if (line.StartsWith("#"))
                        {
                            continue;
                        }
                        else
                        {
                            Color? c = LoadRgbLine(line);
                            if (c.HasValue)
                            {
                                results.Add(c.Value);
                            }
                        }
                    } // end while
                }

                return results;
            }
        }

        static Color? LoadRgbLine(string line)
        {
            string r = "";
            string g = "";
            string b = "";
            int mode = 0;
            bool spaceCancel = true;

            foreach (char c in line)
            {
                if (c == ' ')
                {
                    if (mode == 0)
                    {
                        continue;
                    }
                    else if (spaceCancel)
                    {
                        continue;
                    }
                    else
                    {
                        mode++;
                        spaceCancel = true;
                    }
                }
                else if (c == '\t')
                {
                    if (mode == 0)
                    {
                        continue;
                    }
                    else if (spaceCancel)
                    {
                        continue;
                    }
                    else
                    {
                        mode++;
                        spaceCancel = true;
                    }
                }
                else
                {
                    spaceCancel = false;
                    switch (mode)
                    {
                        case 0:
                            r += c;
                            mode = 1;
                            break;
                        case 1:
                            r += c;
                            break;
                        case 2:
                            g += c;
                            break;
                        case 3:
                            b += c;
                            break;
                        default:
                            break;
                    }
                }
            }

            //r = line.Substring(0, 3).Trim();
            //g = line.Substring(4, 3).Trim();
            //b = line.Substring(8, 3).Trim();

            try
            {
                byte iR = byte.Parse(r);
                byte iG = byte.Parse(g);
                byte iB = byte.Parse(b);

                return Color.FromRgb(iR, iG, iB);
            }
            catch (FormatException)
            {
                return null;
            }
        }

        #endregion

        #region Bitmap Palette (TIF, GIF)

        // https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.bitmappalette

        // not sure why Microsoft limits this to 256 colors, maybe I'll investigate some day but it's not a priority

        /// <summary>
        /// Loads colors from the palette present in TIFF and GIF image files (.tif;.tiff;.gif). Only the first 256 colors are loaded.
        /// </summary>
        /// <param name="filename">The file to load.</param>
        /// <exception cref="ArgumentException">Thrown if the file cannot be opened.</exception>
        /// <exception cref="ArgumentNullException">Thrown if a null value is passed in or if the file cannot be opened.</exception>
        /// <exception cref="UriFormatException">Thrown if the file string does not appear to be a path or URL.</exception>
        public static List<Color> LoadBitmapImagePalette(string file)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(file, UriKind.Absolute);
            img.EndInit();
            BitmapPalette bpal = new BitmapPalette(img, 256); // only grabs first 256 colors

            return bpal.Colors.ToList();
        }

#endregion

    }
}
