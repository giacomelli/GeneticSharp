using GeneticSharp.Runner.MauiApp.ViewModels;
using System.Reflection;

namespace GeneticSharp.Runner.MauiApp;

public partial class PropertyEditorPage : ContentPage
{
	PropertyEditorViewModel _vm;    
    Dictionary<PropertyInfo, IView> _widgetMap;


    public PropertyEditorPage(PropertyEditorViewModel vm)
	{
		InitializeComponent();
        _widgetMap = new Dictionary<PropertyInfo, IView>();

        _vm = vm;
		_vm.PropertyChanged += (s, e) => 
		{
            if (_vm.IsReady)
                BindToView();
        };        

        BindingContext = vm;        
    }

    protected override bool OnBackButtonPressed()
    {
        BindToObject();
        return base.OnBackButtonPressed();
    }

    void BindToView()
    {
        var vbox = propertiesView;

        foreach (var p in _vm.OperatorInfo.GetImplementationProperties())
        {
            var hbox = new HorizontalStackLayout {  VerticalOptions = LayoutOptions.Center };
            var value = p.GetValue(_vm.OperatorInfo.Instance, null);

            var label = new Label { Text = $"{p.Name}: {value:000}" };
            hbox.Add(label);            
            IView input = null;

            if (p.PropertyType == typeof(int))
            {
                var stepper = new Stepper(0, 100, 1, 1) {  WidthRequest = 100 };
                stepper.ValueChanged += (sender, e) => label.Text = $"{p.Name}: {e.NewValue:000}";
                stepper.Value = Convert.ToDouble(value);

                input = stepper;
            }

            if (p.PropertyType == typeof(bool))
            {
                var toggle = new CheckBox();
                toggle.IsChecked = Convert.ToBoolean(value);
                input = toggle;
            }
            else if (p.PropertyType == typeof(float) || p.PropertyType == typeof(double))
            {
                var horizontalScale = new Slider(0, 1, 0.05);

                if (_vm.OperatorInfo.Instance != null)
                    horizontalScale.Value = Convert.ToDouble(value);

                input = horizontalScale;
            }
            else if (p.PropertyType == typeof(TimeSpan))
            {
                var secondsHBox = new HorizontalStackLayout();
                var seconds = new Slider(0, int.MaxValue, 1);
                seconds.Value = ((TimeSpan)value).TotalSeconds;
                secondsHBox.Add(seconds);

                var secondsLabel = new Label { Text = "seconds" };
                secondsHBox.Add(secondsLabel);

                input = secondsHBox;
            }

            if (input != null)
            {
                _widgetMap.Add(p, input);
                hbox.Add(input);
            }

            vbox.Add(hbox);
        }
    }

    void BindToObject()
    {
        foreach (var m in _widgetMap)
        {
            if (m.Key.PropertyType == typeof(int))
            {
                m.Key.SetValue(_vm.OperatorInfo.Instance, Convert.ToInt32(((Stepper)m.Value).Value), new object[0]);
            }
            else if (m.Key.PropertyType == typeof(bool))
            {
                m.Key.SetValue(_vm.OperatorInfo.Instance, ((CheckBox)m.Value).IsChecked, new object[0]);
            }
            else if (m.Key.PropertyType == typeof(float))
            {
                m.Key.SetValue(_vm.OperatorInfo.Instance, Convert.ToSingle(((Slider)m.Value).Value), new object[0]);
            }
            else if (m.Key.PropertyType == typeof(double))
            {
                m.Key.SetValue(_vm.OperatorInfo.Instance, ((Slider)m.Value).Value, new object[0]);
            }
            else if (m.Key.PropertyType == typeof(TimeSpan))
            {
                var hbox = (HorizontalStackLayout)m.Value;
                var seconds = ((Slider)hbox.Children[0]).Value;
                m.Key.SetValue(_vm.OperatorInfo.Instance, TimeSpan.FromSeconds(seconds), new object[0]);
            }
        }
    }
}