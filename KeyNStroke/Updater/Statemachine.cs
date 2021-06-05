using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Xml;

namespace KeyNStroke.Updater
{

    /// <summary>
    /// This implements a singleton statemachine that handles the update process.
    /// 
    /// There is an API to connect this statemachine to one or more UIs.
    /// The UIs are synchronized, so any UI is allowed to trigger actions,
    /// and results will be displayed in all connected UIs at once.
    /// 
    /// The UI needs to provide the following widgets:
    ///      - A button with variable text that can be disabled.
    ///      - A oneline TextBlock for error/progress information.
    /// The UI needs to implement the following functionality:
    ///      - Call Updater.Instance.UiButton_Click() on Button click.
    ///      - Subscribe to the event Updater.Instance.onUiUpdate.
    ///      - On every event, update the UI elements according to the parameter <code>UiUpdateEventArgs</code> of this event.
    ///      - Call Updater.Instance.UiRefresh() once after setting up the event handler.
    ///      - Handle the fact that the callback calls from a different thread.
    ///      - Subscribe to the event Updater.Instance.onShutdownRequest and shutdown the application if this event occurs.
    /// </summary>
    public class Statemachine
    {

        #region Public API

        /// <summary>
        /// The UI state.
        /// One button and one oneline TextBlock must be available on each GUI that wants to provide a GUI for update functionality.
        /// (We abstract the UI state into this struct so we can embed the update functionality into multiple windows.)
        /// </summary>
        public class UiUpdateEventArgs
        {
            public string buttonText;
            public bool buttonEnabled;
            public string info;
            /// <summary>
            /// If this has more than 0 items the info shall be displayed as a hyperlink. On clicking the hyperlink, the details
            /// shall be displayed in a message window.
            /// </summary>
            public List<string> details;
        }

        public delegate void UiUpdateEventHandler(object sender, UiUpdateEventArgs e);
        public delegate void ShutdownRequestEventHandler(object sender);

        /// <summary>
        /// Event that is triggered whenever the UI should change or if UiRefresh is called.
        /// </summary>
        public event UiUpdateEventHandler OnUiUpdate;


        /// <summary>
        /// Event that is triggered when an update is impending and the application must exit immediately.
        /// </summary>
        public event ShutdownRequestEventHandler OnShutdownRequest;

        /// <summary>
        /// A function that should be called if the UI button is clicked.
        /// </summary>
        /// <param name="sender">Can be null.</param>
        /// <param name="e">Can be null.</param>
        public void UiButton_Click(object sender, RoutedEventArgs e)
        {
            switch (updateState)
            {
                case UpdateState.ManifestDownloadTriggerable:
                case UpdateState.NoUpdateAvailable:
                case UpdateState.ManifestDownloadFailed:
                case UpdateState.ManifestVerificationFailed:
                case UpdateState.UpdateVerificationFailed:
                case UpdateState.UpdateDownloadFailed:
                    updateCommands.Add(UpdateCommands.DownloadManifest);
                    break;
                case UpdateState.UpdateAvailable:
                    updateCommands.Add(UpdateCommands.DownloadUpdate);
                    break;
                case UpdateState.ManifestDownloading:
                case UpdateState.UpdateDownloading:
                case UpdateState.UpdateApplicationImpending:
                    break;
            }
        }

        const String UI_CHECK_FOR_UPDATES = "Check for updates";
        const String UI_NO_UPDATE_AVAILABLE = "No update available";
        const String UI_CHECKING_FOR_UPDATES = "Checking for updates...";
        const String UI_MANIFEST_DOWNLOAD_FAILED = "Manifest download failed.";
        const String UI_MANIFEST_VERIFICATION_FAILED = "Manifest verification failed.";
        const String UI_DO_UPDATE_TO_VERSION = "Update to version";
        const String UI_WHATS_NEW_IN_VERSION = "What's new in version";
        const String UI_UPDATE_DOWNLOAD_IN_PROGRESS = "Dowloading...";
        const String UI_UPDATE_DOWNLOAD_FAILED = "Update download failed.";
        const String UI_UPDATE_VERIFICATION_FAILED = "Update verification failed.";
        const String UI_UPDATE_IMPENDING = "Installing... The application will restart in a few moments.";

