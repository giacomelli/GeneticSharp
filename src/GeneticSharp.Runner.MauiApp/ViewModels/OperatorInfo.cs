using System.Reflection;

namespace GeneticSharp.Runner.MauiApp.ViewModels
{
    public class OperatorInfo
    {
        Type _objectType;
        string _implementatonName;

        public OperatorInfo(string objectInterfaceName, string implementationName)
        {
            InterfaceName = objectInterfaceName;
            ImplementationName = implementationName;
        }

        public string InterfaceName { get; }

        public string ImplementationName
        {
            get => _implementatonName;
            set
            {
                if (_implementatonName != value)
                {
                    _implementatonName = value;
                    _objectType = TypeHelper.GetTypeByName(InterfaceName, ImplementationName);

                    if (Instance == null || Instance.GetType() != _objectType)
                        Instance = Activator.CreateInstance(_objectType);
                }
            }
        }

        public object Instance { get; private set; }

        public PropertyInfo[] GetImplementationProperties()
        {
            return _objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(p => p.CanWrite).ToArray();
        }
    }
}
