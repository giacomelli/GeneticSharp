using CommunityToolkit.Mvvm.ComponentModel;

namespace GeneticSharp.Runner.MauiApp.ViewModels
{
    [QueryProperty("OperatorInfo", "OperatorInfo")]    
    public partial class PropertyEditorViewModel : ObservableObject
    {
        OperatorInfo _operatorInfo;

        public PropertyEditorViewModel()
        {               
        }

        [ObservableProperty]
        string objectInterfaceName;

        [ObservableProperty]
        string objectTypeName;

        public OperatorInfo OperatorInfo
        {
            get => _operatorInfo;
            set
            {
                if(value != _operatorInfo)
                {
                    _operatorInfo = value;
                    ObjectInterfaceName = value.InterfaceName;
                    ObjectTypeName = value.ImplementationName;                   
                }
            }
        }

        public string Title => $"{ObjectInterfaceName}: {objectTypeName}";
        public bool IsReady => objectInterfaceName != null && objectTypeName != null;                  
    }   
}
