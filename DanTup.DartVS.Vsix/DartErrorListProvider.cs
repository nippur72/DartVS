﻿using System;
using System.Linq;
using DanTup.DartAnalysis;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace DanTup.DartVS
{
	internal class DartErrorListProvider
	{
		DTE dte;
		ErrorListProvider errorProvider;

		internal DartErrorListProvider(DTE dte, DartPackage package)
		{
			this.dte = dte;
			errorProvider = new ErrorListProvider(package);
		}

		internal void UpdateErrors(AnalysisErrorsNotification errorNotification)
		{
			errorProvider.SuspendRefresh();
			RemoveErrorsForFile(errorNotification.File);

			var errorTasks = errorNotification.Errors.Select(CreateErrorTask);

			foreach (var error in errorTasks)
			{
				error.Navigate += (s, e) =>
				{
					error.Line++; // Line number seems 0-based in most places, but Navigate didn't get the memo :(
					errorProvider.Navigate(error, new Guid(EnvDTE.Constants.vsViewKindCode));
					error.Line--;
				};
				errorProvider.Tasks.Add(error);
			}
			errorProvider.ResumeRefresh();
			errorProvider.Show();
			errorProvider.ForceShowErrors();
		}

		void RemoveErrorsForFile(string path)
		{
			var staleTasks = errorProvider.Tasks.OfType<ErrorTask>().Where(t => t.Document == path).ToArray();
			foreach (var staleTask in staleTasks)
				errorProvider.Tasks.Remove(staleTask);
		}

		private ErrorTask CreateErrorTask(AnalysisError analysisError)
		{
			return new ErrorTask
			{
				// TODO: This is bogus; they don't start with this anymore...
				ErrorCategory =
					analysisError.Message.StartsWith("hint") ? TaskErrorCategory.Message
					: analysisError.Message.StartsWith("warning") ? TaskErrorCategory.Warning
					: TaskErrorCategory.Error,
				Text = analysisError.Message,
				Document = analysisError.Location.File,
				Line = analysisError.Location.StartLine - 1, // Line appears to be 0-based in VS! :-(
				Column = analysisError.Location.StartColumn
			};
		}
	}
}
