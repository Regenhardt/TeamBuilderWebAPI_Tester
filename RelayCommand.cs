using System;
using System.Windows.Input;
using System.Diagnostics;

namespace MVVM
{
	public class RelayCommand : ICommand
    {
        #region [ Fields ]

	    private readonly Action<object> execute;
	    private readonly Predicate<object> canExecute;

        #endregion // Fields

        #region [ Constructors]
        /// <inheritdoc />
        /// <summary>
        /// Create new relay command, based on injection of action object.
        /// </summary>
        /// <param name="execute">Action object</param>
        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }

        /// <summary>
        /// Create new relay command, based on injection of action (command contents) and function predicate for the can execute methode.
        /// </summary>
        /// <param name="execute">Action object.</param>
        /// <param name="canExecute">Function predicate for the can execute.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
	        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        #endregion // Constructors

        #region [ ICommand Members ]
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return canExecute?.Invoke(parameter) ?? true;
        }

        /// <inheritdoc />
        /// <summary>
        /// Evemt handler for the can execute changed event.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
	        remove => CommandManager.RequerySuggested -= value;
        }

        /// <inheritdoc />
        /// <summary>
        /// Command execution with parameter.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            execute(parameter);
        }
        #endregion // ICommand Members
    }
}
