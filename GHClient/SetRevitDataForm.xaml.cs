﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LMNA.Lyrebird.LyrebirdCommon;

namespace LMNA.Lyrebird.GH
{
    /// <summary>
    /// Interaction logic for SetRevitDataForm.xaml
    /// </summary>
    public partial class SetRevitDataForm
    {
        static LinearGradientBrush enterBrush;
        readonly GHClient parent;
        readonly LyrebirdChannel channel;

        string category;
        int categoryId;
        string familyName;
        RevitObject familyObj;
        readonly List<RevitObject> familyNames;
        string typeName;
        List<string> typeNames;
        List<RevitParameter> parameters;
        List<RevitParameter> usedParameters = new List<RevitParameter>();

        public List<RevitParameter> SelectedParameters
        {
            get { return usedParameters; }
            set { usedParameters = value; }
        }

        public SetRevitDataForm(LyrebirdChannel c, GHClient p)
        {
            parent = p;
            channel = c;


            InitializeComponent();

            // Position the form.
            Left = 0;
            Top = 0;
            //MessageBox.Show(string.Format("The system is {0} and the endpoint is:\n{1}", client.State.ToString(), client.Endpoint.Address.ToString()));
            if (channel != null)
            {
                try
                {
                    string test = channel.DocumentName();
                    if (test == null)
                    {
                        MessageBox.Show("Error!!!!!!!!!!");
                    }
                    RevitObject[] temp = channel.FamilyNames().ToArray();
                    if (temp != null && temp.Any())
                        familyNames = channel.FamilyNames().ToList();
                    else if (temp == null)
                    {
                        MessageBox.Show("the array is null");
                    }
                    else
                    {
                        MessageBox.Show("The array is empty");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error\n" + ex.ToString());
                }
                if (familyNames != null && familyNames.Count > 0)
                {
                    familyComboBox.ItemsSource = familyNames;
                    familyComboBox.DisplayMemberPath = "FamilyName";
                    bool check = false;
                    if (parent.FamilyName != null)
                    {
                        for (int i = 0; i < familyNames.Count; i++)
                        {
                            string famName = familyNames[i].FamilyName;
                            if (famName == parent.FamilyName)
                            {
                                familyComboBox.SelectedIndex = i;
                                check = true;
                            }
                        }
                    }
                    if (!check)
                    {
                        familyComboBox.SelectedIndex = 0;
                    }
                }
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void logo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://lmnts.lmnarchitects.com");
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void cancelButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (enterBrush == null)
            {
                enterBrush = EnterBrush();
            }
            cancelRect.Fill = enterBrush;
        }

        private void cancelButton_MouseLeave(object sender, MouseEventArgs e)
        {
            cancelRect.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            parent.FamilyName = familyName;
            parent.TypeName = typeName;
            parent.Category = category;
            parent.CategoryId = categoryId;
            //RevitParameter rp = new RevitParameter();
            parent.InputParams = usedParameters;
            Close();
        }

        private void okButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (enterBrush == null)
            {
                enterBrush = EnterBrush();
            }
            okRect.Fill = enterBrush;
        }

        private void okButton_MouseLeave(object sender, MouseEventArgs e)
        {
            okRect.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                parameters = new List<RevitParameter>();
                parameters = channel.Parameters(familyComboBox.SelectedItem as RevitObject, typeComboBox.SelectedItem as string);
                SelectParameterForm parameterForm = new SelectParameterForm(this, parameters, usedParameters);
                parameterForm.ShowDialog();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error\n\n" + ex.Message);
            }
            // Regenerate list view with parameters.
        }

        private void addButton_MouseEnter(object sender, MouseEventArgs e)
        {
            LinearGradientBrush brush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };
            
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 232, 232, 232), 0.0));
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 180, 180, 180), 1.0));

            addRect.Fill = brush;
        }

        private void addButton_MouseLeave(object sender, MouseEventArgs e)
        {
            addRect.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        private LinearGradientBrush EnterBrush()
        {
            LinearGradientBrush brush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };
            
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 180, 180, 180), 0.0));
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 232, 232, 232), 1.0));
            return brush;
        }

        private void familyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RevitObject fam = familyComboBox.SelectedItem as RevitObject;
            if (fam != null)
            {
                familyName = fam.FamilyName;
                category = fam.Category;
                catNameLabel.Content = "[ " + category + " ]"; 
                categoryId = fam.CategoryId;
                familyObj = fam;
            }

            // get the type names
            typeNames = new List<string>();
            typeNames = channel.TypeNames(familyObj).ToList();
            if (typeNames != null && typeNames.Count > 0)
            {
                typeComboBox.ItemsSource = typeNames;
                typeComboBox.SelectedIndex = 0;

                string type = parent.TypeName;
                if (!string.IsNullOrEmpty(type))
                {
                    for (int i = 0; i < typeNames.Count; i++)
                    {
                        string t = typeNames[i];
                        if (type == t)
                        {
                            typeComboBox.SelectedIndex = i;
                            usedParameters = parent.InputParams;
                            AddControls();
                        }
                    }
                }
            }
        }

        private void typeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string type = typeComboBox.SelectedItem as string;
            typeName = type;

            if (typeName != null)
            {
                parameters = new List<RevitParameter>();
                parameters = channel.Parameters(familyObj, typeName);
            }
        }

        public void AddControls()
        {
            controlPanel.Children.Clear();
            usedParameters.Sort((x, y) => String.CompareOrdinal(x.ParameterName, y.ParameterName));
            if (usedParameters.Count > 0)
            {
                for (int i = 1; i <= usedParameters.Count; i++)
                {
                    ParameterControl control = new ParameterControl(usedParameters[i - 1])
                    {
                        Margin = new Thickness(0, 0, 0, 0)
                    };
                    
                    controlPanel.Children.Add(control);
                }
            }
        }
    }
}
