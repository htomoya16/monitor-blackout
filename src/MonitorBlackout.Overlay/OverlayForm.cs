using MonitorBlackout.Common;

namespace MonitorBlackout.Overlay;

public sealed class OverlayForm : Form
{
    private readonly Mutex _instanceMutex;
    private readonly EventWaitHandle _closeEvent;
    private readonly RegisteredWaitHandle _closeSignalRegistration;
    private bool _isClosing;

    public OverlayForm(MonitorTarget monitor, Mutex instanceMutex, EventWaitHandle closeEvent)
    {
        _instanceMutex = instanceMutex;
        _closeEvent = closeEvent;

        StartPosition = FormStartPosition.Manual;
        Location = monitor.Bounds.Location;
        Size = monitor.Bounds.Size;

        FormBorderStyle = FormBorderStyle.None;
        BackColor = Color.Black;
        TopMost = true;
        ShowInTaskbar = false;

        MouseDown += (_, _) => CloseOverlay();

        _closeSignalRegistration = ThreadPool.RegisterWaitForSingleObject(
            _closeEvent,
            (_, _) => RequestCloseFromSignal(),
            null,
            Timeout.Infinite,
            executeOnlyOnce: true);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _closeSignalRegistration.Unregister(null);
        _closeEvent.Dispose();

        try
        {
            _instanceMutex.ReleaseMutex();
        }
        catch (ApplicationException)
        {
            // Ignore if mutex ownership was already released.
        }

        _instanceMutex.Dispose();
        base.OnFormClosed(e);
    }

    private void RequestCloseFromSignal()
    {
        if (!IsHandleCreated || IsDisposed)
        {
            return;
        }

        try
        {
            BeginInvoke(CloseOverlay);
        }
        catch (InvalidOperationException)
        {
            // Ignore if form is closing while signal callback arrives.
        }
    }

    private void CloseOverlay()
    {
        if (_isClosing)
        {
            return;
        }

        _isClosing = true;
        Close();
    }
}
