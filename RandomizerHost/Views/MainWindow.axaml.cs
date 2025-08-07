using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace RandomizerHost.Views
{
    public class MainWindow : Window
    {

    public async Task NotifyUpdateAsync(string latestVersion)
    {
      var yesButton = new Button { Content = "Yes", HorizontalAlignment = HorizontalAlignment.Right };
      var noButton = new Button { Content = "No", HorizontalAlignment = HorizontalAlignment.Left };

      var stackPanel = new StackPanel
      {
        Children =
    {
        new TextBlock
        {
            Text = $"A new version ({latestVersion}) is available. Would you like to update?",
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 10)
        },
        new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Children = { yesButton, noButton }
        }
    }
      };

      var dialog = new Window
      {
        Content = stackPanel,
        Width = 300,
        Height = 150
      };

      var tcs = new TaskCompletionSource<bool>();
      yesButton.Click += (_, __) =>
      {
        tcs.SetResult(true);
        dialog.Close();
      };
      noButton.Click += (_, __) =>
      {
        tcs.SetResult(false);
        dialog.Close();
      };

      dialog.Show();
      var result = await tcs.Task;
    }

    public async Task DownloadUpdateAsync(string downloadUrl, string savePath)
    {
      using var client = new HttpClient();
      var response = await client.GetAsync(downloadUrl);
      response.EnsureSuccessStatusCode();

      await using var fileStream = new FileStream(savePath, FileMode.Create);
      await response.Content.CopyToAsync(fileStream);
    }
    public async Task DownloadAndInstallUpdateAsync()
    {
      string downloadUrl = "https://github.com/squid-man/MegaMan2Randomizer2/releases"; // URL
      string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RandomizerHost", "latest-version.zip");
      await DownloadUpdateAsync(downloadUrl, savePath);
      // Logic to install the update (e.g., extract and replace files)
      // This is a placeholder; actual implementation will depend on your update mechanism
      Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{savePath}\""));
    }

    //
    // Constructor
    //

    public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }


        //
        // Initialization
        //

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            ///
            // Set up drag and drop for the rom file path text box
            TextBox textBoxRomFile = this.Find<TextBox>("TextBox_RomFile");
            DragDrop.SetAllowDrop(textBoxRomFile, true);
            textBoxRomFile.AddHandler(DragDrop.DragOverEvent, this.DragOver);
            textBoxRomFile.AddHandler(DragDrop.DropEvent, this.Drop);
            textBoxRomFile.PropertyChanged += this.TextBoxRomFile_PropertyChanged;
        }

        private void TextBoxRomFile_PropertyChanged(Object? sender, AvaloniaPropertyChangedEventArgs? e)
        {
        }

        private void DragOver(Object? sender, DragEventArgs? in_DragEventArgs)
        {
            Debug.Assert(in_DragEventArgs is not null);

            // Only allow if the dragged data contains text or filenames
            if (true == in_DragEventArgs.Data.Contains(DataFormats.Text) ||
                true == in_DragEventArgs.Data.Contains(DataFormats.FileNames))
            {
                // Only allow copy or link as drop operations
                in_DragEventArgs.DragEffects = in_DragEventArgs.DragEffects & (DragDropEffects.Copy | DragDropEffects.Link);
            }
            else
            {
                in_DragEventArgs.DragEffects = DragDropEffects.None;
            }
        }


        private void Drop(Object? in_Sender, DragEventArgs? in_DragEventArgs)
        {
            TextBox romFile = (TextBox)(in_Sender!);

            Debug.Assert(in_DragEventArgs is not null);

            if (true == in_DragEventArgs.Data.Contains(DataFormats.Text))
            {
                romFile.Text = in_DragEventArgs.Data.GetText();
            }
            else if (true == in_DragEventArgs.Data.Contains(DataFormats.FileNames))
            {
                var fileName = in_DragEventArgs.Data.GetFileNames()!.First();
                if (fileName is not null)
                    romFile.Text = fileName;
            }
        }
    }
}
