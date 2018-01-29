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
        private const string GIT_HUB_FOLDER = "Generators";

        public GetMoreGeneratorsViewModel()
        {
            GetGeneratorList();
        }

        private GitHubClient Client { get; } = new GitHubClient(new ProductHeaderValue("TheRandomizer")); 
        private string GeneratorPath { get { return Environment.ExpandEnvironmentVariables(Utility.Settings.GeneratorDirectory); } }

        public Cursor Cursor { get { return GetProperty<Cursor>(); } set { SetProperty(value); } }
        public BaseGenerator Selected { get { return GetProperty<BaseGenerator>(); } set { SetProperty(value); } }
        public ObservableCollection<RepositoryContent> Generators { get { return GetProperty<ObservableCollection<RepositoryContent>>(); } set { SetProperty(value); } }
        public bool HasImports {
            get
            {
                return HasImportsInternal(Selected);
            }
        }
        public string Imports
        {
            get
            {
                if (!HasImports) return string.Empty;
                return string.Join(", ", ((TheRandomizer.Generators.Assignment.AssignmentGenerator)Selected).Imports.Select(i => i.Value).ToArray());
            }
        }

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
                    OnPropertyChanged("HasImports");
                    OnPropertyChanged("Imports");
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
                                foreach (var dependency in GetDependencies(Selected))
                                {
                                    var local = Path.Combine(GeneratorPath, dependency);
                                    //var found = Generators.FirstOrDefault(rc => rc.Name.Equals(dependency, StringComparison.InvariantCultureIgnoreCase));
                                    //if (found != null)
                                    //{
                                    //    var local = Path.Combine(GeneratorPath, found.Name);
                                    //    if (!File.Exists(local))
                                    //    {
                                    //        web.DownloadFile(found.DownloadUrl, local);
                                    //    }[]
                                    //}
                                    web.DownloadFile(GetDependencyDownloadUrl(SelectedRepository, dependency), local);
                                }
                            }
                            main?.LoadGenerators();
                            Generators.Remove(SelectedRepository);
                            SelectedRepository = null;
                            Cursor = null;
                        }
                    });
            }
        }

        public string GetDependencyDownloadUrl(RepositoryContent content, string path)
        {
            if (content == null) return string.Empty;
            var url = Path.GetDirectoryName(content.Path);
            url = Path.Combine(url, path);
            return url;
        }

        public List<string> GetDependencies(BaseGenerator generator)
        {
            var dependencies = new List<string>();
            if (HasImportsInternal(generator))
            {
                var assignment = (Generators.Assignment.AssignmentGenerator)generator;
                dependencies.AddRange(assignment.Imports.Select(i => i.Value));
            }
            if (generator.GeneratorType == GeneratorType.Lua)
            {
                var scriptPath = ((Generators.Lua.LuaGenerator)generator).ScriptPath;
                if (!string.IsNullOrWhiteSpace(scriptPath))
                {                    
                    dependencies.Add(scriptPath);
                }
            }
            return dependencies;
        }

        private bool HasImportsInternal(BaseGenerator generator)
        {
            if (generator == null) return false;
            if (generator.GeneratorType == GeneratorType.Assignment)
            {
                return ((Generators.Assignment.AssignmentGenerator)generator).Imports.Count > 0;
            }
            return false;
        }

        public void GetGeneratorList()
        {
            var dep = new DependencyObject();
            if (!DesignerProperties.GetIsInDesignMode(dep))
            {
                Cursor = Cursors.Wait;
                var generators = Client.Repository.Content.GetAllContents(GIT_HUB_USER, GIT_HUB_REPOSITORY, GIT_HUB_FOLDER);
                var result = new List<RepositoryContent>();
                if (generators.Wait(30000))
                {
                    foreach (var repository in generators.Result.Where(r => MatchExtensions(r.Path)))
                    {
                        var local = Path.Combine(GeneratorPath, repository.Name);
                        if (!File.Exists(local))
                        {
                            result.Add(repository);
                        }
                    }
                    Generators = new ObservableCollection<RepositoryContent>(result.OrderBy(r => r.Name));
                }
                Cursor = null;
            }
        }

        private bool MatchExtensions(string fileName)
        {
            return (fileName.EndsWith(".rnd.xml", StringComparison.InvariantCultureIgnoreCase) ||
                    fileName.EndsWith(".lib.xml", StringComparison.InvariantCultureIgnoreCase) ||
                    fileName.EndsWith(".rgen", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
