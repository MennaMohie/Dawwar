﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.IO;

namespace File_Search_Engine_System
{
    public partial class AddCategory : Form
    {
        public AddCategory()
        {
            InitializeComponent();
        }

        public static Dictionary<string, List<string>> D = new Dictionary<string, List<string>>();
        Dictionary<string, bool> CheckDict = new Dictionary<string, bool>();
        //Dictionary<string, string> mapOfKeywords = new Dictionary<string, string>();

        string categoryname;
        List<string> richlist = new List<string>();
        //List<string> l = new List<string>();

        private void AddCategory_Load(object sender, EventArgs e)
        {
            fillCatCombo();

            if (File.Exists("Categories.xml"))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("Categories.xml");

                XmlNodeList list = doc.GetElementsByTagName("Category");

                for (int i = 0; i < list.Count; i++)
                {
                    XmlNodeList child = list[i].ChildNodes;
                    for (int j = 0; j < child.Count; j++)
                    {

                        string value = child[0].InnerText;

                        CheckDict[value] = true;

                    }

                }
            }
        }

        private void homeButton_Click(object sender, EventArgs e)
        {
            Home home = new File_Search_Engine_System.Home();
            home.Show();
            this.Hide();
        }

        private void DoneButton_Click(object sender, EventArgs e)
        {
            richlist.Clear();

            if(CategorytextBox.Text.Length==0)
            {
                MessageBox.Show("Please enter category name.");
                return;
            }

            string[] RichTextBoxLines = richTextBox1.Lines;

            if(RichTextBoxLines.Length == 0)
            {
                MessageBox.Show("Please add at least one keyword.");
                return;
            }

            foreach (string line in RichTextBoxLines)
            {
                if(line.Length!=0)
                    richlist.Add(line);
            }

            if (!File.Exists("Categories.xml"))
            {
                XmlWriter writer = XmlWriter.Create("Categories.xml");
                writer.WriteStartDocument();

                if (CheckDict.ContainsKey(CategorytextBox.Text))
                    MessageBox.Show("This Category is already added!");

                else
                {
                    categoryname = CategorytextBox.Text;

                    D.Add(categoryname, richlist);
                    CheckDict[categoryname] = true;

                    Category C = new Category();
                    C.categoryName = categoryname;
                    foreach(string keyword in richlist)
                    {
                         C.keywords.Add(keyword);
                         //Home.mapOfKeywords.Add(keyword, C.categoryName);
                    }
                    Home.mapOfCategories[C.categoryName] = C;

                    catCombo.Items.Add(C.categoryName);

                    MessageBox.Show("This category is successfully added!");
                    writer.WriteStartElement("Categories");
                    writer.WriteStartElement("Category");

                    writer.WriteStartElement("CategoryName");
                    writer.WriteString(categoryname);
                    writer.WriteEndElement();
                    foreach (var x in D[categoryname])
                    {
                        writer.WriteStartElement("Keyword");
                        writer.WriteString(x);
                        writer.WriteEndElement();

                    }


                    writer.WriteEndElement();

                    writer.WriteEndElement();

                    writer.WriteEndDocument();
                    writer.Close();

                    CategorytextBox.Clear();
                    richTextBox1.Clear();
                }
            }
            else
            {


                XmlDocument doc = new XmlDocument();
                doc.Load("Categories.xml");

                if (CheckDict.ContainsKey(CategorytextBox.Text))
                    MessageBox.Show("This Category is already added!");
                else
                {
                    categoryname = CategorytextBox.Text;

                    D.Add(categoryname, richlist);
                    CheckDict[categoryname] = true;

                    Category C = new Category();
                    C.categoryName = categoryname;
                    foreach (string keyword in richlist)
                    {
                        C.keywords.Add(keyword);
                        //Home.mapOfKeywords.Add(keyword, C.categoryName);
                    }
                    Home.mapOfCategories[C.categoryName] = C;
                    catCombo.Items.Add(C.categoryName);

                    XmlElement cat = doc.CreateElement("Category");

                    XmlElement node = doc.CreateElement("CategoryName");
                    node.InnerText = categoryname;
                    cat.AppendChild(node);

                    foreach (var x in D[categoryname])
                    {
                        XmlElement node1 = doc.CreateElement("Keyword");
                        node1.InnerText = x;
                        cat.AppendChild(node1);
                    }
                    XmlElement root = doc.DocumentElement;
                    root.AppendChild(cat);
                    doc.Save("Categories.xml");

                    MessageBox.Show("Category is successfully added.");

                }
                CategorytextBox.Clear();
                richTextBox1.Clear();
            }
        }

        private void catCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            infoRichTextBox.Visible = true;
            infoRichTextBox.Clear();

            string selectedCat = catCombo.SelectedItem.ToString();


            List<string> keywords = new List<string>();
            keywords = Home.mapOfCategories[selectedCat].keywords;

            string CatNameValue = Home.mapOfCategories[selectedCat].categoryName;

            infoRichTextBox.Text += "Category Name: " + CatNameValue + Environment.NewLine;
            infoRichTextBox.Text += "Keywords: ";

            for (int i = 0; i < keywords.Count; i++)
            {
                if (i != keywords.Count - 1)
                    infoRichTextBox.Text += keywords[i] + ", ";

                else
                    infoRichTextBox.Text += keywords[i] + ".";

            }


            //coloring tag names
            int start = 0;
            int end = infoRichTextBox.Text.LastOrDefault();

            while (start < end)
            {
                infoRichTextBox.Find("Category Name:", start, infoRichTextBox.TextLength, RichTextBoxFinds.MatchCase);
                infoRichTextBox.SelectionColor = Color.DarkMagenta;
                infoRichTextBox.Find("Keywords:", start, infoRichTextBox.TextLength, RichTextBoxFinds.MatchCase);
                infoRichTextBox.SelectionColor = Color.DarkMagenta;


                start = infoRichTextBox.Text.LastOrDefault() + 1;
            }

        }

        private void filesButton_Click(object sender, EventArgs e)
        {
            AddFile form = new AddFile();
            form.Show();
            this.Hide();
        }

        private void AddCategory_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if(catCombo.SelectedIndex==-1)
            {
                MessageBox.Show("Please select a category from the combo box to be deleted.");
                return;
            }
            string delcategoryname;
            bool found = false;
            DialogResult result = MessageBox.Show("Are You Sure You Want To Delete This Category?", "Warning", MessageBoxButtons.YesNo);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("Categories.xml");
                XmlNodeList parent = doc.GetElementsByTagName("Category");
                for (int i = 0; i < parent.Count; i++)
                {
                    XmlNodeList childrens = parent[i].ChildNodes;
                    delcategoryname = childrens[0].InnerText;
                    if (delcategoryname == catCombo.SelectedItem.ToString())
                    {
                        parent[i].ParentNode.RemoveChild(parent[i]);

                        //foreach(string keyword in Home.mapOfCategories[delcategoryname].keywords)
                        //{
                        //    Home.mapOfKeywords.Remove(keyword);
                        //}

                        Home.mapOfCategories.Remove(delcategoryname);

                        catCombo.Items.Remove(delcategoryname);

                        found = true;
                    }

                }
                doc.Save("Categories.xml");
                if (found == true)
                    MessageBox.Show("Category Successfully Deleted.");
                else
                    MessageBox.Show("There's no category with this name!");

            }
        }

        private void fillCatCombo()
        {
            foreach (KeyValuePair<string, Category> KVP in Home.mapOfCategories)
            {
                catCombo.Items.Add(KVP.Key);
            }
        }

    }

}

