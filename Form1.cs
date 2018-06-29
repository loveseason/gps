using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace gps
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string DegreeToDecimel(string Degree)        //将度分秒格式转换为小数格式，用来生成in文件
        {
            char[] sep = new char[] { '\'', '\"', '°', '.' };
            string[] dms = Degree.Split(sep);
            String result = dms[0] + '.' + dms[1] + dms[2] + dms[3];
            return result;

        }
        //struct WGS      //导出的84文件的数据结构
        //{
        //    public String PName;
        //    public String WGS_lat;
        //    public String WGS_lon;
        //    public String WGS_hgt;

        //}
        public String[] PointName;
        public String[] WGS_latitude;
        public String[] WGS_longtitude;
        public String[] WGS_height;

        public String[] Code;
        public String[] X;
        public String[] Y;
        public String[] LocalHeight;
        public String[] H;



        public int inFileLength;
        public int out_file_lenth;
        public String NY_file_path;
        public String TMP_file_path;                                            //方便以后平均数计算用的，会删除的。
        public String GPS_file_path;

        
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {


                if (openFileDialog1.ShowDialog() == DialogResult.OK)                 //打开84处理为in文件
                {
                    textBox1.Text = openFileDialog1.FileName;
                    String FullPathName = openFileDialog1.FileName;
                    String[] readLines = File.ReadAllLines(openFileDialog1.FileName, Encoding.Default);
                    //用enconding.default来打开windows默认的ANSI格式，防止读取°时出现乱码；
                    //WGS myWGS;
                    string inPath = Path.GetDirectoryName(FullPathName) + '\\' + Path.GetFileNameWithoutExtension(openFileDialog1.FileName) + "-in.txt";
                    NY_file_path = Path.GetDirectoryName(FullPathName) + '\\' + Path.GetFileNameWithoutExtension(openFileDialog1.FileName) + "-NY.txt";
                    TMP_file_path = Path.GetDirectoryName(FullPathName) + '\\' + Path.GetFileNameWithoutExtension(openFileDialog1.FileName) + "-TMP.txt";
                    GPS_file_path = Path.GetDirectoryName(FullPathName) + '\\' + Path.GetFileNameWithoutExtension(openFileDialog1.FileName) + "-GPS.txt";
                    //File.Create(inPath);

                    StreamWriter sw = File.CreateText(inPath);                  //新建in文件
                    String str = null;
                    inFileLength = readLines.Length;
                    PointName = new String[inFileLength];
                    WGS_height = new String[inFileLength];
                    WGS_latitude = new String[inFileLength];
                    WGS_longtitude = new String[inFileLength];
                    Code = new String[inFileLength];
                    X = new String[inFileLength];
                    Y = new String[inFileLength];
                    LocalHeight = new String[inFileLength];

                    for (int i = 0; i < readLines.Length; i++)
                    {
                        richTextBox1.Text += str + readLines[i] + Environment.NewLine;

                        Char[] charsep = new char[] { '\t' };
                        String[] tmp = readLines[i].Split(charsep, StringSplitOptions.RemoveEmptyEntries);   //split选项去掉空白项；

                        //此处添加判定是否有异常行的代码

                        PointName[i] = tmp[0];
                        Code[i] = tmp[1];
                        X[i] = tmp[2];
                        Y[i] = tmp[3];
                        LocalHeight[i] = tmp[4];
                        WGS_latitude[i] = tmp[5];
                        WGS_longtitude[i] = tmp[6];
                        WGS_height[i] = tmp[7];


                        String WGSLine = PointName[i] + '\t' + DegreeToDecimel(WGS_latitude[i]) + '\t' + DegreeToDecimel(WGS_longtitude[i]) + '\t' +
                            WGS_height[i];

                        //StreamWriter sw = new StreamWriter(inPath);
                        //StreamWriter sw = File.Open(inPath, FileMode.Open);
                        sw.WriteLine(WGSLine);
                    }
                    sw.Close();
                    MessageBox.Show("OOOOOOK");
                }
            }

            catch (IOException ex)
            {
                MessageBox.Show(ex.ToString());

            }
        }

        private void button2_Click(object sender, EventArgs e)                      //显示in文件
        {
            richTextBox1.Text = null;
            for (int i = 0; i < PointName.Length; i++)
            {
                richTextBox1.Text += PointName[i] + '\t' + DegreeToDecimel(WGS_latitude[i]) + '\t' + DegreeToDecimel(WGS_longtitude[i]) + '\t' +
                        WGS_height[i] + Environment.NewLine;

            }
            MessageBox.Show("THIS IS IT :)");
        }

        private void button3_Click(object sender, EventArgs e)                  //打开out文件对H赋值
        {
            richTextBox1.Text = null;
            OpenFileDialog opg = new OpenFileDialog();
            opg.Filter = "文本文件|*.txt";
            if (opg.ShowDialog() == DialogResult.OK)
            {
                String[] readLines = File.ReadAllLines(opg.FileName, Encoding.Default);
                out_file_lenth = readLines.Length;
                H = new string[out_file_lenth];
                if (out_file_lenth != inFileLength + 1)
                    MessageBox.Show("in文件与out文件不对应！");
                else
                {
                    for (int i = 1; i < readLines.Length; i++)
                    {
                        richTextBox1.Text += readLines[i] + Environment.NewLine;

                        Char[] charsep = new char[] { '\t', ' ' };
                        String[] tmp = readLines[i].Split(charsep, StringSplitOptions.RemoveEmptyEntries);   //split选项去掉空白项；

                        //此处添加判定是否有异常行的代码
                        H[i-1] = tmp[3];

                    }
                    MessageBox.Show(H[out_file_lenth - 2]);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)              //生成内业格式文件
        {
            richTextBox1.Text = null;
            try
            { 
                StreamWriter sw = File.CreateText(NY_file_path);                  //新建NY文件
                for (int i = 0; i < inFileLength; i++)
                {
                    sw.WriteLine(Code[i] + '/' + X[i] + '/' + Y[i] + '/' + H[i]);
                }
                sw.Close();

                richTextBox1.Text = File.ReadAllText(NY_file_path);

                MessageBox.Show("NY已经生成:)");
            }

            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public class MyData
        {
            public string Name;

            public int Count;

            public double MeanX;

            public double MeanY;

            public double MeanZ;

            public MyData(string str)
            {
                Name = str;
            }
            public void Add(double x, double y, double z)
            {
                if (Count == 0)
                {
                    MeanX = x;
                    MeanY = y;
                    MeanZ = z;
                    Count = 1;
                }
                else
                {
                    MeanX = (MeanX * Count + x) / (Count + 1);
                    MeanY = (MeanY * Count + y) / (Count + 1);
                    MeanZ = (MeanZ * Count + z) / (Count + 1);
                    Count++;
                }
            }
        }

        //求取重复测绘的互差
        public class hucha      
        {
            public string DisName;
            public double DisX;
            public double DisY;
            public double DisZ;

            public hucha(string str)
            {
                DisName = str;
            }

            public void discha(List<double> lx, List<double> ly, List<double> lz)
            {
                DisX = lx.Max() - lx.Min();
                DisY = ly.Max() - ly.Min();
                DisZ = lz.Max() - lz.Min();
            }

            

        }

        public List<Tuple<string, string, double, double, double>> TargetData = new List<Tuple<string, string, double, double, double>>();
        public List<MyData> Data = new List<MyData>();
        public List<hucha> myhucha = new List<hucha>();
        private void button5_Click(object sender, EventArgs e)              //开始生成GPS格式文件
        {
            richTextBox1.Text = null;

            try
            {
                StreamWriter sw1 = File.CreateText(GPS_file_path);
                sw1.WriteLine("          青岛市勘察测绘研究院ＧＰＳ观测成果表");
                sw1.WriteLine("=======================================================");
                sw1.WriteLine("点名     X(m)            Y(m)          H(m)");

                for (int i = 0; i < inFileLength; i++)
                {
                    sw1.WriteLine(Code[i] + '\t' + X[i] + '\t' + Y[i] + '\t' + H[i]);
                    if (PointName[i].Contains("-"))
                    {
                        string[] ss = PointName[i].Split(new char[] { '-' });
                        TargetData.Add(new Tuple<string, string, double, double, double>(ss[0], ss[1], double.Parse(X[i]), double.Parse(Y[i]), double.Parse(H[i])));
                    }
                }
                if (TargetData.Count != 0)
                {
                    List<string> existStr = new List<string>();             //测站的个数 "-"前面一样的字符串,将控制点点名放到一个list中
                    foreach (var t in TargetData)
                    {
                        if (!existStr.Contains(t.Item1))
                        {
                            existStr.Add(t.Item1);
                        }
                    }

                    for (int j = 0; j < existStr.Count; j++)
                    {
                        MyData data = new MyData(existStr[j]);
                        hucha wohucha = new hucha(existStr[j]);
                        List<double> tx = new List<double>();
                        List<double> ty = new List<double>();
                        List<double> tz = new List<double>();
                        foreach (var t2 in TargetData)
                        {

                            if (existStr[j] == t2.Item1)
                            {
                                data.Add(t2.Item3, t2.Item4, t2.Item5);
                                tx.Add(t2.Item3 * 100);
                                ty.Add(t2.Item4 * 100);
                                tz.Add(t2.Item5 * 100);
                            }
                            wohucha.discha(tx, ty, tz);                     //求取互差
                        }
                        Data.Add(data);                             //将需要求平均的数据放到一个list中
                        myhucha.Add(wohucha);
                    }


                }

                sw1.WriteLine("=======================================================");
                sw1.WriteLine("注: 平差后的成果为");

                foreach (var d in Data)
                {
                    sw1.WriteLine(d.Name + "\t" + d.MeanX.ToString("F3") + "\t" + d.MeanY.ToString("F3") + "\t" + d.MeanZ.ToString("F3"));          // .ToString("F3");  保留三维小数
                }



                sw1.WriteLine("=======================================================");
                sw1.WriteLine("测量:沙海龙      计算:沙海龙      检核:崔现勇");
                sw1.WriteLine();
                sw1.WriteLine("仪器:天宝5046455569       日期:2018-");
                sw1.WriteLine();
                sw1.WriteLine("备注：H为经青岛市高精度似大地水准面精化的正常高，负值为不要高程。");
                sw1.WriteLine();
                sw1.WriteLine("L083(原)	107379.565	239213.737	46.597 ");
                sw1.WriteLine("		△X=		△Y=		△H=");
                sw1.Close();

                richTextBox1.Text = File.ReadAllText(GPS_file_path);

                //下面代码用于显示有没有测回超限的数据，限差暂定为3cm

                foreach(var h in myhucha)
                {
                    richTextBox2.AppendText("\n");
                    richTextBox2.AppendText(h.DisName + "\t" + h.DisX.ToString("F3") + "\t" + h.DisY.ToString("F3") + "\t" + h.DisZ.ToString("F3"));
                    //判断有没有超限
                    if(h.DisX >= 3 | h.DisY >= 3 | h.DisZ >= 3 )
                    {
                        richTextBox2.AppendText("\n");
                        richTextBox2.AppendText("有超限的数据，请检查" + h.DisName);
                    }
                }

                


                MessageBox.Show("GPS已生成 :)");
                //MessageBox.Show("gps已生成！");
            }


            catch (IOException ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }


    }
}