        /// <summary>
        /// Trigger the UiUpdateEventHandler event. This should be called after a new window connected its UI to the updater.
        /// </summary>
        public void UiRefresh()
        {
            UiUpdateEventArgs eargs = new UiUpdateEventArgs
            {
                info = "",
                details = new List<string>(0)
        };
            switch (updateState)
            {
                case UpdateState.ManifestDownloadTriggerable:
                    eargs.buttonText = UI_CHECK_FOR_UPDATES;
                    eargs.buttonEnabled = true;
                    break;
                case UpdateState.ManifestDownloading:
                    eargs.buttonText = UI_CHECK_FOR_UPDATES;
                    eargs.buttonEnabled = false;
                    eargs.info = UI_CHECKING_FOR_UPDATES;
                    break;
                case UpdateState.ManifestDownloadFailed:
                    eargs.buttonText = UI_CHECK_FOR_UPDATES;
                    eargs.buttonEnabled = true;
                    eargs.info = UI_MANIFEST_DOWNLOAD_FAILED;
                    eargs.details.Add(lastErrorMessage);
                    break;
                case UpdateState.ManifestVerificationFailed:
                    eargs.buttonText = UI_CHECK_FOR_UPDATES;
                    eargs.buttonEnabled = true;
                    eargs.info = UI_MANIFEST_VERIFICATION_FAILED;
                    eargs.details.Add(lastErrorMessage);
                    break;
                case UpdateState.NoUpdateAvailable:
                    eargs.buttonText = UI_CHECK_FOR_UPDATES;
                    eargs.buttonEnabled = true;
                    eargs.info = UI_NO_UPDATE_AVAILABLE;
                    break;
                case UpdateState.UpdateAvailable:
                    eargs.buttonText = UI_DO_UPDATE_TO_VERSION + " " + nextVersion;
                    eargs.buttonEnabled = true;
                    eargs.info = UI_WHATS_NEW_IN_VERSION + " " + nextVersion;
                    eargs.details.Add(nextVersionInfo);
                    break;
                case UpdateState.UpdateDownloading:
                    eargs.buttonText = UI_DO_UPDATE_TO_VERSION + " " + nextVersion;
                    eargs.buttonEnabled = false;
                    eargs.info = UI_UPDATE_DOWNLOAD_IN_PROGRESS + " (" + downloadPercentage.ToString() + "%)";
                    break;
                case UpdateState.UpdateVerificationFailed:
                    eargs.buttonText = UI_CHECK_FOR_UPDATES;
                    eargs.buttonEnabled = true;
                    eargs.info = UI_UPDATE_VERIFICATION_FAILED;
                    eargs.details.Add(lastErrorMessage);
                    break;
                case UpdateState.UpdateDownloadFailed:
                    eargs.buttonText = UI_CHECK_FOR_UPDATES;
                    eargs.buttonEnabled = true;
                    eargs.info = UI_UPDATE_DOWNLOAD_FAILED;
                    eargs.details.Add(lastErrorMessage);
                    break;
                case UpdateState.UpdateApplicationImpending:
                    eargs.buttonText = UI_CHECK_FOR_UPDATES;
                    eargs.buttonEnabled = false;
                    eargs.info = UI_UPDATE_IMPENDING;
                    break;
                default:
                    break;
            }

            OnUiUpdate?.Invoke(this, eargs);
        }

        #endregion

        #region Singleton

        private static readonly Statemachine instance = new Statemachine();

        /// <summary>
        /// Singleton
        /// </summary>
        public static Statemachine Instance
        {
            get { return instance; }
        }

        #endregion

        #region Constructor, Thread start

        private readonly Thread updateThread;

        private Statemachine()
        {
            updateThread = new Thread(new ThreadStart(UpdateThread));
            updateThread.IsBackground = true;
            updateThread.Start();
        }

        #endregion

        #region Variables 

        /// <summary>
        /// A queue for communication from UI to the Update Thread
        /// </summary>
        readonly BlockingCollection<UpdateCommands> updateCommands = new BlockingCollection<UpdateCommands>(new ConcurrentQueue<UpdateCommands>());

        #region Infos for User/UI

        /// <summary>
        /// The latest error message.
        /// </summary>
        string lastErrorMessage = "";

        /// <summary>
        /// The download progress during state UpdateDownloading
        /// </summary>
        int downloadPercentage = 0;

        /// <summary>
        /// The version number of the available update.
        /// </summary>
        string nextVersion = "";

        /// <summary>
        /// The changelog for the available update.
        /// </summary>
        string nextVersionInfo = "";

