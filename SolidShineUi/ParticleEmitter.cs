using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace SolidShineUi
{
    /// <summary>
    /// A control that emits multiple particles while active, with various properties to control the appearance and behavior of these particles.
    /// </summary>
    public class ParticleEmitter : Control, IDisposable
    {

#if NETCOREAPP
        Grid? g;
#else
        Grid g;
#endif

        Thread rthr;

        /// <summary>
        /// Create a ParticleEmitter.
        /// </summary>
        public ParticleEmitter()
        {
            // don't need to set a template here since I've set it in Generic.xaml

            //string template =
            //"<ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
            //"<Grid Name=\"grid\" Background=\"{TemplateBinding Background}\" />" +
            //"</ControlTemplate>";
            //Template = (ControlTemplate)XamlReader.Parse(template);

            rthr = new Thread(() =>
            {
                while (active)
                {
                    threadRun(this);
                }
            });

            IsEnabledChanged += ParticleEmitter_IsEnabledChanged;

            //IsHitTestVisible = false;
            //Focusable = false;
        }

        private void ParticleEmitter_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            enabled = IsEnabled;
        }

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("grid") is Grid gg && gg != null)
            {
                g = gg;
            }
            else return;
        }

        #region Properties

        /// <summary>
        /// Get or set the brush that should be painted onto the particles when created.
        /// </summary>
        /// <remarks>
        /// Once the particle emitter starts, this brush is frozen and cannot be edited.
        /// </remarks>
        public Brush ParticleBrush { get => (Brush)GetValue(ParticleBrushProperty); set => SetValue(ParticleBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="ParticleBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ParticleBrushProperty
            = DependencyProperty.Register(nameof(ParticleBrush), typeof(Brush), typeof(ParticleEmitter),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Red)));

        /// <summary>
        /// Get or set the maximum number of particles that can be alive at a time. If this number is reached, new particles
        /// will not be created until some existing ones are culled.
        /// </summary>
        /// <remarks>
        /// The default value is 100. This can be used to help with performance, with smaller numbers being more performant.
        /// If set to a value less than 0, then no new particles will be created.
        /// </remarks>
        public int MaxCount { get => (int)GetValue(MaxCountProperty); set => SetValue(MaxCountProperty, value); }

        /// <summary>The backing dependency property for <see cref="MaxCount"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MaxCountProperty
            = DependencyProperty.Register(nameof(MaxCount), typeof(int), typeof(ParticleEmitter),
            new FrameworkPropertyMetadata(100));

        /// <summary>
        /// Get or set the minimum number of update ticks that created particles should be alive for.
        /// </summary>
        /// <remarks>
        /// Each created particle will have a randomly selected lifetime value, somewhere between <see cref="MinLife"/> and <see cref="MaxLife"/>.
        /// Once that particle is created, that value will tick down on every update tick; once that value reaches 0, that particle is culled and removed.
        /// </remarks>
        public int MinLife { get => (int)GetValue(MinLifeProperty); set => SetValue(MinLifeProperty, value); }

        /// <summary>The backing dependency property for <see cref="MinLife"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MinLifeProperty
            = DependencyProperty.Register(nameof(MinLife), typeof(int), typeof(ParticleEmitter),
            new FrameworkPropertyMetadata(5));

        /// <summary>
        /// Get or set the maximum number of update ticks that created particles should be alive for.
        /// </summary>
        /// <remarks>
        /// Each created particle will have a randomly selected lifetime value, somewhere between <see cref="MinLife"/> and <see cref="MaxLife"/>.
        /// Once that particle is created, that value will tick down on every update tick; once that value reaches 0, that particle is culled and removed.
        /// </remarks>
        public int MaxLife { get => (int)GetValue(MaxLifeProperty); set => SetValue(MaxLifeProperty, value); }

        /// <summary>The backing dependency property for <see cref="MaxLife"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MaxLifeProperty
            = DependencyProperty.Register(nameof(MaxLife), typeof(int), typeof(ParticleEmitter),
            new FrameworkPropertyMetadata(20));

        /// <summary>
        /// Get or set the direction that created particles should go in. If the Vector is <c>0, 0</c>, particles will go in all directions.
        /// </summary>
        /// <remarks>
        /// If the Vector is <c>0, 0</c>, then each created particle will select and go in a random direction. If the Vector is any other value,
        /// then all particles will go in that specific direction. For best performance, use a normalized vector.
        /// </remarks>
        public Vector ParticleDirection { get => (Vector)GetValue(ParticleDirectionProperty); set => SetValue(ParticleDirectionProperty, value); }

        /// <summary>The backing dependency property for <see cref="ParticleDirection"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ParticleDirectionProperty
            = DependencyProperty.Register(nameof(ParticleDirection), typeof(Vector), typeof(ParticleEmitter),
            new FrameworkPropertyMetadata(new Vector(0, 0)));

        /// <summary>
        /// Get or set the minimum speed that each created particle will travel in each update tick.
        /// </summary>
        /// <remarks>
        /// Each created particle will have a randomly selected speed, somewhere between <see cref="MinSpeed"/> and <see cref="MaxSpeed"/>.
        /// Each update tick, that particle will move in its chosen direction at a distance equal to that speed.
        /// </remarks>
        public double MinSpeed { get => (double)GetValue(MinSpeedProperty); set => SetValue(MinSpeedProperty, value); }

        /// <summary>The backing dependency property for <see cref="MinSpeed"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MinSpeedProperty
            = DependencyProperty.Register(nameof(MinSpeed), typeof(double), typeof(ParticleEmitter),
            new FrameworkPropertyMetadata(1.0));

        /// <summary>
        /// Get or set the maximum speed that each created particle will travel in each update tick.
        /// </summary>
        /// <remarks>
        /// Each created particle will have a randomly selected speed, somewhere between <see cref="MinSpeed"/> and <see cref="MaxSpeed"/>.
        /// Each update tick, that particle will move in its chosen direction at a distance equal to that speed.
        /// </remarks>
        public double MaxSpeed { get => (double)GetValue(MaxSpeedProperty); set => SetValue(MaxSpeedProperty, value); }

        /// <summary>The backing dependency property for <see cref="MaxSpeed"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MaxSpeedProperty
            = DependencyProperty.Register(nameof(MaxSpeed), typeof(double), typeof(ParticleEmitter),
            new FrameworkPropertyMetadata(5.0));

        /// <summary>
        /// Get or set the minimum size that each created particle should be.
        /// </summary>
        /// <remarks>
        /// Make sure that the width and height values in <see cref="MinSize"/> are smaller than their respective values in <see cref="MaxSize"/>.
        /// <para/>
        /// Each created particle will have a randomly selected size, somewhere between <see cref="MinSize"/> and <see cref="MaxSize"/>.
        /// </remarks>
        public Size MinSize { get => (Size)GetValue(MinSizeProperty); set => SetValue(MinSizeProperty, value); }

        /// <summary>The backing dependency property for <see cref="MinSize"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MinSizeProperty
            = DependencyProperty.Register(nameof(MinSize), typeof(Size), typeof(ParticleEmitter),
            new FrameworkPropertyMetadata(new Size(5, 5)));

        /// <summary>
        /// Get or set the maximum size that each created particle should be.
        /// </summary>
        /// <remarks>
        /// Make sure that the width and height values in <see cref="MinSize"/> are smaller than their respective values in <see cref="MaxSize"/>.
        /// <para/>
        /// Each created particle will have a randomly selected size, somewhere between <see cref="MinSize"/> and <see cref="MaxSize"/>.
        /// </remarks>
        public Size MaxSize { get => (Size)GetValue(MaxSizeProperty); set => SetValue(MaxSizeProperty, value); }

        /// <summary>The backing dependency property for <see cref="MaxSize"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MaxSizeProperty
            = DependencyProperty.Register(nameof(MaxSize), typeof(Size), typeof(ParticleEmitter),
            new FrameworkPropertyMetadata(new Size(20, 20)));

        /// <summary>
        /// Get or set whether the size of particles can change while they are alive. Use <see cref="SizeChangeVariance"/> to control how much the size can change by.
        /// </summary>
        /// <remarks>
        /// The particle's height and width will change independently of each other. The sizes will never be smaller than <see cref="MinSize"/> or 
        /// larger than <see cref="MaxSize"/>.
        /// </remarks>
        public bool ChangeSizeWhileAlive { get => (bool)GetValue(ChangeSizeWhileAliveProperty); set => SetValue(ChangeSizeWhileAliveProperty, value); }

        /// <summary>The backing dependency property for <see cref="ChangeSizeWhileAlive"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ChangeSizeWhileAliveProperty
            = DependencyProperty.Register(nameof(ChangeSizeWhileAlive), typeof(bool), typeof(ParticleEmitter),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Get or set whether the direction of particles can change while they are alive.
        /// </summary>
        public bool ChangeDirectionWhileAlive { get => (bool)GetValue(ChangeDirectionWhileAliveProperty); set => SetValue(ChangeDirectionWhileAliveProperty, value); }

        /// <summary>The backing dependency property for <see cref="ChangeDirectionWhileAlive"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ChangeDirectionWhileAliveProperty
            = DependencyProperty.Register(nameof(ChangeDirectionWhileAlive), typeof(bool), typeof(ParticleEmitter),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Get or set the maximum amount by which the size of particles can change while they are alive, if <see cref="ChangeSizeWhileAlive"/> is set to <c>true</c>.
        /// </summary>
        /// <remarks>
        /// The particle's height and width will change independently of each other. For each, it will choose a new value somewhere between 0 and this value 
        /// to increase or decrease by.
        /// The sizes will never be smaller than <see cref="MinSize"/> or larger than <see cref="MaxSize"/>.
        /// </remarks>
        public double SizeChangeVariance { get => (double)GetValue(SizeChangeVarianceProperty); set => SetValue(SizeChangeVarianceProperty, value); }

        /// <summary>The backing dependency property for <see cref="SizeChangeVariance"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SizeChangeVarianceProperty
            = DependencyProperty.Register(nameof(SizeChangeVariance), typeof(double), typeof(ParticleEmitter),
            new FrameworkPropertyMetadata(5.0));

        /// <summary>
        /// Get or set the maxmimum angle by which the direction of particles can change while they are alive, if <see cref="ChangeDirectionWhileAlive"/> is
        /// set to <c>true</c>. The angle is measured in radians.
        /// </summary>
        /// <remarks>
        /// This is an angle measured in radians (since C# uses radians for its trigonometric functions), you can use <see cref="AngleDegreesToRadians(double)"/>
        /// to convert an angle from degrees to radians.
        /// For each particle when it changes its direction, it will choose a new value somewhere between 0 and this value and change its direction, going either
        /// clockwise or counterclockwise from its existing direction.
        /// </remarks>
        public double DirectionChangeVariance { get => (double)GetValue(DirectionChangeVarianceProperty); set => SetValue(DirectionChangeVarianceProperty, value); }

        /// <summary>The backing dependency property for <see cref="DirectionChangeVariance"/>. See the related property for details.</summary>
        public static readonly DependencyProperty DirectionChangeVarianceProperty
            = DependencyProperty.Register(nameof(DirectionChangeVariance), typeof(double), typeof(ParticleEmitter),
            new FrameworkPropertyMetadata(1.0));

        /// <summary>
        /// Get or set the number of update ticks that a particle should wait before changing its size or direction, while <see cref="ChangeSizeWhileAlive"/>
        /// or <see cref="ChangeDirectionWhileAlive"/> are set to <c>true</c>.
        /// </summary>
        /// <remarks>
        /// This allows particles to keep their new size or direction for a period before changing again; set this to 0 to allow particles to change every tick.
        /// </remarks>
        public int VarianceCooldown { get => (int)GetValue(VarianceCooldownProperty); set => SetValue(VarianceCooldownProperty, value); }

        /// <summary>The backing dependency property for <see cref="VarianceCooldown"/>. See the related property for details.</summary>
        public static readonly DependencyProperty VarianceCooldownProperty
            = DependencyProperty.Register(nameof(VarianceCooldown), typeof(int), typeof(ParticleEmitter),
            new FrameworkPropertyMetadata(5));

        /// <summary>
        /// Get or set how often this emitter should run an update tick, with higher values making the update tick happen less frequently.
        /// On an update tick, the emitter will update all existing particles.
        /// </summary>
        /// <remarks>
        /// This can be changed to help with performance, with higher values being more performant. A value of <c>0</c> will cause this to run every
        /// time the particle emitter checks for ticks. This should be equal to or less than <see cref="EmitRate"/>, or unintended behavior may occur.
        /// <para/>
        /// The particle emitter operates by running an internal timer on another thread, and that thread sleeps at the amount of 
        /// <see cref="SleepPeriod"/> before checking for ticks. Thus, the most often an update tick may occur is every <see cref="SleepPeriod"/>
        /// milliseconds.
        /// </remarks>
        public int UpdateRate { get => (int)GetValue(UpdateRateProperty); set => SetValue(UpdateRateProperty, value); }

        /// <summary>The backing dependency property for <see cref="UpdateRate"/>. See the related property for details.</summary>
        public static readonly DependencyProperty UpdateRateProperty
            = DependencyProperty.Register(nameof(UpdateRate), typeof(int), typeof(ParticleEmitter),
            new FrameworkPropertyMetadata(1));


        /// <summary>
        /// Get or set how often this emitter should run an emit tick, with higher values making the emit tick happen less frequently.
        /// On an emit tick, the emitter will potentially create new particles (if <see cref="MaxCount"/> hasn't been reached).
        /// </summary>
        /// <remarks>
        /// This can be changed to help with performance, with higher values being more performant. A value of <c>0</c> will cause this to run every
        /// time the particle emitter checks for ticks. This should be equal to or greater than <see cref="UpdateRate"/>, or unintended behavior may occur.
        /// <para/>
        /// The particle emitter operates by running an internal timer on another thread, and that thread sleeps at the amount of 
        /// <see cref="SleepPeriod"/> before checking for ticks. Thus, the most often an emit tick may occur is every <see cref="SleepPeriod"/>
        /// milliseconds.
        /// </remarks>
        public int EmitRate { get => (int)GetValue(EmitRateProperty); set => SetValue(EmitRateProperty, value); }

        /// <summary>The backing dependency property for <see cref="EmitRate"/>. See the related property for details.</summary>
        public static readonly DependencyProperty EmitRateProperty
            = DependencyProperty.Register(nameof(EmitRate), typeof(int), typeof(ParticleEmitter),
            new FrameworkPropertyMetadata(1));

        /// <summary>
        /// Get or set the minimum number of particles to create (emit) during an emit tick.
        /// </summary>
        /// <remarks>
        /// If the number of existing particles is equal to or greater than <see cref="MaxCount"/>, then no new particles will be created
        /// on an emit tick.
        /// </remarks>
        public uint EmitCountMin { get => (uint)GetValue(EmitCountMinProperty); set => SetValue(EmitCountMinProperty, value); }

        /// <summary>The backing dependency property for <see cref="EmitCountMin"/>. See the related property for details.</summary>
        public static readonly DependencyProperty EmitCountMinProperty
            = DependencyProperty.Register(nameof(EmitCountMin), typeof(uint), typeof(ParticleEmitter),
            new FrameworkPropertyMetadata((uint)2));

        /// <summary>
        /// Get or set the maximum number of particles to create (emit) during an emit tick.
        /// </summary>
        /// <remarks>
        /// If the number of existing particles is equal to or greater than <see cref="MaxCount"/>, then no new particles will be created
        /// on an emit tick.
        /// </remarks>
        public uint EmitCountMax { get => (uint)GetValue(EmitCountMaxProperty); set => SetValue(EmitCountMaxProperty, value); }

        /// <summary>The backing dependency property for <see cref="EmitCountMax"/>. See the related property for details.</summary>
        public static readonly DependencyProperty EmitCountMaxProperty
            = DependencyProperty.Register(nameof(EmitCountMax), typeof(uint), typeof(ParticleEmitter),
            new FrameworkPropertyMetadata((uint)7));

        #endregion

        #region Variables / Status Properties

        // counters
        int emitCounter = 0;
        int updateCounter = 0;

        // values to reset counters back to after hitting 0
        int emitMax = 0;
        int updateMax = 0;

        // status variables
        bool active = true;
        bool enabled = false;

        // the amount to sleep
        int sleepAmt = 16;

        /// <summary>
        /// The amount that the particle emitter's thread should sleep before trying to run another tick. By default, this is 16.
        /// </summary>
        public int SleepPeriod
        {
            get => sleepAmt;
            set
            {
                if (IsRunning) return;
                else sleepAmt = value;
            }
        }

        /// <summary>
        /// Get if this particle emitter is currently enabled and running.
        /// </summary>
        public bool IsRunning { get => enabled; }

        /// <summary>
        /// Get if this particle emitter has been shutdown. This is changed to <c>true</c> after <see cref="Shutdown"/> or <see cref="Dispose"/> is called.
        /// </summary>
        public bool IsShutdown { get; private set; } = false;

        // internal call to clear all particles
        bool cullAll = false;

        #endregion

        #region Emitter Commands

        /// <summary>
        /// Start the emitter.
        /// </summary>
        public void Start()
        {
            if (rthr.ThreadState == ThreadState.Running) return;
            if (IsShutdown) return;


            // let's do a bit of data validation right now
            if (EmitCountMax < EmitCountMin) // the max is less than the min, let's flip them around
            {
                uint tempEmit = EmitCountMax;
                EmitCountMax = EmitCountMin;
                EmitCountMin = tempEmit;
            }

            if (MaxLife < MinLife) // the max is less than the min, let's flip them around
            {
                int tempLife = MaxLife;
                MaxLife = MinLife;
                MinLife = tempLife;
            }

            ParticleBrush?.Freeze();

            // updating internal properties
            emitMax = EmitRate;
            updateMax = UpdateRate;

            emitCounter = emitMax;
            updateCounter = updateMax;

            // let's get rolling
            active = true;
            enabled = true;

            rthr.Start();
        }

        /// <summary>
        /// Pause the already-started emitter.
        /// </summary>
        public void Pause()
        {
            enabled = false;
        }

        /// <summary>
        /// Resume the emitter from being paused.
        /// </summary>
        public void Resume()
        {
            emitMax = EmitRate;
            updateMax = UpdateRate;

            enabled = true;
        }

        /// <summary>
        /// Shutdown the emitter. Once shutdown, the emitter cannot be started or resumed again.
        /// </summary>
        public void Shutdown()
        {
            active = false;
            enabled = false;

            if (IsShutdown) return;
            rthr.Join(1000);
            IsShutdown = true;
        }

        /// <summary>
        /// Clear and remove all particles in this emitter.
        /// </summary>
        public void Clear()
        {
            if (IsRunning && enabled)
            {
                // we'll tell the thread to cull all, so that we're not causing any thread race issues
                cullAll = true;
            }
            else
            {
                // okay, we can just do it ourselves
                g?.Children.Clear();
            }
        }

        #endregion

        #region Emitter Logic

        Action<ParticleEmitter> threadRun = (ParticleEmitter pe) =>
        {
            Task.WaitAll(new Task[] {
            Task.Run(() =>
            {
                if (pe.enabled)
                {
                    pe.updateCounter--;
                    pe.emitCounter--;

                    if (pe.emitCounter <= 0)
                    {
                        pe.Emit();
                        pe.emitCounter = pe.emitMax;
                    }

                    if (pe.cullAll) // told to clear all particles
                    {
                        pe.Dispatcher.Invoke(() =>
                        {
                            pe.g?.Children.Clear();
                        });
                        pe.cullAll = false;
                    }

                    if (pe.updateCounter <= 0)
                    {
                        pe.UpdateItems();
                        pe.updateCounter = pe.updateMax;
                    }
                }
            }),
            Task.Delay(pe.sleepAmt)});
        };

        /// <summary>
        /// Create new particles.
        /// </summary>
        void Emit()
        {
            Dispatcher.Invoke(() =>
            {
                if (g == null) return;

                if (g.Children.Count >= MaxCount) return;

                List<ParticleItem> newGuys = new List<ParticleItem>();
                Random r = new Random();
                int emitCount = r.Next((int)EmitCountMin, (int)EmitCountMax);

                for (int i = 0; i < emitCount; i++)
                {
                    ParticleItem newGuy = CreateParticle();
                    newGuys.Add(newGuy);
                }

                foreach (ParticleItem newGuy in newGuys)
                {
                    g.Children.Add(newGuy);
                }
            });

            ParticleItem CreateParticle()
            {
                Random r = new Random();
                ParticleItem b = new ParticleItem();

                double startX = r.NextDouble() * ActualWidth;
                double startY = r.NextDouble() * ActualHeight;

                b.Background = ParticleBrush;
                b.Margin = new Thickness(startX, startY, -6000, -6000);
                b.HorizontalAlignment = HorizontalAlignment.Left;
                b.VerticalAlignment = VerticalAlignment.Top;

                b.Width = MinSize.Width + (r.NextDouble() * (MaxSize.Width - MinSize.Width));
                b.Height = MinSize.Height + (r.NextDouble() * (MaxSize.Height - MinSize.Height));
                b.Life = r.Next(MinLife, MaxLife);
                b.Acceleration = MinSpeed + (r.NextDouble() * (MaxSpeed - MinSpeed));

                if (VarianceCooldown >= 1)
                {
                    b.SizeVarianceCooldown = r.Next(1, VarianceCooldown);
                    b.DirectionVarianceCooldown = r.Next(1, VarianceCooldown);
                }

                if (ParticleDirection.Length == 0)
                {
                    double angle = r.NextDouble() * 360;
                    Vector v = GetVectorFromAngle(angle);
                    b.Direction = v;
                }
                else
                {
                    b.Direction = ParticleDirection;
                }

                b.UpdateVectorSpeed();

                return b;
            }
        }

        /// <summary>
        /// Convert an angle value from degrees to radians. C#'s math values use radians for its trigonometry calculations.
        /// </summary>
        /// <param name="angle">The angle value to convert, in degrees.</param>
        /// <returns>The equivalent angle amount in radians.</returns>
        public static double AngleDegreesToRadians(double angle)
        {
            return angle * (Math.PI / 180);
        }

        static Vector GetVectorFromAngle(double angle)
        {
            double angleInRadians = AngleDegreesToRadians(angle);

            // Calculate the vector components
            double x = Math.Cos(angleInRadians);
            double y = Math.Sin(angleInRadians);

            Vector v = new Vector(x, y);
            v.Normalize();

            return v;
        }

        void UpdateItems()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (g == null) return;

                List<ParticleItem> cullList = new List<ParticleItem>();

                foreach (var item in g.Children)
                {
                    if (item is ParticleItem pi)
                    {
                        pi.Margin = AdjustMargin(pi.Margin, pi.Direction.X, 0, pi.Direction.Y, 0);
                        pi.Life--;

                        if (pi.Life <= 0)
                        {
                            cullList.Add(pi);
                        }
                        else
                        {
                            Random r = new Random();
                            if (ChangeDirectionWhileAlive)
                            {
                                if (pi.DirectionVarianceCooldown > 0)
                                {
                                    pi.DirectionVarianceCooldown--;
                                }
                                else
                                {
                                    double dr1 = r.NextDouble();
                                    double dirVar = DirectionChangeVariance * r.NextDouble() * (dr1 >= 0.5 ? 1 : -1);

                                    // Original vector
                                    (double x, double y) = (pi.Direction.X, pi.Direction.Y);

                                    // Calculate the rotated vector
                                    double newX = x * Math.Cos(dirVar) - y * Math.Sin(dirVar);
                                    double newY = x * Math.Sin(dirVar) + y * Math.Cos(dirVar);

                                    pi.Direction = new Vector(newX, newY);
                                    pi.UpdateVectorSpeed();

                                    pi.DirectionVarianceCooldown = VarianceCooldown;
                                }
                            }

                            if (ChangeSizeWhileAlive)
                            {
                                if (pi.SizeVarianceCooldown > 0)
                                {
                                    pi.SizeVarianceCooldown--;
                                }
                                else
                                {
                                    // generate random numbers
                                    double dr1 = r.NextDouble();
                                    double sizeVar = SizeChangeVariance * r.NextDouble() * (dr1 >= 0.5 ? 1 : -1);

                                    // create new width and height, and do validate values
                                    double newWidth = pi.Width + sizeVar;
                                    if (newWidth < 0 || newWidth < MinSize.Width) newWidth = Math.Max(0, MinSize.Width);
                                    if (newWidth > MaxSize.Width) newWidth = MaxSize.Width;
                                    double newHeight = pi.Height + sizeVar;
                                    if (newHeight < 0 || newHeight < MinSize.Height) newHeight = Math.Max(0, MinSize.Height);
                                    if (newHeight > MaxSize.Height) newHeight = MaxSize.Height;

                                    // set new width and height to item
                                    pi.Width = newWidth;
                                    pi.Height = newHeight;

                                    pi.SizeVarianceCooldown = VarianceCooldown;
                                }
                            }
                        }

                    }
                }

                foreach (ParticleItem item in cullList)
                {
                    g.Children.Remove(item);
                }
            }));

        }

        static Thickness AdjustMargin(Thickness t, double left = 0, double right = 0, double top = 0, double bottom = 0)
        {
            return new Thickness(t.Left + left, t.Top + top, t.Right + right, t.Bottom + bottom);
        }

        #endregion

        /// <summary>Shutdown this particle emitter. This should be called when you're done needing the emitter.</summary>
        public void Dispose()
        {
            Shutdown();
            GC.SuppressFinalize(this);
        }

        #region ParticleItem class

        private sealed class ParticleItem : Border
        {
            /// <summary>
            /// The distance by which this particle should travel each update tick. If this is changed, call <see cref="UpdateVectorSpeed"/> afterward.
            /// </summary>
            public double Acceleration { get => (double)GetValue(AccelerationProperty); set => SetValue(AccelerationProperty, value); }

            /// <summary>The backing dependency property for <see cref="Acceleration"/>. See the related property for details.</summary>
            public static readonly DependencyProperty AccelerationProperty
                = DependencyProperty.Register(nameof(Acceleration), typeof(double), typeof(ParticleItem),
                new FrameworkPropertyMetadata(1.0));

            /// <summary>
            /// The amount of life this particle has left. Once it hits 0, the particle emitter will remove this particle.
            /// </summary>
            public int Life { get => (int)GetValue(LifeProperty); set => SetValue(LifeProperty, value); }

            /// <summary>The backing dependency property for <see cref="Life"/>. See the related property for details.</summary>
            public static readonly DependencyProperty LifeProperty
                = DependencyProperty.Register(nameof(Life), typeof(int), typeof(ParticleItem),
                new FrameworkPropertyMetadata(10));

            /// <summary>
            /// The direction and speed this particle should move.
            /// </summary>
            public Vector Direction { get => (Vector)GetValue(DirectionProperty); set => SetValue(DirectionProperty, value); }

            /// <summary>The backing dependency property for <see cref="Direction"/>. See the related property for details.</summary>
            public static readonly DependencyProperty DirectionProperty
                = DependencyProperty.Register(nameof(Direction), typeof(Vector), typeof(ParticleItem),
                new FrameworkPropertyMetadata(new Vector(0, 0)));

            /// <summary>
            /// Generate a new Direction value by normalizing the existing direction and applying the Acceleration value.
            /// </summary>
            public void UpdateVectorSpeed()
            {
                Vector v = Direction;
                v.Normalize();
                Direction = v * Acceleration;
            }

            /// <summary>
            /// The cooldown amount before this particle will change its size (if the parent emitter is set to allow changes while alive).
            /// </summary>
            public int SizeVarianceCooldown { get => (int)GetValue(SizeVarianceCooldownProperty); set => SetValue(SizeVarianceCooldownProperty, value); }

            /// <summary>The backing dependency property for <see cref="SizeVarianceCooldown"/>. See the related property for details.</summary>
            public static readonly DependencyProperty SizeVarianceCooldownProperty
                = DependencyProperty.Register(nameof(SizeVarianceCooldown), typeof(int), typeof(ParticleItem),
                new FrameworkPropertyMetadata(3));

            /// <summary>
            /// The cooldown amount before this particle will change its direction (if the parent emitter is set to allow changes while alive).
            /// </summary>
            public int DirectionVarianceCooldown { get => (int)GetValue(DirectionVarianceCooldownProperty); set => SetValue(DirectionVarianceCooldownProperty, value); }

            /// <summary>The backing dependency property for <see cref="DirectionVarianceCooldown"/>. See the related property for details.</summary>
            public static readonly DependencyProperty DirectionVarianceCooldownProperty
                = DependencyProperty.Register(nameof(DirectionVarianceCooldown), typeof(int), typeof(ParticleItem),
                new FrameworkPropertyMetadata(3));

        }

        #endregion

    }
}
