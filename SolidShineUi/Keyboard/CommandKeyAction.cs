using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SolidShineUi.Keyboard
{
    public class CommandKeyAction : IKeyAction, ICommandSource
    {

#if NETCOREAPP
        object? cParam = null;
        IInputElement? cTarget = null;

        public UIElement? SourceElement { get; }

        public object CommandParameter { get => cParam!; }
        public IInputElement CommandTarget { get => cTarget!; }

        public CommandKeyAction(ICommand command, object? commandParameter, IInputElement? commandTarget, string methodId, UIElement? sourceItem = null)
        {
            c = command;
            cParam = commandParameter;
            cTarget = commandTarget;
            ID = methodId;
            SourceElement = sourceItem;
        }

#else
        object cParam = null;
        IInputElement cTarget = null;

        public UIElement SourceElement { get; }
        
        public object CommandParameter { get => cParam; }
        public IInputElement CommandTarget { get => cTarget; }

        public CommandKeyAction(ICommand command, object commandParameter, IInputElement commandTarget, string methodId, UIElement sourceItem = null)
        {
            c = command;
            cParam = commandParameter;
            cTarget = commandTarget;
            ID = methodId;
            SourceElement = sourceItem;
        }
#endif

        ICommand c;

        public string ID { get; set; }
        public ICommand Command { get => c; }

        public void Execute()
        {
            if (c.CanExecute(cParam))
            {
                if (c is RoutedCommand rc)
                {
                    rc.Execute(cParam, cTarget);
                }
                else
                {
                    c.Execute(cParam);
                }
            }
        }
    }
}
