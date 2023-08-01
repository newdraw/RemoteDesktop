using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; 
using System.Drawing.Imaging; 
using System.Net.Sockets;
using System.Threading;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Runtime.InteropServices;

namespace WindowsFormsApp11
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }
         
          
        Random rnd = new Random();
        const int frameWidth = 128;
        const int frameHeight = 64;
        const int totalPixels = frameWidth * frameHeight;

        double keepLevels = 0.9;
        int ditherLevels = 7;
        DitherType ditherType = DitherType.Pattern;

        enum DitherType
        {
            Pattern = 0,
            Random = 1,
            None = 2
        }

        /// <summary>
        /// 截取屏幕
        /// </summary>
        unsafe byte[] snapshot(out int[] histogram)
        {
            var width = Screen.PrimaryScreen.Bounds.Width;
            var height = Screen.PrimaryScreen.Bounds.Height;
            using var img = new Bitmap(width, height);
            using var g = Graphics.FromImage(img);

            histogram = new int[256];

            var pixels = new byte[totalPixels];

            g.CopyFromScreen(0, 0, 0, 0, img.Size, CopyPixelOperation.SourceCopy);

            var bits = img.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            var p = (byte*)bits.Scan0;

            for (var y = 0; y < frameHeight; y++)
                for (var x = 0; x < frameWidth; x++)
                {
                    var srcX = x * (width / frameWidth);
                    var srcY = y * (height / frameHeight);
                    var pb = p + ((srcY * width) + srcX) * 3;
                    var pg = pb + 1;
                    var pr = pb + 2;
                    var pixel = (byte)(*pr * 0.299 + *pg * 0.587 + *pb * 0.114);

                    pixels[y * frameWidth + x] = pixel;
                    histogram[pixel]++;
                }

            img.UnlockBits(bits);

            return pixels;
        }

        /// <summary>
        /// 裁剪输出区间
        /// </summary>
        /// <param name="histogram"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        void cutLevels(int[] histogram, out byte min, out byte max)
        {
            min = byte.MinValue;
            max = byte.MaxValue; 

            double totalLevels = frameWidth * frameHeight;
            double outLevels = totalLevels;

            while (true)
            {
                if (max - min < ditherLevels * 2) //保留给仿色等级，防止黑白不分
                {
                    break;
                }

                //牺牲量小的一端
                if (histogram[min] > histogram[max])
                {
                    if ((outLevels - histogram[max]) / totalLevels > keepLevels)
                    {
                        max--;
                        outLevels -= histogram[max];
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    if ((outLevels - histogram[min]) / totalLevels > keepLevels)
                    {
                        min++;
                        outLevels -= histogram[min];
                    }
                    else
                    {
                        break;
                    }
                }
            } 
        }

        /// <summary>
        /// 计算帧数据
        /// </summary>
        /// <param name="ditherType"></param>
        /// <param name="pixels"></param>
        /// <param name="histogram"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        byte[] makeFrame(DitherType ditherType, byte[] pixels, int[] histogram, byte min, byte max)
        {

            var middleLevel = 0;
            if (ditherType == DitherType.None)
            {
                var sum = 0;
                for (int i = min; i <= max; i++)
                {
                    sum += histogram[i];
                }
                var count = 0;
                for (var i = min; i <= max; i++)
                {
                    count += histogram[i];
                    if (count >= sum / 2)
                    {
                        middleLevel = i;
                        break;
                    }
                }
            }

            var data = new byte[frameWidth * frameHeight / 8];
            for (var pos = 0; pos < pixels.Length; pos++)
            {

                var x = pos % frameWidth;
                var y = pos / frameWidth;
                var draw = false;
                if (ditherType == DitherType.None)
                {
                    draw = pixels[pos] > middleLevel;
                }
                else
                {
                    var level = (pixels[pos] - min) / Math.Max(1, (max - min) / ditherLevels);
                    ////(x+y*height)*level
                    /////(x + y * (height * 1.1)) * level
                    /////((pos % width)+(pos/width)*height
                    var value = ditherType == DitherType.Random ? rnd.Next() :
                        (x + y * (frameHeight + level )* 1.5) ;
                    draw = value % ditherLevels < level;
                }

                if (draw)
                {
                    var i = pos / 8;
                    var bit = pos % 8;
                    data[i] |= (byte)(1 << bit);
                }
            }
            return data;
        }

        /// <summary>
        /// 计算帧差
        /// </summary>
        /// <param name="currFrame"></param>
        /// <param name="lastFrame"></param>
        /// <returns></returns>
        byte[] findFrameDiff(byte[] currFrame, byte[] lastFrame)
        {
            var frameDiff = new List<byte>();
            if (lastFrame != null)
            {
                short fromIndex = -1;
                var bytes = new List<byte>();

                for (short i = 0; i < currFrame.Length; i++)
                {
                    var last = lastFrame[i];
                    var curr = currFrame[i];
                    if (last != curr)
                    {
                        if (fromIndex == -1)
                        {
                            fromIndex = i;
                        }
                        bytes.Add(curr);
                    }
                    else
                    {
                        if (fromIndex != -1)
                        {
                            frameDiff.AddRange(BitConverter.GetBytes(fromIndex));
                            frameDiff.AddRange(BitConverter.GetBytes((short)bytes.Count));
                            frameDiff.AddRange(bytes);

                            fromIndex = -1;
                            bytes.Clear();
                        }
                    }
                }
            }
            return frameDiff.ToArray();
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="frameData"></param>
        /// <param name="frameDiff"></param>
        /// <param name="isFirstFrame"></param>
        void sendData(NetworkStream stream, byte[] frameData, byte[] frameDiff, bool isFirstFrame)
        {

            const byte DATA_FRAME = 0;
            const byte DATA_FRAMEDIFF = 1;
            lock (stream)
            {
                if (isFirstFrame || frameDiff.Length > frameData.Length)
                {
                    stream.WriteByte(DATA_FRAME);
                    stream.Write(frameData, 0, frameData.Length);
                }
                else
                {
                    var dat = frameDiff.ToArray();
                    stream.WriteByte(DATA_FRAMEDIFF);
                    stream.Write(BitConverter.GetBytes((short)dat.Length), 0, 2);
                    stream.Write(dat, 0, dat.Length);
                }
                stream.Flush();
            }
        }

        /// <summary>
        /// 画直方图
        /// </summary>
        /// <param name="histogram"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        Bitmap paintHistogram(int[] histogram, byte min, byte max)
        {
            using var penLimit = new Pen(Color.Red);
            penLimit.DashStyle = DashStyle.Dot; 
            var imgHistogram = new Bitmap(byte.MaxValue + 1, 200);
            using var gHistogram = Graphics.FromImage(imgHistogram);
            gHistogram.Clear(Color.White);
            var histMax = histogram.Max();

            for (var i = 0; i <= byte.MaxValue; i++)
            {
                if (histogram[i] == 0)
                {
                    continue;
                }
                if (histogram[i] == histMax)
                {

                }
                gHistogram.DrawLine(
                     Pens.Black,
                    i, imgHistogram.Height, i, imgHistogram.Height - (histogram[i] * imgHistogram.Height / histMax));
            }
            gHistogram.DrawLine(penLimit, min, 0, min, imgHistogram.Height);
            gHistogram.DrawLine(penLimit, max, 0, max, imgHistogram.Height);
            return imgHistogram;
        }

        NetworkStream stream = null;

        unsafe void workProc()
        {

            long frames = 0;

            var lastMin = byte.MinValue;
            var lastMax = byte.MaxValue;
            byte[] lastFrame = null;

            using var tcp = new TcpClient();
            tcp.Connect(txtIP.Text, 5720);
            stream = tcp.GetStream();
            while (true)
            {
                var pixels = snapshot(out var histogram);
                cutLevels(histogram, out var min, out var max);

                //从上一个区间平滑过渡，防止闪烁
                min = (byte)(min + (lastMin - min) / 3);
                max = (byte)(max + (lastMax - max) / 3);

                lastMin = min;
                lastMax = max;

                var frame = makeFrame(ditherType, pixels, histogram, min, max);
                var frameDiff = findFrameDiff(frame, lastFrame);
                sendData(stream, frame, frameDiff, frames == 0);

                if (frames % 2 == 0)
                {
                    using var imgHistogram = paintHistogram(histogram, min, max);
                    using var g = picHistogram.CreateGraphics();
                    g.DrawImage(imgHistogram, 0, 0);
                }

                lastFrame = frame;
                frames++;
            }

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            txtIP.Enabled = false;
            btnStart.Enabled = false;
            new Thread(workProc) { IsBackground = true }.Start();
        }

        private void trackDitherLevels_Scroll(object sender, EventArgs e)
        {
            ditherLevels = trackDitherLevels.Value;
        }

        private void trackKeepLevels_Scroll(object sender, EventArgs e)
        {
            keepLevels = trackKeepLevels.Value / 100d;
        }

        private void cboDitherType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ditherType= (DitherType) cboDitherType.SelectedIndex; 
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            cboDitherType.SelectedIndex = 0;
        }

        private void chkShowInfo_CheckedChanged(object sender, EventArgs e)
        {
            if(stream!=null)
            {
                lock(stream)
                {
                    const byte DATA_CONFIG = 2;

                    stream.WriteByte(DATA_CONFIG);
                    stream.WriteByte((byte)(chkShowInfo.Checked?1:0));
                    stream.Flush();
                }
            }
        }
    }
}
