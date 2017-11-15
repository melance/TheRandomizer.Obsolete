using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Windows.Input;
using TheRandomizer.Generators;
using TheRandomizer.Generators.Parameter;
using TheRandomizer.Utility;
using TheRandomizer.WinApp.Commands;

namespace TheRandomizer.WinApp.Utility
{
    public class GeneratorWrapper : ObservableBase
    {
        
        #region Members
        private BaseGenerator _generator;
        #endregion

        #region Constructors
        public GeneratorWrapper(BaseGenerator generator)
        {
            _generator = generator;
        }
        #endregion

        #region Properties
        public int MaxLength { get; set; } = 10;
        public int Repeat { get; set; } = 1;

        public Guid Id
        {
            get
            {
                return _generator != null ? _generator.Id : Guid.Empty;
            }
        }

        public string Name {
            get
            {
                return _generator?.Name;
            }
        }

        public string Description
        {
            get
            {
                return _generator?.Description;
            }
        }

        public string Author
        {
            get
            {
                return _generator?.Author;
            }
        }

        public string GeneratorType
        {
            get
            {
                return _generator?.GeneratorType;
            }
        }

        public OutputFormat OutputFormat
        {
            get
            {
                return _generator != null ? _generator.OutputFormat : OutputFormat.Text;
            }
        }

        public string URL
        {
            get
            {
                return _generator?.Url;
            }
        }

        public string TagList
        {
            get
            {
                return _generator?.TagList;
            }
        }

        public bool SupportsMaxLength
        {
            get
            {
                return _generator != null ? _generator.SupportsMaxLength : false;
            }
        }

        public bool HasParameters
        {
            get
            {
                return _generator != null ? _generator.Parameters.Count > 0 : false;
            }
        }

        public ConfigurationList Parameters
        {
            get
            {
                return _generator?.Parameters;
            }
        }

        public string Results
        {
            get
            {
                return GetProperty<string>();
            }
            set
            {
                SetProperty(value);
            }
        }

        public ICommand GenerateContent
        {
            get
            {
                return new DelegateCommand(Generate);
            }
        }
        #endregion

        #region Methods
        public void Cancel()
        {
            _generator?.Cancel();
        }

        public void Generate()
        {
            Results = FormatResults(_generator?.Generate(Repeat, MaxLength));
            OnPropertyChanged("Results");
        }
        
        private string FormatResults(IEnumerable<string> results)
        {            
            using (StringWriter sWriter = new StringWriter())
            {
                using (HtmlTextWriter writer = new HtmlTextWriter(sWriter))
                {
                    var odd = true;
                    writer.WriteFullBeginTag("head");
                    writer.WriteFullBeginTag("style");
                    writer.WriteLine("body { font-family: Consolas, Courier New, Monospace; }");
                    writer.WriteLine("div.even { background-color: #F8F8F8; }");
                    writer.WriteLine("div.error { color: red; font-weight: bold; }");
                    if (!string.IsNullOrWhiteSpace(_generator.CSS))
                    {
                        writer.WriteLine(_generator.CSS);
                    }
                    writer.WriteEndTag("style");
                    writer.WriteEndTag("head");
                    writer.WriteFullBeginTag("body");
                    if (results != null && results.ToList().Count > 0)
                    {
                        foreach (var item in results)
                        {
                            writer.WriteBeginTag("div");
                            if (odd)
                                writer.WriteAttribute("class", "odd");
                            else
                                writer.WriteAttribute("class", "even");
                            odd = !odd;
                            writer.Write(HtmlTextWriter.TagRightChar);
                            writer.Write(item);
                            writer.WriteEndTag("div");
                        }
                    }
                    else
                    {
                        writer.WriteBeginTag("div");
                        writer.WriteAttribute("class", "error");
                        writer.Write(HtmlTextWriter.TagRightChar);
                        writer.Write("No Results");
                        writer.WriteEndTag("span");
                    }
                    writer.WriteEndTag("body");
                }
                return sWriter.ToString();
            }
        }
        
        #endregion
    }
}
