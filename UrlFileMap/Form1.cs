using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace UrlFileMap
{
    public partial class Form1 : Form
    {

        Color[] lightColor = new Color[8];
        public Form1()
        {
            InitializeComponent();
            lightColor[0] = Color.FromArgb(152, 251, 152);
            lightColor[1] = Color.FromArgb(135, 206, 250);
            lightColor[2] = Color.FromArgb(255, 182, 193);
            lightColor[3] = Color.FromArgb(255, 250, 205);
            lightColor[4] = Color.FromArgb(255, 160, 122);
            lightColor[5] = Color.FromArgb(144, 238, 144);
            lightColor[6] = Color.FromArgb(255, 228, 225);
            lightColor[7] = Color.FromArgb(240, 248, 255);
        }

        private Dictionary<string, Dictionary<string, string>> GetMatchResult(RichTextBox r, string pattern)
        { 
            r.SelectAll();
            r.SelectionBackColor = Color.White;
            r.SelectionColor = Color.Black;
            r.DeselectAll();
            Dictionary<string, Dictionary<string, string>> ldd = new Dictionary<string, Dictionary<string, string>>();
            int index = 0;
            for (int i = 0; i < r.Lines.Length; i++)
            {
                int colorindex = 0;
                var line = r.Lines[i];
                if (line.Trim() == "")
                    continue;
                if (ldd.ContainsKey(line))
                    continue;
                ldd.Add(line, new Dictionary<string, string>());
                Regex rgx = new Regex(pattern);
                Match match = rgx.Match(line);
                if (match.Success)
                {
                    string[] names = rgx.GetGroupNames();
                    Console.WriteLine("Named Groups:");
                    Dictionary<string, string> dd = new Dictionary<string, string>();
                    foreach (var name in names)
                    {
                        Group grp = match.Groups[name];
                        if (name != "0")
                        {
                            //r.Select(index + grp.Index, grp.Length
                            int Si = r.GetFirstCharIndexFromLine(i) + grp.Index;
                            r.Select(Si, grp.Length);
                            r.SelectionBackColor = lightColor[colorindex % 8];
                            colorindex++;
                        }

                        dd.Add(name, grp.Value);
                        Console.WriteLine("{0}: '{1}'", name, grp.Value);
                    }
                    ldd[line] = dd;
                }
                index += line.Length + 1;
            }
            return ldd;
        }


        // google richtextbox GetFirstCharIndexFromLine wordwrap
        // https://stackoverflow.com/questions/61606673/finding-width-of-each-line-in-richtextbox-where-wordwrap-is-true
        public void Rb(RichTextBox i)
        {
            int previousFirstCharIndex = 0;
            int lineIndex = 1;
            int firstCharIndex = i.GetFirstCharIndexFromLine(lineIndex);
            List<int> lineLengths = new List<int>();
            do
            {
                lineLengths.Add(firstCharIndex - previousFirstCharIndex);
                previousFirstCharIndex = firstCharIndex;
                lineIndex += 1;
                firstCharIndex = i.GetFirstCharIndexFromLine(lineIndex);
            } while (firstCharIndex != -1);

            lineLengths.Add(i.TextLength - previousFirstCharIndex);

            MessageBox.Show(string.Join(Environment.NewLine, lineLengths), "Line Lengths");
        }
        private Dictionary<string, Dictionary<string, string>> GetMatchResult(string[] lines, string pattern)
        {
            Dictionary<string, Dictionary<string, string>> ldd = new Dictionary<string, Dictionary<string, string>>();
            foreach (var line in lines)
            {
                if (ldd.ContainsKey(line))
                    continue;
                ldd.Add(line, new Dictionary<string, string>());
                Regex rgx = new Regex(pattern);
                Match match = rgx.Match(line);
                if (match.Success)
                {
                    string[] names = rgx.GetGroupNames();
                    Console.WriteLine("Named Groups:");
                    Dictionary<string, string> dd = new Dictionary<string, string>();
                    foreach (var name in names)
                    {
                        Group grp = match.Groups[name];
                        dd.Add(name, grp.Value);
                        Console.WriteLine("   {0}: '{1}'", name, grp.Value);
                    }
                    ldd[line] = dd;
                }
            }
            return ldd;
        }


        // a-b
        private Dictionary<string, Dictionary<string, string>> CompareListDict(Dictionary<string, Dictionary<string, string>> a, Dictionary<string, Dictionary<string, string>> b)
        {
            List<string> Removestring = new List<string>();
            foreach (var k in b.Keys)
            {
                foreach (var i in a.Keys)
                {
                    if (CompareDict(a[i], b[k]))
                    {
                        Removestring.Add(i);
                        break;
                    }
                }
            }
            foreach (var i in Removestring)
            {
                a.Remove(i);
            }
            return a;
        }

        private bool CompareDict(Dictionary<string, string> a, Dictionary<string, string> b)
        {
            foreach (var i in a)
            {
                if (i.Key == "0")
                    continue;
                if (!b.ContainsKey(i.Key))
                    return false;
                if (b[i.Key] != i.Value)
                    return false;
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Rb(richTextBox1);
            var a = GetMatchResult(richTextBox1, textBox1.Text);
            var b = GetMatchResult(richTextBox2, textBox2.Text);
            var c = GetMatchResult(richTextBox3, textBox3.Text);
            a = CompareListDict(a, b);
            a = CompareListDict(a, c);
            richTextBox4.Text = "";


            foreach(var i in a.Keys)
            {
                string Seq = "";
                foreach (var j in textBox5.Text.Split(','))
                {
                    if (a[i].ContainsKey(j))
                        Seq += a[i][j];
                }
                a[i].Add("Seq", Seq);
            }

            foreach (var i in a.OrderBy(x => x.Value["Seq"]))
            {
                richTextBox4.Text += i.Key + "\r\n";
            }


            //foreach (var i in a)
            //{
            //    richTextBox4.Text += i.Key + "\r\n";
            //}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = "";
            var i = Directory.GetFiles(textBox4.Text);
            richTextBox2.Text = "";
            foreach (var j in i)
            {
                richTextBox2.Text += j + "\r\n";
            }
        }

    }
}
