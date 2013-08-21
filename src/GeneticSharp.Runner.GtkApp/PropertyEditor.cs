using System;
using Gtk;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace GeneticSharp.Runner.GtkApp
{
	public partial class PropertyEditor : Gtk.Dialog
	{
		#region Fields
		private Type m_objectType;
		private Dictionary<PropertyInfo, Widget> m_widgetMap;
		#endregion

		#region Constructors
		public PropertyEditor (Type objectType, object objectInstance)
		{
			this.Build ();
			buttonOk.Clicked += delegate {
				BindToObject();
				Destroy();
			};

			buttonCancel.Clicked += delegate {
				Destroy();
			};

			m_objectType = objectType;

			if (objectInstance == null || objectInstance.GetType() != objectType) {
				ObjectInstance = Activator.CreateInstance (objectType);	
			} else {
				ObjectInstance = objectInstance;
			}

			m_widgetMap = new Dictionary<PropertyInfo, Widget> ();
			BindToWidget ();
			VBox.ResizeMode = ResizeMode.Immediate;
			VBox.CheckResize ();

			ShowAll ();
		}
		#endregion

		#region Properties
		public object ObjectInstance { get; private set; }
		#endregion

		#region Methods
		private static PropertyInfo[] GetObjectProperties(Type objectType)
		{
			return objectType.GetProperties (BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(p => p.CanWrite).ToArray();
		}

		public static bool HasEditableProperties(Type objectType)
		{
			return GetObjectProperties (objectType).Length > 0;
		}

		private void BindToWidget()
		{
			foreach (var p in GetObjectProperties(m_objectType)) {
				var hbox = new HBox ();

				var label = new Label (p.Name);
				hbox.Add (label);
				var value = p.GetValue (ObjectInstance, null);
				Widget input = null;

				if (p.PropertyType == typeof(int)) {
					var spinButton = new SpinButton (0, int.MaxValue, 1);
					spinButton.Value = Convert.ToDouble (value);
					input = spinButton;
				}
				if (p.PropertyType == typeof(bool)) {
					var toggle = new ToggleButton();
					toggle.Active = Convert.ToBoolean (value);
					input = toggle;
				}
				else if (p.PropertyType == typeof(float) || p.PropertyType == typeof(double)) {
					var hScale = new HScale (0, 1, 0.05);

					if (ObjectInstance != null) {
						hScale.Value = Convert.ToDouble (value);
					}

					input = hScale;
				}
				else if (p.PropertyType == typeof(TimeSpan)) {
					var secondsHBox = new HBox();
					var seconds = new SpinButton (0, int.MaxValue, 1);
					seconds.Value = ((TimeSpan)value).TotalSeconds;
					secondsHBox.Add (seconds);

					var secondsLabel = new Label("seconds");
					secondsHBox.Add (secondsLabel);

					input = secondsHBox;
				}

				if (input != null) {
					m_widgetMap.Add (p, input);
					hbox.Add (input);
				}
							
				VBox.Add (hbox);
			}
		}

		private void BindToObject()
		{
			foreach (var m in m_widgetMap) {
				if (m.Key.PropertyType == typeof(int)) {
					m.Key.SetValue (ObjectInstance, ((SpinButton)m.Value).ValueAsInt, new object[0]);
				} else if (m.Key.PropertyType == typeof(bool)) {
					m.Key.SetValue (ObjectInstance, ((ToggleButton)m.Value).Active, new object[0]);
				}else if (m.Key.PropertyType == typeof(float)) {
					m.Key.SetValue (ObjectInstance, Convert.ToSingle (((HScale)m.Value).Value), new object[0]);
				} else if (m.Key.PropertyType == typeof(double)) {
					m.Key.SetValue (ObjectInstance, ((HScale)m.Value).Value, new object[0]);
				}else if (m.Key.PropertyType == typeof(TimeSpan)) {
					var hbox = (HBox)m.Value;
					var seconds = ((SpinButton)hbox.Children [0]).Value;
					m.Key.SetValue (ObjectInstance, TimeSpan.FromSeconds(seconds), new object[0]);
				}
			}
		}
		#endregion
	}
}

