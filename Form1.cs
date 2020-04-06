using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.IO;

namespace MTtest
{
    public partial class Form1 : Form
    {
        public string SkinType, Category, browserurl;
        public int cyclenumber = 0;
        public bool steamcom, umod, commas, zero, last, json, popular;
        public List<int> checkeditems = new List<int>();
        public string steamurl = @"https://steamcommunity.com/workshop/browse/?appid=252490";
        List<SkinList> list = new List<SkinList>();
        Dictionary<string, string> steamnames = new Dictionary<string, string>() 
        {
            {"Bandana Mask", "Bandana"},
            {"Improvised Balaclava", "Balaclava"},
            {"Burlap Trousers", "Burlap Pants"},
            {"Baseball Cap", "Cap"},
            {"Shirt", "Collared Shirt"},
            {"Bone Helmet", "Deer Skull Mask"},
            {"Hide Vest", "Hide Shirt"},
            {"Hide Boots", "Hide Shoes"},
            {"Longsleeve T-Shirt", "Long TShirt"},
            {"Miners Hat", "Miner Hat"},
            {"Road Sign Jacket", "Roadsign Vest"},
            {"Road Sign Kilt", "Roadsign Pants"},
            {"Jacket", "Vagabond Jacket"},
            {"Boots", "Work Boots"},
            {"Assault Rifle", "AK47"},
            {"Bolt Action Rifle", "Bolt Rifle"},
            {"MP5A4", "Mp5"},
            {"Salvaged Sword", "Sword"},
            {"Python Revolver", "Python"},
            {"LR-300 Assault Rifle", "LR300"},
            {"M39 Rifle", "M39"},
            {"Rug Bear Skin", "Bearskin Rug"},
            {"Wood Double Door", "Wooden Double Door"},
            {"Pickaxe", "Pick Axe"},
            {"Stone Pickaxe", "Stone Pick Axe"},
            {"T-Shirt", "TShirt"}
        };

        private class SkinsJson
        {
            [JsonProperty(PropertyName = "Command")]
            public string Command = "skin";

            [JsonProperty(PropertyName = "Skins")]
            public List<SkinItem> Skins = new List<SkinItem>();

            [JsonProperty(PropertyName = "Container Panel Name")]
            public string Panel = "generic";

            [JsonProperty(PropertyName = "Container Capacity")]
            public int Capacity = 36;

            [JsonProperty(PropertyName = "UI")]
            public UIConfiguration UI = new UIConfiguration();

            [JsonProperty(PropertyName = "Debug")]
            public bool Debug = false;

            public class SkinItem
            {
                [JsonProperty(PropertyName = "Item Shortname")]
                public string Shortname = "shortname";

                [JsonProperty(PropertyName = "Skins")]
                public List<ulong> Skins = new List<ulong>();
            }

            public class UIConfiguration
            {
                [JsonProperty(PropertyName = "Background Color")]
                public string BackgroundColor = "0.18 0.28 0.36";

                [JsonProperty(PropertyName = "Background Anchors")]
                public Anchors BackgroundAnchors = new Anchors
                { AnchorMinX = "1.0", AnchorMinY = "1.0", AnchorMaxX = "1.0", AnchorMaxY = "1.0" };

                [JsonProperty(PropertyName = "Background Offsets")]
                public Offsets BackgroundOffsets = new Offsets
                { OffsetMinX = "-300", OffsetMinY = "-100", OffsetMaxX = "0", OffsetMaxY = "0" };

                [JsonProperty(PropertyName = "Left Button Text")]
                public string LeftText = "<size=36><</size>";

                [JsonProperty(PropertyName = "Left Button Color")]
                public string LeftColor = "0.11 0.51 0.83";

                [JsonProperty(PropertyName = "Left Button Anchors")]
                public Anchors LeftAnchors = new Anchors
                { AnchorMinX = "0.025", AnchorMinY = "0.05", AnchorMaxX = "0.325", AnchorMaxY = "0.95" };

                [JsonProperty(PropertyName = "Center Button Text")]
                public string CenterText = "<size=36>Page: {page}</size>";

                [JsonProperty(PropertyName = "Center Button Color")]
                public string CenterColor = "0.11 0.51 0.83";

