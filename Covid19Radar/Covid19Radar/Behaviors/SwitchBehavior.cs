using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Covid19Radar.Behaviors
{
    public class SwitchBehavior : Behavior<Switch>
    {
        /// <summary>
        /// The command property.
        /// </summary>
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(SwitchBehavior), null);

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The command.</value>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Gets the bindable.
        /// </summary>
        /// <value>The bindable.</value>
        public Switch Bindable { get; private set; }

        /// <summary>
        /// On attached to.
        /// </summary>
        /// <param name="bindable">The bindable component.</param>
        protected override void OnAttachedTo(Switch bindable)
        {
            base.OnAttachedTo(bindable);
            Bindable = bindable;
            Bindable.BindingContextChanged += OnBindingContextChanged;
            Bindable.Toggled += OnSwitchToggled;
        }

        /// <summary>
        /// Ons the binding context changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            OnBindingContextChanged();
            BindingContext = Bindable.BindingContext;
        }

        /// <summary>
        /// Ons the switch toggled.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnSwitchToggled(object sender, ToggledEventArgs e)
        {
            Command?.Execute(e.Value);
        }

        /// <summary>
        /// On detaching from.
        /// </summary>
        /// <param name="bindable">The bindable component.</param>
        protected override void OnDetachingFrom(Switch bindable)
        {
            base.OnDetachingFrom(bindable);
            Bindable.BindingContextChanged -= OnBindingContextChanged;
            Bindable.Toggled -= OnSwitchToggled;
            Bindable = null;
        }
    }
}
