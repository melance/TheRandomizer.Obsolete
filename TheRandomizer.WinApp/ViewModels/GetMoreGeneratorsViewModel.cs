using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheRandomizer.Utility;
using Octokit;
using System.Windows.Input;
using TheRandomizer.WinApp.Commands;
using System.IO;
using System.Collections.ObjectModel;
using System.Net;
using TheRandomizer.Generators;
using System.Windows;
using System.ComponentModel;

namespace TheRandomizer.WinApp.ViewModels
{
    class GetMoreGeneratorsViewModel : ObservableBase
    {
        private const string GIT_HUB_USER = "melance";
        private const string GIT_HUB_REPOSITORY = "TheRandomizerCustomizations";
        private const string GIT_HUG_FOLDER = "Grammars";

        public GetMoreGeneratorsViewModel()
        {
            GetGeneratorList();
        }

        private GitHubClient Client { get; } = new GitHubClient(new ProductHeaderValue("TheRandomizer")); 
        private string GeneratorPath { get { return Environment.ExpandEnvironmentVariables(Properties.Settings.Default.GeneratorDirectory); } }

        public Cursor Cursor { get { return GetProperty<Cursor>(); } set { SetProperty(value); } }
        public BaseGenerator Selected { get { return GetProperty<BaseGenerator>(); } set { SetProperty(value); } }
        public ObservableCollection<RepositoryContent> Generators { get { return GetProperty<ObservableCollection<RepositoryContent>>(); } set { SetProperty(value); } }

        public RepositoryContent SelectedRepository
        {
            get
            {
                return GetProperty<RepositoryContent>();
            }
            set
            {
                SetProperty(value);
                if (SelectedRepository != null)
                {
                    GetDetails(SelectedRepository);
                }
            }
        }

        public void GetDetails(RepositoryContent repository)
        {
            Cursor = Cursors.Wait;
            var content = Client.Repository.Content.GetAllContents(GIT_HUB_USER, GIT_HUB_REPOSITORY, repository.Path);
            content.Wait();
            Selected = BaseGenerator.Deserialize(content.Result.FirstOrDefault().Content);
            Cursor = null;
        }
        
        public DelegateCommand Download
        {
            get
            {
                return new DelegateCommand(
                    () =>
                    {
                        if (SelectedRepository != null)
                        {
                            Cursor = Cursors.Wait;
                            var path = Path.Combine(GeneratorPath, SelectedRepository.Name);
                            var main = System.Windows.Application.Current.MainWindow.DataContext as ViewModels.MainWindowViewModel;
                            using (var web = new WebClient())
                            {
                                web.DownloadFile(SelectedRepository.DownloadUrl, path);
                            }
                            main?.LoadGenerators();
                            Generators.Remove(SelectedRepository);
                            SelectedRepository = null;
                            Cursor = null;
                        }
                    });
            }
        }

        public void GetGeneratorList()
        {
            var dep = new DependencyObject();
            if (!DesignerProperties.GetIsInDesignMode(dep))
            {
                Cursor = Cursors.Wait;
                var generators = Client.Repository.Content.GetAllContents(GIT_HUB_USER, GIT_HUB_REPOSITORY, GIT_HUG_FOLDER);
                var result = new List<RepositoryContent>();
                if (generators.Wait(30000))
                {
                    foreach (var repository in generators.Result)
                    {
                        var local = Path.Combine(GeneratorPath, repository.Name);
                        if (!File.Exists(local))
                        {
                            result.Add(repository);
                        }
                    }
                    result.RemoveAll(r => !MatchExtensions(r.Name));
                    Generators = new ObservableCollection<RepositoryContent>(result.OrderBy(r => r.Name));
                }
                Cursor = null;
            }
        }

        private bool MatchExtensions(string fileName)
        {
            return (fileName.EndsWith(".rnd.xml", StringComparison.InvariantCultureIgnoreCase) || fileName.EndsWith(".rgen", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
