
namespace VoiceRecorder
{
    using Caliburn.Micro;
    using Caliburn.Micro.BindableAppBar;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Data;
    using Model;
    using ViewModels;
    using Windows.Phone.Speech.VoiceCommands;
    
    public class Bootstrapper : PhoneBootstrapper
    {
        #region Fields

        PhoneContainer _container;

        #endregion

        protected override void Configure()
        {
            AddCustomConventions();
            _container = new PhoneContainer(RootFrame);
            _container.RegisterPhoneServices();
            PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
            _container.RegisterSingleton(typeof(AudioCaptureDevice), String.Empty, typeof(AudioCaptureDevice));
            _container.RegisterSingleton(typeof(IRecordingManager), String.Empty, typeof(RecordingManager));
            _container.RegisterSingleton(typeof(ITagManager), String.Empty, typeof(TagManager));
            _container.RegisterSingleton(typeof(IAudioRecorder), String.Empty, typeof(AudioRecorder));
            _container.RegisterInstance(typeof(RecordingsContext), String.Empty, new RecordingsContext("Data Source=isostore:/Items.sdf"));
            _container.PerRequest<IRecordingStreamManager, RecordingStreamManager>();
            RegisterActions();
            RegisterViewModels();
        }

        private void RegisterActions()
        {
            _container
                .PerRequest<IRecordingAction, PlayAction>()
                .PerRequest<IRecordingAction, EditAction>()
                .PerRequest<IRecordingAction, DeleteAction>()
                .PerRequest<IRecordingAction, CopyToMediaLibraryAction>();
        }

        private void RegisterViewModels()
        {
            _container
                .PerRequest<MainPageViewModel>()
                .Singleton<FilterSettingsViewModel>()
                .PerRequest<RecordingsViewModel>()
                .PerRequest<RecordingViewModel>()
                .PerRequest<RecordingViewModelFactory>()
                .PerRequest<TileViewModel>()
                .PerRequest<TagsViewModel>()
                .PerRequest<SettingsViewModel>()
                .PerRequest<EditRecordingViewModel>()
                .PerRequest<RecordingActionsViewModel>()
                .PerRequest<SelectTagsViewModel>()
                .PerRequest<NewRecordingViewModel>()
                .PerRequest<NewTagViewModel>()
                .PerRequest<AboutViewModel>()
                .PerRequest<DeleteTagsViewModel>()
                .PerRequest<ExportRecordingViewModel>();
        }

        private void AddCustomConventions()
        {
            // App Bar Conventions
            ConventionManager.AddElementConvention<BindableAppBarButton>(
                Control.IsEnabledProperty, "DataContext", "Click");
            ConventionManager.AddElementConvention<BindableAppBarMenuItem>(
                Control.IsEnabledProperty, "DataContext", "Click");

            ConventionManager.AddElementConvention<Pivot>(Pivot.ItemsSourceProperty, "SelectedItem", "SelectionChanged").ApplyBinding =
                (viewModelType, path, property, element, convention) =>
                {
                    if (ConventionManager
                        .GetElementConvention(typeof(ItemsControl))
                        .ApplyBinding(viewModelType, path, property, element, convention))
                    {
                        ConventionManager
                            .ConfigureSelectedItem(element, Pivot.SelectedItemProperty, viewModelType, path);
                        ConventionManager
                            .ApplyHeaderTemplate(element, Pivot.HeaderTemplateProperty, null, viewModelType);
                        return true;
                    }

                    return false;
                };

            ConventionManager.AddElementConvention<Panorama>(Panorama.ItemsSourceProperty, "SelectedItem", "SelectionChanged").ApplyBinding =
                (viewModelType, path, property, element, convention) =>
                {
                    if (ConventionManager
                        .GetElementConvention(typeof(ItemsControl))
                        .ApplyBinding(viewModelType, path, property, element, convention))
                    {
                        ConventionManager
                            .ConfigureSelectedItem(element, Panorama.SelectedItemProperty, viewModelType, path);
                        ConventionManager
                            .ApplyHeaderTemplate(element, Panorama.HeaderTemplateProperty, null, viewModelType);
                        return true;
                    }

                    return false;
                };

        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void OnUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            //if (Debugger.IsAttached)
            //{
            //    Debugger.Break();
            //}

            e.Handled = true;
        }

        protected async override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            await VoiceCommandService.InstallCommandSetsFromFileAsync(new Uri("ms-appx:///VoiceCommands.xml"));

#if DEBUG
            LogManager.GetLog = type => new DebugLogger();
#endif
        }

        protected override async void OnClose(object sender, ClosingEventArgs e)
        {
            await StopRecordingIfActive();
            base.OnClose(sender, e);
        }

        protected override async void OnDeactivate(object sender, DeactivatedEventArgs e)
        {
            await StopRecordingIfActive();
            base.OnDeactivate(sender, e);
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[] { GetType().Assembly };
        }

        private async Task StopRecordingIfActive()
        {
            var recorder = _container.GetInstance(typeof(IAudioRecorder), String.Empty) as IAudioRecorder;
            if(recorder != null)
                await recorder.StopRecording();
        }
    }
}
