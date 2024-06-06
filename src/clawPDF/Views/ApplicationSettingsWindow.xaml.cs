﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using clawSoft.clawPDF.Core.Settings;
using clawSoft.clawPDF.Helper;
using clawSoft.clawPDF.Shared.Helper;
using clawSoft.clawPDF.ViewModels;

namespace clawSoft.clawPDF.Views
{
    internal partial class ApplicationSettingsWindow : Window
    {
        public ApplicationSettingsWindow()
        {
            InitializeComponent();
        }

        public ApplicationSettingsWindow(ApplicationSettings applicationSettings,
            ApplicationProperties applicationProperties, IEnumerable<ConversionProfile> conversionProfiles)
            : this()
        {
            GeneralTabUserControl.ViewModel.ApplicationSettings = applicationSettings;
            GeneralTabUserControl.ViewModel.ApplicationProperties = applicationProperties;
            GeneralTabUserControl.PreviewLanguageAction = PreviewLanguageAction;
            TitleTabUserControl.ViewModel.ApplyTitleReplacements(applicationSettings.TitleReplacement);
            DebugTabUserControl.ViewModel.ApplicationSettings = applicationSettings;
            DebugTabUserControl.UpdateSettings = UpdateSettingsAction;
            PrinterTabUserControl.ViewModel.ConversionProfiles = conversionProfiles;
            PrinterTabUserControl.ViewModel.ApplicationSettings = applicationSettings;
        }

        private ApplicationSettingsViewModel ViewModel => (ApplicationSettingsViewModel)DataContext;

        private void UpdateSettingsAction(clawPDFSettings settings)
        {
            GeneralTabUserControl.ViewModel.ApplicationSettings = settings.ApplicationSettings;
            GeneralTabUserControl.ViewModel.ApplicationProperties = settings.ApplicationProperties;
            TitleTabUserControl.ViewModel.ApplyTitleReplacements(settings.ApplicationSettings.TitleReplacement);
            PrinterTabUserControl.ViewModel.ConversionProfiles = settings.ConversionProfiles;
            PrinterTabUserControl.ViewModel.ApplicationSettings = settings.ApplicationSettings;
        }

        private void PreviewLanguageAction()
        {
            TranslationHelper.Instance.TranslatorInstance.Translate(this);
            TranslationHelper.Instance.TranslatorInstance.Translate(PrinterTabUserControl);
            PrinterTabUserControl.UpdateProfilesList();
            TranslationHelper.Instance.TranslatorInstance.Translate(TitleTabUserControl);
            TranslationHelper.Instance.TranslatorInstance.Translate(DebugTabUserControl);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SystemSetting setting = SystemConfig.Setting;
            setting.RisUrl = RisSystemTabUserControl.txtRisUrl.Text;
            setting.PrintWay = RisSystemTabUserControl._printWay;
            setting.PdfTabVisible = RisSystemTabUserControl.PdfTabVisible.IsChecked ?? false;
            setting.OCRTabVisible = RisSystemTabUserControl.OCRTabVisible.IsChecked ?? false;
            setting.ScriptActionVisible = RisSystemTabUserControl.ScriptActionVisible.IsChecked ?? false;
            setting.AttachmentActionVisible = RisSystemTabUserControl.AttachmentActionVisible.IsChecked ?? false;
            setting.BackgroundActionVisible = RisSystemTabUserControl.BackgroundActionVisible.IsChecked ?? false;
            setting.CoverActionVisible = RisSystemTabUserControl.CoverActionVisible.IsChecked ?? false;
            setting.EmailClientActionVisible = RisSystemTabUserControl.FtpActionVisible.IsChecked ?? false;
            setting.EmailSmtpActionVisible = RisSystemTabUserControl.FtpActionVisible.IsChecked ?? false;
            setting.FtpActionVisible = RisSystemTabUserControl.FtpActionVisible.IsChecked ?? false;
            SystemConfig.Save(setting);
            DialogResult = true;
        }

        private void ApplicationSettingsWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            IntPtr hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
            ThemeHelper.ChangeTitleBar(hWnd);

            TranslationHelper.Instance.TranslatorInstance.Translate(this);
        }

        private void ApplicationSettingsWindow_OnClosed(object sender, EventArgs e)
        {
            TranslationHelper.Instance.RevertTemporaryTranslation();
        }
    }
}