                [JsonProperty(PropertyName = "Center Button Anchors")]
                public Anchors CenterAnchors = new Anchors
                { AnchorMinX = "0.350", AnchorMinY = "0.05", AnchorMaxX = "0.650", AnchorMaxY = "0.95" };

                [JsonProperty(PropertyName = "Right Button Text")]
                public string RightText = "<size=36>></size>";

                [JsonProperty(PropertyName = "Right Button Color")]
                public string RightColor = "0.11 0.51 0.83";

                [JsonProperty(PropertyName = "Right Button Anchors")]
                public Anchors RightAnchors = new Anchors
                { AnchorMinX = "0.675", AnchorMinY = "0.05", AnchorMaxX = "0.975", AnchorMaxY = "0.95" };

                public class Anchors
                {
                    [JsonProperty(PropertyName = "Anchor Min X")]
                    public string AnchorMinX = "0.0";

                    [JsonProperty(PropertyName = "Anchor Min Y")]
                    public string AnchorMinY = "0.0";

                    [JsonProperty(PropertyName = "Anchor Max X")]
                    public string AnchorMaxX = "1.0";

                    [JsonProperty(PropertyName = "Anchor Max Y")]
                    public string AnchorMaxY = "1.0";

                    [JsonIgnore]
                    public string AnchorMin => $"{AnchorMinX} {AnchorMinY}";

                    [JsonIgnore]
                    public string AnchorMax => $"{AnchorMaxX} {AnchorMaxY}";
                }

                public class Offsets
                {
                    [JsonProperty(PropertyName = "Offset Min X")]
                    public string OffsetMinX = "0";

                    [JsonProperty(PropertyName = "Offset Min Y")]
                    public string OffsetMinY = "0";

                    [JsonProperty(PropertyName = "Offset Max X")]
                    public string OffsetMaxX = "100";

                    [JsonProperty(PropertyName = "Offset Max Y")]
                    public string OffsetMaxY = "100";

                    [JsonIgnore]
                    public string OffsetMin => $"{OffsetMinX} {OffsetMinY}";

                    [JsonIgnore]
                    public string OffsetMax => $"{OffsetMaxX} {OffsetMaxY}";
                }
            }
        }

        class SkinList
        {
            public string name;
            public string shortname;
            public List<ulong> skinid = new List<ulong>(); 
        }

