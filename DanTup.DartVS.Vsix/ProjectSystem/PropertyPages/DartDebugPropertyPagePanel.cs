﻿namespace DanTup.DartVS.ProjectSystem.PropertyPages
{
	using System;
	using CommandLineBuilder = Microsoft.Build.Utilities.CommandLineBuilder;

	public partial class DartDebugPropertyPagePanel : DartPropertyPagePanel
	{
		public DartDebugPropertyPagePanel()
			: this(null)
		{
		}

		public DartDebugPropertyPagePanel(DartDebugPropertyPage parentPropertyPage)
			: base(parentPropertyPage)
		{
			InitializeComponent();
			UpdateStates();
			RefreshCommandLine();
		}

		public new DartDebugPropertyPage ParentPropertyPage
		{
			get
			{
				return (DartDebugPropertyPage)base.ParentPropertyPage;
			}
		}

		public StartAction StartAction
		{
			get
			{
				if (btnStartClass.Checked)
					return StartAction.Class;
				else if (btnStartProgram.Checked)
					return StartAction.Program;
				else if (btnStartBrowser.Checked)
					return StartAction.Browser;
				else
					return StartAction.Unknown;
			}

			set
			{
				switch (value)
				{
				case StartAction.Class:
					btnStartClass.Checked = true;
					break;

				case StartAction.Program:
					btnStartProgram.Checked = true;
					break;

				case StartAction.Browser:
					btnStartBrowser.Checked = true;
					break;

				case StartAction.Unknown:
				default:
					btnStartClass.Checked = false;
					btnStartProgram.Checked = false;
					btnStartBrowser.Checked = false;
					break;
				}
			}
		}

		public string StartClass
		{
			get
			{
				return txtStartClass.Text;
			}

			set
			{
				txtStartClass.Text = value;
			}
		}

		public string StartProgram
		{
			get
			{
				return txtStartProgram.Text;
			}

			set
			{
				txtStartProgram.Text = value;
			}
		}

		public string StartBrowserUrl
		{
			get
			{
				return txtStartBrowser.Text;
			}

			set
			{
				txtStartBrowser.Text = value;
			}
		}

		public string ExtraArguments
		{
			get
			{
				return txtExtraOptions.Text;
			}

			set
			{
				txtExtraOptions.Text = value;
			}
		}

		public string WorkingDirectory
		{
			get
			{
				return txtWorkingDirectory.Text;
			}

			set
			{
				txtWorkingDirectory.Text = value;
			}
		}

		public bool UseRemoteMachine
		{
			get
			{
				return chkUseRemoteMachine.Checked;
			}

			set
			{
				chkUseRemoteMachine.Checked = value;
			}
		}

		public string RemoteMachineName
		{
			get
			{
				return txtRemoteMachine.Text;
			}

			set
			{
				txtRemoteMachine.Text = value;
			}
		}

		public string VirtualMachineArguments
		{
			get
			{
				return txtJvmArguments.Text;
			}

			set
			{
				txtJvmArguments.Text = value;
			}
		}

		public string AgentArguments
		{
			get
			{
				return txtAgentArguments.Text;
			}

			set
			{
				txtAgentArguments.Text = value;
			}
		}

		private void RefreshCommandLine()
		{
			CommandLineBuilder commandLine = new CommandLineBuilder();

			switch (StartAction)
			{
			case StartAction.Class:
				var projectConfigs = ParentPropertyPage.Configurations;
				DartProjectConfig projectConfig = projectConfigs != null && projectConfigs.Count == 1 ? (DartProjectConfig)projectConfigs[0] : null;
				string javaPath = "javac.exe";
				commandLine.AppendFileNameIfNotNull(javaPath);

				commandLine.AppendSwitch("-agentpath:{AgentPath}");
				if (!string.IsNullOrEmpty(AgentArguments))
					commandLine.AppendTextUnquoted("=" + AgentArguments);

				if (!string.IsNullOrEmpty(VirtualMachineArguments))
					commandLine.AppendTextUnquoted(" " + VirtualMachineArguments);

				if (!string.IsNullOrEmpty(StartClass))
					commandLine.AppendFileNameIfNotNull(StartClass);

				break;

			case StartAction.Program:
			case StartAction.Browser:
				throw new NotSupportedException();

			case StartAction.Unknown:
			default:
				break;
			}

			if (!string.IsNullOrEmpty(ExtraArguments))
				commandLine.AppendTextUnquoted(" " + ExtraArguments);

			txtCommandLine.Text = commandLine.ToString();
		}

		private void UpdateStates()
		{
			// some options are not supported right now
			btnStartProgram.Enabled = false;
			txtStartProgram.Enabled = false;
			btnStartBrowser.Enabled = false;
			txtStartBrowser.Enabled = false;
			chkUseRemoteMachine.Enabled = false;
			txtRemoteMachine.Enabled = false;

			txtStartClass.Enabled = btnStartClass.Checked;
		}

		private void HandleStateAffectingChange(object sender, EventArgs e)
		{
			ParentPropertyPage.IsDirty = true;
			UpdateStates();
			RefreshCommandLine();
		}

		private void HandleCommandLineAffectingChange(object sender, EventArgs e)
		{
			ParentPropertyPage.IsDirty = true;
			RefreshCommandLine();
		}
	}
}
