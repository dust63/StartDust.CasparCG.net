﻿using System;
using System.Linq;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using StarDust.CasparCG.net.Connection;
using StarDust.CasparCG.net.Device;

namespace StarDusrt.CasparCG.net.Demo.WPF.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        protected ICasparDevice casparDevice;
        private string _title = "WPF DEMO StarDust.CasparCG.net";
        private string _results;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string Results
        {
            get { return _results; }
            set { SetProperty(ref _results, value); }
        }

        public bool IsConnected => casparDevice.IsConnected;


        public DelegateCommand ConnectCommand { get; private set; }

        public DelegateCommand DisconnectCommand { get; private set; }

        public DelegateCommand VersionCommand { get; private set; }

        public DelegateCommand PlayCommand { get; private set; }


        public MainWindowViewModel(ICasparDevice casparDevice)
        {
            this.casparDevice = casparDevice;
            casparDevice.ConnectionStatusChanged += OnCasparCgConnectionStatusChanged;
            InitializeCommand();
        }



        private void InitializeCommand()
        {
            ConnectCommand = new DelegateCommand(OnConnectCommand, ()=> !casparDevice.IsConnected);
            DisconnectCommand = new DelegateCommand(OnDisconnectCommand, () => casparDevice.IsConnected);
            VersionCommand = new DelegateCommand(OnVersionCommand,()=> casparDevice.IsConnected);
            PlayCommand = new DelegateCommand(OnPlayCommand,()=> casparDevice.IsConnected);
        }

        private void OnDisconnectCommand()
        {
            this.casparDevice.Disconnect();
        }

        private void OnPlayCommand()
        {
            var clip = casparDevice.Mediafiles.FirstOrDefault(x=> x.Name.Equals("AMB", StringComparison.OrdinalIgnoreCase)) ?? casparDevice.Mediafiles.FirstOrDefault();
            if(clip == null)
                return;
            var channel = casparDevice.Channels.FirstOrDefault();
            channel?.Load(clip.FullName, false);
            var status = channel?.Play();
            AppendToResults($"Play {(status.GetValueOrDefault(false) ? "Ok": "Ko")}");
        }


        private void OnVersionCommand()
        {
            AppendToResults(casparDevice.Version);
        }

        private void OnConnectCommand()
        {
            if (casparDevice.IsConnected)
                return;


            casparDevice.Connect("127.0.0.1");
        }

        private void AppendToResults(string message)
        {
            Results = string.Concat(_results, Environment.NewLine, message);
        }

        private void OnCasparCgConnectionStatusChanged(object sender, ConnectionEventArgs e)
        {
            RaisePropertyChanged(nameof(IsConnected));
            ConnectCommand?.RaiseCanExecuteChanged();
            DisconnectCommand?.RaiseCanExecuteChanged();
            PlayCommand?.RaiseCanExecuteChanged();
            VersionCommand?.RaiseCanExecuteChanged();
        }
    }
}
