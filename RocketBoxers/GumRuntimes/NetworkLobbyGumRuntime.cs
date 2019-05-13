using RedGrin;
using RocketBoxers.Network;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace RocketBoxers.GumRuntimes
{
    public partial class NetworkLobbyGumRuntime
    {
        #region Events

        public event EventHandler StartGameClicked;

        #endregion

        public string EffectiveName
        {
            get
            {
                if (LocalGameRadioButton.FormsControl.IsChecked == true)
                {
                    return ClientNameTextBox.FormsControl.Text;
                }
                else if (ServerRadioButton.FormsControl.IsChecked == true)
                {
                    return ServerNameTextBox.FormsControl.Text;
                }
                else
                {
                    // no name, so just return whatever 
                    return "Me";
                }
            }
        }

        partial void CustomInitialize()
        {
            LocalGameRadioButton.FormsControl.Checked += HandleRadioButtonChecked;
            ServerRadioButton.FormsControl.Checked += HandleRadioButtonChecked;
            ClientRadioButton.FormsControl.Checked += HandleRadioButtonChecked;
            ClientPortTextBox.FormsControl.Text =
                NetworkManager.Self.Configuration.ApplicationPort.ToString();

            LocalGameRadioButton.FormsControl.IsChecked = true;

            ServerNameTextBox.FormsControl.TextChanged += HandleServerNameTextChange;
            IpTextBox.FormsControl.Text =
                NetworkManager.Self.GetLocalIpAddress();
            ServerPortTextBox.FormsControl.Text =
                NetworkManager.Self.Configuration.ApplicationPort.ToString();

            BeginHostingButton.FormsControl.Click += HandleBeginHostingClick;
            JoinAsClientButton.FormsControl.Click += HandleJoinAsClientClicked;

            NetworkLogger.Self.MessageAdded += HandleMessageAdded;
            NetworkManager.Self.Connections.CollectionChanged += HandleConnectionsChanged;

            StartButton.FormsControl.Click += (not, used) => StartGameClicked?.Invoke(this, null);

            // force show you
            HandleConnectionsChanged(null, null);
        }

        private void HandleConnectionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // for now just wipe it and refresh it, less efficient but easy:
            while (JoinedPlayerListContainer.Children.Count > 0)
            {
                JoinedPlayerListContainer.Children.RemoveAt(
                    JoinedPlayerListContainer.Children.Count - 1);
            }

            // add one for "you"
            var player = new GumRuntimes.LobbyConnectedPlayerRuntime();
            player.Text = "You";
            JoinedPlayerListContainer.Children.Add(player);

            foreach (var connection in NetworkManager.Self.Connections)
            {
                var connectedPlayer = new GumRuntimes.LobbyConnectedPlayerRuntime();
                connectedPlayer.Connection = connection;
                connectedPlayer.UpdateText();
                JoinedPlayerListContainer.Children.Add(connectedPlayer);
            }

            UpdateStartButtonVisibility();

        }

        private void HandleMessageAdded(object sender, EventArgs e)
        {

            var builder = new StringBuilder();

            foreach (var message in NetworkLogger.Self.Messages)
            {
                builder.AppendLine(message);
            }

            this.OutputTextInstance.Text = builder.ToString();
        }

        private void HandleRadioButtonChecked(object sender, EventArgs e)
        {
            ServerNameStack.Visible = ServerRadioButton.FormsControl.IsChecked == true;
            ServerButtonStack.Visible = ServerRadioButton.FormsControl.IsChecked == true;
            ServerPortStack.Visible = ServerRadioButton.FormsControl.IsChecked == true;

            IpStack.Visible = ClientRadioButton.FormsControl.IsChecked == true;
            ClientPlayerNameStack.Visible = ClientRadioButton.FormsControl.IsChecked == true;
            ClientButtonStack.Visible = ClientRadioButton.FormsControl.IsChecked == true;
            ClientPortStack.Visible = ClientRadioButton.FormsControl.IsChecked == true;

            UpdateStartButtonVisibility();
        }

        private void UpdateStartButtonVisibility()
        {
            StartButton.Visible =
                LocalGameRadioButton.FormsControl.IsChecked == true ||
                (ServerRadioButton.FormsControl.IsChecked == true &&
                    NetworkManager.Self.Connections.Count > 0);
        }

        private void HandleBeginHostingClick(object sender, EventArgs e)
        {
            if (int.TryParse(ServerPortTextBox.FormsControl.Text, out int parsedPort))
            {
                NetworkManager.Self.Configuration.ApplicationPort =
                    parsedPort;

                NetworkManager.Self.Initialize(RedGrin.NetworkRole.Server);
            }

        }

        private void HandleJoinAsClientClicked(object sender, EventArgs e)
        {
            if (int.TryParse(ClientPortTextBox.FormsControl.Text, out int parsedPort))
            {
                NetworkManager.Self.Configuration.ApplicationPort =
                    parsedPort;
                NetworkManager.Self.Initialize(RedGrin.NetworkRole.Client);
                var ipAddress = IpTextBox.Text;
                NetworkManager.Self.Connect(ipAddress);
            }
        }

        private void HandleServerNameTextChange(object sender, EventArgs e)
        {
            BeginHostingButton.FormsControl.IsEnabled = !string.IsNullOrEmpty(ServerNameTextBox.FormsControl.Text);
        }

        public void CustomActivity()
        {

        }

        public void CustomDestroy()
        {
            NetworkLogger.Self.MessageAdded -= HandleMessageAdded;
        }
    }
}
