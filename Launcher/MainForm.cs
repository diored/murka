namespace DioRed.Murka.Launcher;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
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
}
