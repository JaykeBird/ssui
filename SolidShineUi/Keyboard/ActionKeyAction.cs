using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SolidShineUi.Keyboard
{
    public class ActionKeyAction : IKeyAction
    {
        Action a;

#if NETCOREAPP
        public UIElement? SourceElement { get; }

        public ActionKeyAction(Action action, string methodId, UIElement? sourceElement = null)
        {
            a = action;
            ID = methodId;
            SourceElement = sourceElement;
        }

#else
        public UIElement SourceElement { get; }

        public ActionKeyAction(Action action, string methodId, UIElement sourceElement = null)
        {
            a = action;
            ID = methodId;
            SourceElement = sourceElement;
        }
#endif
        public string ID { get; set; }

        public void Execute()
        {
            a.Invoke();
        }
    }
}
