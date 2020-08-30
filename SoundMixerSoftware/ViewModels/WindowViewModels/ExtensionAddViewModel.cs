using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
using SoundMixerSoftware.Helpers.Utils;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    public class ExtensionAddViewModel : Screen
    {

        #region Public Properties

        public static ExtensionAddViewModel Instance => IoC.Get<ExtensionAddViewModel>();

        public BindableCollection<SectionModel> Sections { get; set; } = new BindableCollection<SectionModel>();
        public int SliderIndex { get; set; }

        public ISessionModel SelectedSession => Sections.FirstOrDefault(x => x.SelectedSession != null)?.SelectedSession;

        #endregion

        #region Constructor

        public ExtensionAddViewModel()
        {
            var defaultSection = new SectionModel
            {
                Id = "default",
                Name = "Default Extension"
            };
            defaultSection.Sessions.Add(new ForegroundSessionModel());
            AddSection(defaultSection);
        }

        #endregion
        
        #region Public Methods

        public bool AddSection(SectionModel section)
        {
            var sectionId = section.Id;
            if (Sections.Any(x => x.Id == sectionId))
                return false;
            section.PropertyChanged += SectionOnPropertyChanged;
            Sections.Add(section);
            return true;
        }

        public SectionModel GetSection(string sectionId)
        {
            return Sections.FirstOrDefault(x => x.Id == sectionId);
        }

        public bool AddSession(string sectionId, ISessionModel session)
        {
            var section = GetSection(sectionId);
            if (section == default)
                return false;
            section.Sessions.Add(session);
            return true;
        }

        #endregion
        
        #region Private Events
        
        private void SectionOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propertyName = e.PropertyName;
            if (propertyName != nameof(SectionModel.SelectedSession))
                return;
            var selectedSession = (sender as SectionModel)?.SelectedSession;
            if (selectedSession == null)
                return;
            foreach (var section in Sections)
            {
                if(section.SelectedSession == selectedSession)
                    continue;
                section.SelectedSession = null;
            }
        }

        public void AddClick()
        {
            var selectedSession = SelectedSession;
            if (selectedSession == null)
                return;
            SessionUtils.AddSession(SliderIndex, selectedSession.CreateSession(SliderIndex));
            TryCloseAsync();
        }
        
        #endregion
    }
}