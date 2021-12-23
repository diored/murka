using Microsoft.Extensions.Configuration;

namespace DioRed.Murka.Launcher;

public partial class MainForm : Form
{
    private readonly bool _autoStart;

    public MainForm()
    {
        InitializeComponent();
    }

    public MainForm(IConfiguration configuration, bool autoStart)
        :this()
    {
        LoadWrapperAppProperties(serverWrapper, configuration.GetSection("wrappers:server"));
        LoadWrapperAppProperties(botWrapper, configuration.GetSection("wrappers:bot"));
        _autoStart = autoStart;
    }

    private void LoadWrapperAppProperties(ConsoleWrapper wrapper, IConfigurationSection configurationSection)
    {
        wrapper.AppPath = configurationSection["path"];
        wrapper.AppArguments = configurationSection["args"] ?? string.Empty;
        wrapper.AppProcessName = configurationSection["processName"];
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        ConsoleWrapper[] wrappers = { serverWrapper, botWrapper };

        DialogResult result;

        do
        {
            try
            {
                foreach (var wrapper in wrappers)
                {
                    if (wrapper.IsRunning)
                    {
                        wrapper.Stop();
                    }
                }

                result = DialogResult.OK;
            }
            catch (Exception ex)
            {
                result = MessageBox.Show($"Error occured:\n{ex.Message}\n", "Error occured", MessageBoxButtons.AbortRetryIgnore);

                if (result == DialogResult.Abort)
                {
                    e.Cancel = true;
                }
            }
        } while (result == DialogResult.Retry);
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        if (_autoStart)
        {
            serverWrapper.StartAsync();
            botWrapper.StartAsync();
        }
    }
}
