#if ANDROID
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Microsoft.Maui.Handlers;
using AndroidX.AppCompat.Widget;
using Android.Widget;
using Android.Text;
using Android.Graphics;
using Android.OS;
using System.IO;
using Paint = Android.Graphics.Paint;
using Color = Android.Graphics.Color;
using Path = Android.Graphics.Path;

#endif


namespace CustomDrawingPad
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }
    }

    public class CustomDrawingPad : Microsoft.Maui.Controls.View
    {
        public CustomDrawingPad()
        {
            // Set any default properties for the control
        }
    }

#if ANDROID

    public class CustomDrawingPadHandler : Microsoft.Maui.Handlers.ViewHandler<CustomDrawingPad, Android.Views.View>
{
        private CustomDrawingView? drawingView;
        private Bitmap? bitmap;
        private Canvas? canvas;
        private Android.Graphics.Paint? paint;
        private Path? path;

    public static IPropertyMapper<CustomDrawingPad, CustomDrawingPadHandler> PropertyMapper = new PropertyMapper<CustomDrawingPad, CustomDrawingPadHandler>(ViewHandler.ViewMapper)
    {
       
    };

    public static CommandMapper<CustomDrawingPad, CustomDrawingPadHandler> CommandMapper = new(ViewCommandMapper)
    {
    };

    public CustomDrawingPadHandler() : base(PropertyMapper, CommandMapper)
    {

    }

    protected override Android.Views.View CreatePlatformView()
        {
            drawingView = new CustomDrawingView(Context);
            return drawingView;
        }

        protected override void DisconnectHandler(Android.Views.View platformView)
        {
            base.DisconnectHandler(platformView);
            drawingView?.Dispose();
            drawingView = null;
        }

        protected override void ConnectHandler(Android.Views.View platformView)
        {
            base.ConnectHandler(platformView);
            //InitializeDrawingTools();
        }

        
}

public class CustomDrawingView : Android.Views.View
{
    private Paint paint;
    private Path path;  // This should be an instance variable, not static
    private Bitmap bitmap;
    private Canvas canvas;

    public CustomDrawingView(Context context) : base(context)
    {
        Initialize();
    }

    private void Initialize()
    {
        // Initialize the paint and path for drawing
        paint = new Paint
        {
            Color = Color.Black,
            AntiAlias = true,
            StrokeWidth = 5f,
            StrokeCap = Paint.Cap.Round,
            StrokeJoin = Paint.Join.Round
        };
        paint.SetStyle(Paint.Style.Stroke); // Set the paint style to stroke for drawing outlines

        path = new Path();  // Initialize a new instance of Path
        bitmap = Bitmap.CreateBitmap(1000, 1000, Bitmap.Config.Argb8888);
        canvas = new Canvas(bitmap);
    }

    protected override void OnDraw(Canvas canvas)
    {
        base.OnDraw(canvas);
        // Draw the existing bitmap content first
        canvas.DrawBitmap(bitmap, 0, 0, null);
        // Draw the current path
        canvas.DrawPath(path, paint);
    }

    public override bool OnTouchEvent(MotionEvent e)
    {
        // Handle touch events for drawing
        float x = e.GetX();
        float y = e.GetY();

        switch (e.Action)
        {
            case MotionEventActions.Down:
                // When touch starts, move to the touched position
                path.MoveTo(x, y);
                break;
            case MotionEventActions.Move:
                // When touch moves, draw a line to the new position
                path.LineTo(x, y);
                break;
            case MotionEventActions.Up:
                // When touch ends, draw the path on the bitmap
                canvas.DrawPath(path, paint);
                path.Reset();  // Reset the path after drawing
                break;
        }

        // Request to redraw the view
        Invalidate();
        return true;
    }

    protected override IParcelable OnSaveInstanceState()
    {
        var savedState = base.OnSaveInstanceState();

        if (bitmap != null)
        {
            using var memoryStream = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, memoryStream);
            var byteArray = memoryStream.ToArray();

            var bundle = new Bundle();
            bundle.PutParcelable("instanceState", savedState);
            bundle.PutByteArray("bitmap", byteArray);
            return bundle;
        }

        return savedState;
    }

    protected override void OnRestoreInstanceState(IParcelable state)
    {
        if (state is Bundle bundle)
        {
            var savedState = bundle.GetParcelable("instanceState");
            var byteArray = bundle.GetByteArray("bitmap");

            if (byteArray != null)
            {
                bitmap = BitmapFactory.DecodeByteArray(byteArray, 0, byteArray.Length);
                if (bitmap != null)
                {
                    canvas = new Canvas(bitmap);
                }
            }

            base.OnRestoreInstanceState((IParcelable?)savedState);
        }
        else
        {
            base.OnRestoreInstanceState(state);
        }
    }
}

#endif

}
