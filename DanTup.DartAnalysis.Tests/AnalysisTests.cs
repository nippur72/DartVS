﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DanTup.DartAnalysis.Tests
{
	public class AnalysisTests : Tests
	{
		[Fact]
		public async Task SetAnalysisRoots()
		{
			using (var service = new DartAnalysisService(SdkFolder, ServerScript))
			{
				bool? isAnalyzing = null; // Keep track of when we're called to say server status has changed
				List<AnalysisError> errors = new List<AnalysisError>(); // Keep track of errors that are reported
				using (service.ServerStatusNotification.Subscribe(e => { isAnalyzing = e.IsAnalysing; }))
				using (service.AnalysisErrorsNotification.Subscribe(e => errors.AddRange(e.Errors)))
				{
					// Send a request to do some analysis.
					await service.SetAnalysisRoots(new[] { SampleDartProject });

					// Wait for a server status message (which should be that the analysis complete)
					await service.ServerStatusNotification.FirstAsync();


					// Ensure the event fired to say analysing had finished.
					Assert.Equal(false, isAnalyzing);

					// Ensure the error-free file got no errors.
					Assert.Equal(0, errors.Where(e => e.File.EndsWith("\\hello_world.dart")).Count());

					// Ensure the single-error file got the expected error.
					Assert.Equal(1, errors.Where(e => e.File.EndsWith("\\single_type_error.dart")).Distinct().Count());
					var error = errors.First(e => e.File.EndsWith("\\single_type_error.dart"));
					Assert.Equal("StaticWarningCode.ARGUMENT_TYPE_NOT_ASSIGNABLE", error.ErrorCode);
				}
			}
		}

		[Fact]
		public async Task TestAnalysisUpdateContent()
		{
			using (var service = new DartAnalysisService(SdkFolder, ServerScript))
			{
				List<AnalysisError> errors = new List<AnalysisError>(); // Keep track of errors that are reported
				using (service.AnalysisErrorsNotification.Subscribe(e => errors.AddRange(e.Errors)))
				{
					// Set the roots to our known project.
					await service.SetAnalysisRoots(new[] { SampleDartProject });

					// Wait for a server status message (which should be that the analysis complete)
					await service.ServerStatusNotification.FirstAsync();
				}

				// Ensure we got the expected error in single_type_error.
				Assert.Equal(1, errors.Where(e => e.File.EndsWith("\\single_type_error.dart")).Distinct().Count());

				// Clear the error list ready for next time.
				errors.Clear();

				using (service.AnalysisErrorsNotification.Subscribe(e => errors.AddRange(e.Errors)))
				{

					// Build a "fix" for this, which is to change the 1 to a string '1'.
					await service.UpdateContent(
						Path.Combine(SampleDartProject, "single_type_error.dart"),
						@"
						void main() {
							my_function('1');
						}

						void my_function(String a) {
						}
					"
					);

					// Wait for a server status message (which should be that the analysis complete)
					await service.ServerStatusNotification.FirstAsync();
				}

				// Ensure the error has gone away.
				Assert.Equal(0, errors.Where(e => e.File.EndsWith("\\single_type_error.dart")).Distinct().Count());
			}
		}

		[Fact]
		public async Task AnalysisSetSubscriptions()
		{
			using (var service = new DartAnalysisService(SdkFolder, ServerScript))
			{
				// Set the roots to our known project.
				await service.SetAnalysisRoots(new[] { SampleDartProject });

				// Request all the other stuff
				await service.SetAnalysisSubscriptions(new[] { "HIGHLIGHTS", "NAVIGATION", "OUTLINE" }, Path.Combine(SampleDartProject, "hello_world.dart"));

				// Wait for a server status message (which should be that the analysis complete)
				await service.ServerStatusNotification.FirstAsync();

				// TODO: Ensure we got some expected stuffs
			}
		}
	}
}
