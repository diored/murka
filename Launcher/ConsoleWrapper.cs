using CliWrap;
using CliWrap.EventStream;

using System.ComponentModel;
using System.Diagnostics;

namespace DioRed.Murka.Launcher;

public partial class ConsoleWrapper : UserControl
{
    private CancellationTokenSource? _cts;

    private bool _isRunning;

    public ConsoleWrapper()
    {
        AppPath = string.Empty;
        AppArguments = string.Empty;

        InitializeComponent();

        logList.DrawMode = DrawMode.OwnerDrawFixed;
        logList.DrawItem += new DrawItemEventHandler(logList_DrawItem);

        startButton.Click += async (_, _) => await StartAsync();
        stopButton.Click += (_, _) => Stop();
        clearButton.Click += (_, _) => ClearLog();
        killButton.Click += (_, _) => Kill();
    }

    public string AppPath { get; set; }
    public string AppArguments { get; set; }
    public string? AppProcessName { get; set; }

    public async Task StartAsync()
    {
        Command cmd = Cli.Wrap(AppPath).WithArguments(AppArguments);

        if (AppPath is not null)
        {
            cmd = cmd.WithWorkingDirectory(Path.GetDirectoryName(AppPath)!);
        }

        cmd = cmd.WithValidation(CommandResultValidation.None);

        IsRunning = true;
        _cts = new CancellationTokenSource();

        try
        {
            await foreach (var cmdEvent in cmd.ListenAsync(_cts.Token))
            {
                var logItem = cmdEvent switch
                {
                    StartedCommandEvent started => new LogItem($"Started", LogItemType.System),
                    StandardOutputCommandEvent stdOut => new LogItem(stdOut.Text, LogItemType.Output),
                    StandardErrorCommandEvent stdErr => new LogItem(stdErr.Text, LogItemType.Error),
                    ExitedCommandEvent exited => new Func<LogItem>(() =>
                    {
                        IsRunning = false;
                        return new LogItem($"Stopped (code {exited.ExitCode})", LogItemType.System);
                    })(),
                    _ => null
                };

                if (logItem is not null)
                {
                    logList.Items.Add(logItem);
                }
            }
        }
        catch
        { }
    }

    public void Stop()
    {
        _cts?.Cancel();
        logList.Items.Add(new LogItem("Stopped", LogItemType.System));

        IsRunning = false;
    }

    public void ClearLog()
    {
        logList.Items.Clear();
    }

    public void Kill()
    {
        foreach (var process in GetAppProcesses())
        {
            process.Kill();
            logList.Items.Add(new LogItem($"Process {process.Id} was killed", LogItemType.System));
        }
    }

    private ICollection<Process> GetAppProcesses()
    {
        List<Process> appProcesses = new();

        foreach (var process in Process.GetProcessesByName(AppProcessName))
        {
            try
            {
                if (process.MainModule?.FileName == AppPath)
                {
                    appProcesses.Add(process);
                }
            }
            catch { }
        }

        return appProcesses;
    }

    [EditorBrowsable(EditorBrowsableState.Always)]
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Bindable(true)]
    public override string Text
    {
        get => titleLabel.Text;
        set => titleLabel.Text = value;
    }

    public bool IsRunning
    {
        get => _isRunning;

        private set
        {
            _isRunning = value;

            startButton.Enabled = !value;
            stopButton.Enabled = value;
        }
    }

    private void logList_DrawItem(object? sender, DrawItemEventArgs e)
    {
        if (e.Index == -1) return;

        e.DrawBackground();

        var item = logList.Items[e.Index];

        (string text, LogItemType type) = item is LogItem logItem
            ? (logItem.Text, logItem.Type)
            : (item.ToString()!, LogItemType.Output);

        Brush myBrush = type switch
        {
            LogItemType.Error => Brushes.Red,
            LogItemType.System => Brushes.Blue,
            _ => Brushes.Black
        };

        e.Graphics.DrawString(text, e.Font ?? Font, myBrush, e.Bounds, StringFormat.GenericDefault);

        e.DrawFocusRectangle();
    }
}