        #endregion

        #endregion




        private UpdateState updateState;

        public enum UpdateState
        {
            ManifestDownloadTriggerable,
            ManifestDownloading,
            ManifestDownloadFailed,
            ManifestVerificationFailed, // like ManifestDownloadTriggerable, but with error message
            NoUpdateAvailable,
            UpdateAvailable,
            UpdateDownloading,
            UpdateVerificationFailed, // like ManifestDownloadTriggerable, but with error message
            UpdateDownloadFailed,
            UpdateApplicationImpending,
        }

        private enum UpdateCommands
        {
            DownloadManifest,
            DownloadUpdate
        }

        void UpdateThread()
        {
            XmlDocument verifiedManifest = null;
            byte[] update_exe = null;

            while (true)
            {
                try
                {
                    // Blocks until at least one new Item is available
                    UpdateCommands command = updateCommands.Take();

                    // Only execute the last command in the queue, discard all others
                    while (updateCommands.Count > 0)
                    {
                        command = updateCommands.Take();
                    }

                    switch (updateState)
                    {
                        case UpdateState.ManifestDownloadTriggerable:
                        case UpdateState.ManifestDownloadFailed:
                        case UpdateState.ManifestVerificationFailed:
                        case UpdateState.UpdateVerificationFailed:
                        case UpdateState.UpdateDownloadFailed:
                        case UpdateState.NoUpdateAvailable:
                            if (command == UpdateCommands.DownloadManifest)
                            {
                                updateState = UpdateState.ManifestDownloading;
                                UiRefresh();
                                try
                                {
                                    XmlDocument m = Utils.DownloadAndVerifyManifest();
                                    if (Utils.CanUpdate(m))
                                    {
                                        updateState = UpdateState.UpdateAvailable;
                                        nextVersion = m.SelectSingleNode("manifest/update/version").InnerText;
                                        nextVersionInfo = m.SelectSingleNode("manifest/update/info").InnerText;
                                        verifiedManifest = m;
                                    }
                                    else
                                    {
                                        updateState = UpdateState.NoUpdateAvailable;
                                    }
                                }
                                catch (Utils.VerificationException e)
                                {
                                    updateState = UpdateState.ManifestVerificationFailed;
                                    lastErrorMessage = "Verification failed: \n" + e.Message;
                                }
                                catch (Utils.DownloadFailedException e)
                                {
                                    updateState = UpdateState.ManifestDownloadFailed;
                                    lastErrorMessage = "Download failed: \n" + e.Message;
                                }
                                catch (Exception e)
                                {
                                    updateState = UpdateState.ManifestDownloadFailed;
                                    lastErrorMessage = "Unexpected error: \n" + e.Message;
                                }
                            }
                            UiRefresh();
                            break;
                        case UpdateState.ManifestDownloading:
                        case UpdateState.UpdateDownloading:
                        case UpdateState.UpdateApplicationImpending:
                            // Will never happen here
                            break;
                        case UpdateState.UpdateAvailable:
                            if (command == UpdateCommands.DownloadUpdate)
                            {
                                updateState = UpdateState.UpdateDownloading;
                                UiRefresh();
                                try
                                {
                                    update_exe = Utils.DownloadExecutableAndVerifyHash(verifiedManifest, (progress) =>
                                    {
                                        downloadPercentage = progress;
                                        UiRefresh();
                                    }, new TimeSpan(0, 5, 0));
                                    updateState = UpdateState.UpdateApplicationImpending;
                                    UiRefresh();
                                    Updater.TriggerUpdateStep2(update_exe);
                                    OnShutdownRequest?.Invoke(this);
                                }
                                catch (Utils.VerificationException e)
                                {
                                    updateState = UpdateState.UpdateVerificationFailed;
                                    lastErrorMessage = "Verification failed: \n" + e.Message;
                                }
                                catch (Utils.DownloadFailedException e)
                                {
                                    updateState = UpdateState.UpdateDownloadFailed;
                                    lastErrorMessage = "Download failed: \n" + e.Message;
                                }
                                catch (Exception e)
                                {
                                    updateState = UpdateState.UpdateDownloadFailed;
                                    lastErrorMessage = "Unexpected error: \n" + e.Message;
                                }
                            }
                            UiRefresh();
                            break;
                    }

                }
                catch (InvalidOperationException)
                {
                    return;
                }
            }
        }

    }
}