        public Form1()
        {
            InitializeComponent();

            steamcom = checkBox5.Checked;
            umod = checkBox6.Checked;
            commas = checkBox1.Checked;
            zero = checkBox2.Checked;
            last = checkBox3.Checked;
            json = checkBox4.Checked;
            if (radioButton1.Checked) SkinType = "accepted"; else SkinType = "all";
            if (radioButton3.Checked) Category = "selected"; else Category = "all";
            if (steamcom) groupBox1.Enabled = true; else groupBox1.Enabled = false;
            if (commas) checkBox3.Enabled = true; else checkBox3.Enabled = false;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (umod == false && steamcom == false)
            {
                MessageBox.Show("Выберите хотя-бы один ресурс, откуда будет собираться информация.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }           

            var umodhtml = @"https://umod.org/documentation/games/rust/definitions";
            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument htmlDoc;
            HtmlNodeCollection node, node2;

            try
            {
                htmlDoc = web.Load(umodhtml);
                node = htmlDoc.DocumentNode.SelectNodes("//h2[@class='has-anchors']");
                node2 = htmlDoc.DocumentNode.SelectNodes("//table[@id='skins-list']");
            }
            catch
            {
                MessageBox.Show("Невозможно получить доступ к сайту uMod.org или его элементам!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }         
            
            int j = 0;

            foreach (var a in node)
            {
                List<ulong> id = new List<ulong>();
                string str = a.InnerText;
                string shortname = str.Substring(0, str.Length - 1).Substring(str.IndexOf("(") + 1);
                string name = str.Substring(0, str.IndexOf("(")).Trim();

                //////////
                //фикс на дублирование списка скинов (из-за ошибки на сайте umod список скинов написан два раза)
                if (j > 0 && name == list[0].name) break;
                /////////

                if (umod)
                {
                    for (int i = 0; i < node2[j].LastChild.ChildNodes.Count; i++)
                    {
                        id.Add(ulong.Parse(node2[j].LastChild.ChildNodes[i].FirstChild.InnerText));
                    }
                    list.Add(new SkinList
                    {
                        name = name,
                        shortname = shortname,
                        skinid = id
                    });
                }
                else
                {
                    list.Add(new SkinList
                    {
                        name = name,
                        shortname = shortname
                    });
                }
                j++;
            }

            for (int i = 0; i < list.Count; i++)
            {
                checkedListBox1.Items.Add(list[i].name);
            }

            if (Category == "selected")
            {
                toolStripStatusLabel1.Text = "Теперь выберите нужные вам категории и нажмите Далее";
            }
            else
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, true);
                }
                if (json)
                {
                    toolStripStatusLabel1.Text = "Теперь выберите нужные вам категории и нажмите Далее";
                }
                else
                {
                    toolStripStatusLabel1.Text = "Нажимайте Далее, чтобы получить информацию по каждому предмету";
                }                              
            }
            
            if (checkedListBox1.Items.Count > 0)
            {
                groupBox3.Enabled = false;
                groupBox1.Enabled = false;
                groupBox4.Enabled = false;
                groupBox2.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = true;
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (cyclenumber == 0)
            {
                if (checkedListBox1.CheckedItems.Count == 0)
                {
                    MessageBox.Show("Выберите хотя-бы один предмет.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (var a in checkedListBox1.CheckedIndices)
                { 
                    checkeditems.Add(Convert.ToInt32(a));
                }

                if (steamcom)
                {
                    if (SkinType == "accepted")
                    {
                        steamurl += "&browsesort=accepted&browsefilter=accepted";
                    }
                    else
                    {
                        steamurl += "&actualsort=trend&browsesort=trend";
                    }
                }
                checkedListBox1.Enabled = false;
            }

            if (json) 
            {
                button2.Enabled = false;
                button3.Enabled = false;
                await Task.Run(() => CreateJson()); 
            }

            if (checkeditems.Count == cyclenumber)
            {
                button2.Enabled = false;
                toolStripStatusLabel1.Text = "Работа закончена!";
                if (json) 
                {
                    MessageBox.Show("Программа закончила свою работу.\nФайл Skins.json создан в папке с программой.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                } 
                else
                {
                    MessageBox.Show("Программа закончила свою работу.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }               
                return;
            }

            linkLabel1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            if (steamcom) await Task.Run(() => GetSteamData());
            ShowItemsData();
            progressBar1.Value = 0;
            cyclenumber++;
            linkLabel1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
        }

        private void GetSteamData()
        {
            string teg = list[checkeditems[cyclenumber]].name;
            if (steamnames.ContainsKey(teg)) teg = steamnames[teg];
            string steamurlloc = steamurl + @"&requiredtags[]=" + teg.Replace(" ", "+");

            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument htmlDoc;
            string str;

            try 
            {
                htmlDoc = web.Load(steamurlloc);
                str = htmlDoc.DocumentNode.SelectNodes("//div[@class='workshopBrowsePagingInfo']").First().InnerText;
            }
            catch
            {
                MessageBox.Show("Невозможно получить доступ к сайту steamcommunity или его элементам!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var df = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int allnum = Int32.Parse(df[3].Replace(",", ""));
            int lastpage;  

            if (htmlDoc.DocumentNode.SelectNodes("//a[@class='pagelink']") == null)
            {
                lastpage = 1;
            }
            else
            {
                lastpage = Int32.Parse(htmlDoc.DocumentNode.SelectNodes("//a[@class='pagelink']").Last().InnerText);
                if (popular && SkinType == "all")
                {
                    lastpage = Convert.ToInt32((((float)allnum / 100) * (float)numericUpDown1.Value) / 30);
                }              
            }
            
            var ugc = htmlDoc.DocumentNode.SelectNodes("//a[contains(concat(' ', @class, ' '), 'ugc')]");
            int ad = 1;

            if (popular)
            {
                allnum = lastpage * 30;
            }

            BeginInvoke(new MethodInvoker(delegate
            {
                progressBar1.Maximum = allnum;
            }));

            for (int i = 1; i < lastpage + 1; i++) 
            {
                HtmlAgilityPack.HtmlDocument htmlsteam;

                if (i > 1)
                {
                    try { 
                        htmlsteam = web.Load(steamurlloc + "&p=" + i);
                        ugc = htmlsteam.DocumentNode.SelectNodes("//a[contains(concat(' ', @class, ' '), 'ugc')]");
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно получить доступ к сайту steamcommunity или его элементам!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                foreach (var a in ugc)
                {
                    var find = list.Find(x => x.name == list[checkeditems[cyclenumber]].name);
                    ulong id = ulong.Parse(a.Attributes["data-publishedfileid"].Value);

                    if (umod)
                    {
                        if (!list.Exists(x => x.skinid.Contains(id))) find.skinid.Add(id);
                    }
                    else
                    {
                        find.skinid.Add(id);
                    }

                    BeginInvoke(new MethodInvoker(delegate
                    {
                        progressBar1.PerformStep();
                    }));

                    
                    Console.WriteLine(ad.ToString()+"/"+allnum.ToString());
                    ad++;
                }
                
            }
        }

        private void ShowItemsData()
        {          
            if (cyclenumber != checkeditems.Count)
            {
                string fulltext = "";
                string name = list[checkeditems[cyclenumber]].name;
                label7.Text = "Найдено скинов: " + list[checkeditems[cyclenumber]].skinid.Count;
                textBox2.Text = list[checkeditems[cyclenumber]].name;
                textBox3.Text = list[checkeditems[cyclenumber]].shortname;
                textBox1.Clear();
                browserurl = "";

                if (steamnames.ContainsKey(name)) name = steamnames[name];

                if (SkinType == "accepted")
                {
                    browserurl = steamurl + "&requiredtags[]=" + name.Replace(" ", "+");
                }
                else
                {
                    browserurl += steamurl + "&requiredtags[]=" + name.Replace(" ", "+");
                }

                if (zero)
                {
                    if (commas)
                    {
                        fulltext = "0,\r\n";
                    }
                    else
                    {
                        fulltext = "0\r\n";
                    }
                }

                for (int i = 0; i < list[checkeditems[cyclenumber]].skinid.Count; i++)
                {                     
                    string text = list[checkeditems[cyclenumber]].skinid[i].ToString();

                    if (commas) text += ",";

                    if (i != list[checkeditems[cyclenumber]].skinid.Count - 1)
                    {
                        fulltext += text + "\r\n";
                    }
                    else
                    {
                        fulltext += text;
                        if (last)
                        {
                           fulltext = fulltext.Substring(0, fulltext.Length - 1);
                        }
                    }
                }
                textBox1.Text = fulltext;
            }
        }

        private void CreateJson()
        {
            SkinsJson json = new SkinsJson();

            for (cyclenumber = 0; cyclenumber < checkeditems.Count; cyclenumber++) {

                BeginInvoke(new MethodInvoker(delegate
                {
                    toolStripStatusLabel1.Text = "Собираем информацию: собрано " + cyclenumber.ToString() + "/" + checkeditems.Count.ToString();
                    progressBar1.Value = 0;
                }));
                if (steamcom) GetSteamData();
                
                SkinsJson.SkinItem test = new SkinsJson.SkinItem
                {
                    Shortname = list[checkeditems[cyclenumber]].shortname,
                    Skins = list[checkeditems[cyclenumber]].skinid
                };

                json.Skins.Add(test);

               
            }
            string jsond = JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("Skins.json", jsond);
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                steamcom = true;
                groupBox1.Enabled = true;
            }
            else
            {
                steamcom = false;
                groupBox1.Enabled = false;
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                umod = true;
            }
            else
            {
                umod = false;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(browserurl);
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                popular = true;
            }
            else
            {
                popular = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox1.Text);
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Checked)
            {
                Category= "all";
            }
            else
            {
                Category = "selected";
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                commas = true;
                checkBox3.Enabled = true;
            }
            else
            {
                commas = false;

                checkBox3.Checked = false;
                checkBox3.Enabled = false;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                last = true;
            }
            else
            {
                last = false;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                json = true;

                checkBox1.Checked = false;
                checkBox1.Enabled = false;

                checkBox2.Checked = false;
                checkBox2.Enabled = false;

                checkBox3.Checked = false;
                checkBox3.Enabled = false;
            }
            else
            {
                json = false;
                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                zero = true;
            }
            else
            {
                zero = false;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Checked)
            {
                SkinType = "accepted";
            }
            else
            {
                SkinType = "all";
            }
        }
    }
}

