using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;

namespace SolidShineUi
{
    /// <summary>
    /// Contains helper functions for creating or cloning Avalonia Transition objects.
    /// </summary>
    public static class TransitionsHelper
    {

        /// <summary>
        /// Create a new <see cref="Transitions"/> list that contains deep clones of the transitions in an existing object.
        /// </summary>
        /// <param name="sourceObject">the source object to read from</param>
        public static Transitions? CloneTransitions(Animatable sourceObject)
        {
            if (sourceObject.Transitions == null) return null;
            else return CloneTransitions(sourceObject.Transitions);
        }

        /// <summary>
        /// Create a new <see cref="Transitions"/> list that contains deep clones of an existing Transitions list.
        /// </summary>
        /// <param name="source">the source list to read from</param>
        public static Transitions? CloneTransitions(Transitions source)
        {
            if (source.Count == 0) return null;
            List<ITransition> newItems = new List<ITransition>();
            foreach (ITransition item in source)
            {
                // it seems to me, looking at Avalonia's source code, that TransitionBase
                // is the only implementer of ITransition, which makes me wonder why they
                // have an interface that can only be internally implemented. I suppose
                // it's for future proofing, but does seem a bit unnecessary
                if (item is TransitionBase tb)
                {
                    // get property and get the new TransitionBase
                    if (tb.Property == null) continue;
                    Type t = tb.Property!.PropertyType;
                    TransitionBase? newTransition = GetTransitionForType(t);
                    if (newTransition == null) continue; // either it's a third-party transition, or my code isn't capturing it right
                    
                    // copy over the properties from the old one
                    newTransition.Property = tb.Property;
                    newTransition.Delay = tb.Delay;
                    newTransition.Duration = tb.Duration;
                    newTransition.Easing = tb.Easing; // TODO: properly deep clone the easing, I think there's more going on here
                    newItems.Add(newTransition);
                }
            }
            return [.. newItems];
        }

        /// <summary>
        /// Create a new Transition object based upon the type of the property.
        /// </summary>
        /// <param name="t">the type of the property to get a Transition object for</param>
        /// <remarks>
        /// This is based on the generic type listed in <see cref="Transition{T}"/> for
        /// each of the Transition classes in Avalonia (for example, <see cref="BoolTransition"/>
        /// is a <c>Transition&lt;bool&gt;</c>). 
        /// <para/>
        /// For <see cref="BrushTransition"/> and <see cref="TransformOperationsTransition"/>, 
        /// their type are <see cref="IBrush"/> and <see cref="ITransform"/> respectively, 
        /// so that is the type that is being expected here in <paramref name="t"/>. 
        /// If you want to get a transition for a brush or transform, pass in that interface
        /// type here instead.
        /// <para/>
        /// This only lists the transition objects included in Avalonia itself; custom transition
        /// objects made by other parties are not listed here.
        /// </remarks>
        public static TransitionBase? GetTransitionForType(Type t)
        {
            if (t == typeof(bool)) return new BoolTransition();
            if (t == typeof(BoxShadows)) return new BoxShadowsTransition();
            if (t == typeof(IBrush)) return new BrushTransition();
            if (t == typeof(Color)) return new ColorTransition();
            if (t == typeof(CornerRadius)) return new CornerRadiusTransition();
            if (t == typeof(double)) return new DoubleTransition();
            if (t == typeof(float)) return new FloatTransition();
            if (t == typeof(int)) return new IntegerTransition();
            if (t == typeof(Point)) return new PointTransition();
            if (t == typeof(RelativePoint)) return new RelativePointTransition();
            if (t == typeof(Size)) return new SizeTransition();
            if (t == typeof(Thickness)) return new ThicknessTransition();
            if (t == typeof(ITransform)) return new TransformOperationsTransition();
            if (t == typeof(Vector)) return new VectorTransition();
            else return null;
        }

    }
}
