using MonitorBlackout.Common;

namespace MonitorBlackout.Overlay;

public sealed class OverlayForm : Form
{
    // Overlay の生存期間と同期する OS オブジェクト。
    private readonly Mutex _instanceMutex;
    private readonly EventWaitHandle _closeEvent;
    private readonly RegisteredWaitHandle _closeSignalRegistration;
    private bool _isClosing;

    public OverlayForm(MonitorTarget monitor, Mutex instanceMutex, EventWaitHandle closeEvent)
    {
        _instanceMutex = instanceMutex;
        _closeEvent = closeEvent;

        // 対象モニタの bounds にピッタリ合わせる。
        StartPosition = FormStartPosition.Manual;
        Location = monitor.Bounds.Location;
        Size = monitor.Bounds.Size;

        // 枠なし・黒背景・最前面で「黒幕」ウィンドウ化する。
        FormBorderStyle = FormBorderStyle.None;
        BackColor = Color.Black;
        TopMost = true;
        ShowInTaskbar = false;

        // 全域クリックで即終了（詰み防止）。
        MouseDown += (_, _) => CloseOverlay();

        // Toggle からの OFF シグナルを受けたら閉じる。
        _closeSignalRegistration = ThreadPool.RegisterWaitForSingleObject(
            _closeEvent,
            (_, _) => RequestCloseFromSignal(),
            null,
            Timeout.Infinite,
            executeOnlyOnce: true);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        // 待機登録と同期オブジェクトを解放する。
        _closeSignalRegistration.Unregister(null);
        _closeEvent.Dispose();

        try
        {
            _instanceMutex.ReleaseMutex();
        }
        catch (ApplicationException)
        {
            // すでに所有権が解放済みなら無視する。
        }

        _instanceMutex.Dispose();
        base.OnFormClosed(e);
    }

    private void RequestCloseFromSignal()
    {
        // フォーム破棄中なら UI 呼び出しを行わない。
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
            // シグナル到着とフォーム終了が競合した場合は無視する。
        }
    }

    private void CloseOverlay()
    {
        // 二重 Close を防ぐ。
        if (_isClosing)
        {
            return;
        }

        _isClosing = true;
        Close();
    }
}
