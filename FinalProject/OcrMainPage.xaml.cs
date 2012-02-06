// -
// <copyright file="MainPage.xaml.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -
using System;
using System.Diagnostics;
using System.Windows;
using Hawaii.Services.Client;
using Hawaii.Services.Client.Ocr;
using Microsoft.Phone.Controls;
using GPS.Controls;
using GPS.Data;
using GPS.Utils;
using GPS.ViewModels;
using System.Text;
using UsingBingMaps.REST;

namespace GPS
{
    /// <summary>
    /// MainPage class contains the code behind for the MainPage user control.
    /// The MainPage user control is the main user control used by this application.
    /// </summary>
    public partial class OcrMainPage : PhoneApplicationPage
    {
        private OcrData ocrData;
        private OcrConversionStateManager ocrConversionStateManager;
        private FeedBackUtil user_feedback = FeedBackUtil.Instance;

        /// <summary>
        /// Initializes a new instance of the MainPage class.
        /// </summary>
        public OcrMainPage()
        {
            InitializeComponent();

            if (this.VerifyHawaiiAppId())
            {
                this.ocrData = OcrData.Instance;
                this.ocrData.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.OcrData_PropertyChanged);

                this.ocrConversionStateManager = OcrConversionStateManager.Instance;
                this.ocrConversionStateManager.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.ConversionStateManager_PropertyChanged);

                this.mainPivot.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.MainPivot_SelectionChanged);
            }

            Debug.WriteLine("----------------------------\ntesting REST CLIENT\n------------------------------------");
            RestClient rc = new RestClient();
        }

        /// <summary>
        /// This method is called when the hardware back key is pressed.
        /// </summary>
        /// <param name="e">
        /// Allows us to cancel the request to indicate that it was handled by the application.
        /// </param>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            // If we are currently showing a pivot item other than the main one we'll react 
            // to the back button by sliding to the main pivot item and in the same time we'll 
            // prevent the app from exiting.
            if (this.mainPivot.SelectedIndex > 0)
            {
                this.mainPivot.SelectedIndex = 0;
                e.Cancel = true;
            }
        }

        private bool VerifyHawaiiAppId()
        {
            if (!String.IsNullOrEmpty(HawaiiClient.HawaiiApplicationId))
            {
                return true;
            }
            else
            {
                this.HawaiiAppIdErrorArea.Visibility = Visibility.Visible;
                this.ContentPanel.Visibility = Visibility.Collapsed;

                return false;
            }
        }

        private void ConversionStateManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "OcrConversionState")
            {
                if (this.ocrConversionStateManager.OcrConversionState != OcrConversionState.Converting)
                {

                
                    this.Dispatcher.BeginInvoke(() => { this.mainPivot.SelectedIndex = 2; });
                }

                this.UpdateApplicationBarVisibility();
            }
        }

        private void MainPivot_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.UpdateApplicationBarVisibility();
        }

        private void UpdateApplicationBarVisibility()
        {
            this.ApplicationBar.IsVisible = this.mainPivot.SelectedIndex == 2 && this.ocrConversionStateManager.OcrConversionState == OcrConversionState.ConversionOK;
        }

        private void OcrData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PhotoStream")
            {
                if (this.ocrData.PhotoStream != null)
                {
                    this.UpdateAfterPhotoStreamChanged();
                }
            }
        }

        private void UpdateAfterPhotoStreamChanged()
        {
            this.EnsurePhotoAreaExists();
            this.EnsureTextAreaExists();
            this.StartOcrConversion();
        }

        private void StartOcrConversion()
        {
            OcrService.RecognizeImageAsync(
                HawaiiClient.HawaiiApplicationId,
                OcrClientUtils.GetPhotoBits(this.ocrData.PhotoStream),
                (output) => 
                {
                    // This section defines the body of what is known as an anonymous method. 
                    // This anonymous method is the callback method 
                    // called on the completion of the OCR process.
                    // Using Dispatcher.BeginInvoke ensures that 
                    // OnOcrCompleted is invoked on the Main UI thread.
                    this.Dispatcher.BeginInvoke(() => OnOcrCompleted(output)); 
                });

            this.ocrConversionStateManager.OcrConversionState = OcrConversionState.Converting;
            this.Dispatcher.BeginInvoke(() => { this.mainPivot.SelectedIndex = 1; });
        }

        private void OnOcrCompleted(OcrServiceResult result)
        {
            string address = " ";
            Debug.Assert(result != null, "result is null");

            if (result.Status == Status.Success)
            {
                this.ocrData.SetOcrResult(result.OcrResult.OcrTexts);

                if (this.ocrData.GetWordCount() > 0)
                {
                    int wordcount = 0;
                    StringBuilder sb = new StringBuilder();
                    // TODO TO STRING
                    foreach (OcrText ocr in result.OcrResult.OcrTexts)
                    {
                        address += ocr.Text;
                        wordcount += ocr.Words.Count;
                        Debug.WriteLine(ocr.Text);
                        sb.Append(ocr.Text); // ocr text
                    }
                    this.ocrConversionStateManager.OcrConversionState = OcrConversionState.ConversionOK;
                }
                else
                {
                    this.ocrConversionStateManager.OcrConversionState = OcrConversionState.ConversionEmpty;
                }
            }
            else
            {
                this.ocrConversionStateManager.OcrConversionState = OcrConversionState.ConversionError;
                this.ocrConversionStateManager.OcrConversionErrorMessage = result.Exception.Message;
            }

            NavigationService.Navigate(new Uri("/AddressPlotting.xaml?from=button" + "&longitude=" + address, UriKind.RelativeOrAbsolute));
           
        }

        private void EnsurePhotoAreaExists()
        {
            if (this.mainPivot.Items.Count < 2)
            {
                PivotItem photoAreaPivot = new PivotItem();
                photoAreaPivot.Margin = new Thickness(0);
                photoAreaPivot.Header = "Photo";
                this.mainPivot.Items.Add(photoAreaPivot);

                photoAreaPivot.Content = new PhotoArea();
            }
        }

        private void EnsureTextAreaExists()
        {
            if (this.mainPivot.Items.Count < 3)
            {
                PivotItem textAreaPivot = new PivotItem();
                textAreaPivot.Margin = new Thickness(0);
                textAreaPivot.Header = "Text";
                this.mainPivot.Items.Add(textAreaPivot);

                textAreaPivot.Content = new TextArea();
            }
        }

        private void ApBarSimpleMode_Click(object sender, EventArgs e)
        {
            this.SetTextAreaMode(TextViewMode.Simple);
        }

        private void AppBarDetailedMode_Click(object sender, EventArgs e)
        {
            this.SetTextAreaMode(TextViewMode.Detailed);
        }

        private void SetTextAreaMode(TextViewMode textViewMode)
        {
            if (this.mainPivot.Items.Count >= 3)
            {
                PivotItem textAreaPivot = (PivotItem)this.mainPivot.Items[2];
                TextArea textArea = (TextArea)textAreaPivot.Content;
                TextAreaViewModel textAreaViewModel = (TextAreaViewModel)textArea.DataContext;
                textAreaViewModel.TextViewMode = textViewMode;
            }
        }
    }
}
